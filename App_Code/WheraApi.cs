using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace WheraOwnerTenant
{
    // Compativel com C# 5 (sem interpolacao de string, sem ?., sem async/await).
    // Chamadas à API Whera feitas pelo servidor, com o token da sessão no header Authorization.

    public class WheraApiException : Exception
    {
        public int StatusCode { get; private set; }
        public string Corpo { get; private set; }
        public string RetryAfter { get; private set; }

        public WheraApiException(int statusCode, string mensagem, string corpo)
            : this(statusCode, mensagem, corpo, null) { }

        public WheraApiException(int statusCode, string mensagem, string corpo, string retryAfter)
            : base(mensagem)
        {
            StatusCode = statusCode;
            Corpo = corpo;
            RetryAfter = retryAfter;
        }
    }

    public class WheraApi
    {
        private readonly string token;

        public WheraApi(string token)
        {
            this.token = token;
        }

        private static string UrlBase
        {
            get { return (ConfigurationManager.AppSettings["WheraApiUrl"] ?? "").TrimEnd('/'); }
        }

        /// <summary>Imagem servida pela própria API ("/midias/..."): completa com o endereço da API.</summary>
        public static string UrlMidia(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            return url.StartsWith("/") && !url.StartsWith("//") ? UrlBase + url : url;
        }

        private static int TimeoutMs
        {
            get
            {
                int segundos;
                return int.TryParse(ConfigurationManager.AppSettings["WheraApiTimeout"], out segundos) ? segundos * 1000 : 60000;
            }
        }

        public T Get<T>(string rota)
        {
            return Enviar<T>("GET", rota, null);
        }

        public T Post<T>(string rota, object corpo)
        {
            return Enviar<T>("POST", rota, corpo);
        }

        public T Put<T>(string rota, object corpo)
        {
            return Enviar<T>("PUT", rota, corpo);
        }

        public T Patch<T>(string rota, object corpo)
        {
            return Enviar<T>("PATCH", rota, corpo);
        }

        public void Delete(string rota)
        {
            Enviar<object>("DELETE", rota, null);
        }

        /// <summary>Envia um arquivo em multipart/form-data (ex.: imagem em api/midias/imagens, campo "arquivo").</summary>
        public T EnviarArquivo<T>(string rota, string campo, string nomeArquivo, string tipoConteudo, byte[] conteudo)
        {
            return EnviarArquivo<T>(rota, campo, nomeArquivo, tipoConteudo, conteudo, TimeoutMs);
        }

        /// <param name="tempoLimiteMs">Para envios demorados (ex.: importação em lote), maior que o padrão.</param>
        public T EnviarArquivo<T>(string rota, string campo, string nomeArquivo, string tipoConteudo, byte[] conteudo, int tempoLimiteMs)
        {
            string limite = "----WheraOwnerTenant" + Guid.NewGuid().ToString("N");
            string nome = (nomeArquivo ?? "arquivo").Replace("\"", "").Replace("\r", "").Replace("\n", "");

            byte[] cabecalho = Encoding.UTF8.GetBytes("--" + limite + "\r\n" +
                "Content-Disposition: form-data; name=\"" + campo + "\"; filename=\"" + nome + "\"\r\n" +
                "Content-Type: " + (string.IsNullOrEmpty(tipoConteudo) ? "application/octet-stream" : tipoConteudo) + "\r\n\r\n");
            byte[] rodape = Encoding.UTF8.GetBytes("\r\n--" + limite + "--\r\n");

            var corpo = new byte[cabecalho.Length + conteudo.Length + rodape.Length];
            Buffer.BlockCopy(cabecalho, 0, corpo, 0, cabecalho.Length);
            Buffer.BlockCopy(conteudo, 0, corpo, cabecalho.Length, conteudo.Length);
            Buffer.BlockCopy(rodape, 0, corpo, cabecalho.Length + conteudo.Length, rodape.Length);

            return Executar<T>("POST", rota, corpo, "multipart/form-data; boundary=" + limite, tempoLimiteMs);
        }

        private T Enviar<T>(string metodo, string rota, object corpo)
        {
            byte[] bytes = corpo == null ? null
                : Encoding.UTF8.GetBytes(new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(corpo));
            return Executar<T>(metodo, rota, bytes, "application/json; charset=utf-8", TimeoutMs);
        }

        private T Executar<T>(string metodo, string rota, byte[] bytes, string tipoCorpo, int tempoLimiteMs)
        {
            var serializador = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };

            var requisicao = (HttpWebRequest)WebRequest.Create(UrlBase + "/" + rota.TrimStart('/'));
            requisicao.Method = metodo;
            requisicao.Accept = "application/json";
            requisicao.Timeout = tempoLimiteMs;
            requisicao.ReadWriteTimeout = tempoLimiteMs;
            if (!string.IsNullOrEmpty(token))
                requisicao.Headers[HttpRequestHeader.Authorization] = "Bearer " + token;

            if (bytes != null)
            {
                requisicao.ContentType = tipoCorpo;
                requisicao.ContentLength = bytes.Length;
                using (var fluxo = requisicao.GetRequestStream())
                    fluxo.Write(bytes, 0, bytes.Length);
            }

            int status;
            string texto;
            try
            {
                using (var resposta = (HttpWebResponse)requisicao.GetResponse())
                {
                    status = (int)resposta.StatusCode;
                    texto = LerCorpo(resposta);
                }
            }
            catch (WebException ex)
            {
                var resposta = ex.Response as HttpWebResponse;
                if (resposta == null)
                    throw new WheraApiException(0, "API Whera indisponível: " + ex.Message, null);

                using (resposta)
                {
                    status = (int)resposta.StatusCode;
                    string retryAfter = resposta.Headers["Retry-After"];
                    texto = LerCorpo(resposta);
                    if (status < 200 || status > 299)
                        throw new WheraApiException(status, MensagemErro(serializador, status, texto), texto, retryAfter);
                }
            }

            if (status < 200 || status > 299)
                throw new WheraApiException(status, MensagemErro(serializador, status, texto), texto);

            if (string.IsNullOrWhiteSpace(texto)) return default(T);
            return serializador.Deserialize<T>(texto);
        }

        private static string LerCorpo(HttpWebResponse resposta)
        {
            using (var leitor = new StreamReader(resposta.GetResponseStream(), Encoding.UTF8))
                return leitor.ReadToEnd();
        }

        /// <summary>
        /// A API responde erro como ProblemDetails: usa "detail" ou "title" quando vierem. No erro de validação (400),
        /// junta as mensagens de cada campo ("errors").
        /// </summary>
        private static string MensagemErro(JavaScriptSerializer serializador, int status, string texto)
        {
            try
            {
                var problema = serializador.Deserialize<ProblemaApi>(texto);
                if (problema != null && problema.errors != null && problema.errors.Count > 0)
                {
                    var mensagens = new List<string>();
                    foreach (var campo in problema.errors)
                        foreach (var mensagem in campo.Value ?? new List<string>())
                            if (!mensagens.Contains(mensagem)) mensagens.Add(mensagem);
                    if (mensagens.Count > 0) return string.Join(" ", mensagens);
                }
                if (problema != null && !string.IsNullOrWhiteSpace(problema.detail)) return problema.detail;
                if (problema != null && !string.IsNullOrWhiteSpace(problema.title)) return problema.title;
            }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }

            return "A API Whera recusou a solicitação (HTTP " + status + ").";
        }

        private class ProblemaApi
        {
            public string title { get; set; }
            public string detail { get; set; }
            public Dictionary<string, List<string>> errors { get; set; }
        }
    }
}
