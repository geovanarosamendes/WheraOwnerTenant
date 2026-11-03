using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace WheraOwnerTenant
{
    /// <summary>
    /// Envia à API os erros não tratados deste painel, para aparecerem na área de TI do Owner.
    /// Roda fora da requisição e nunca lança exceção: uma falha ao avisar não pode piorar a falha original.
    /// A API mascara os dados sensíveis e descarta a query string ao receber.
    /// </summary>
    public static class RegistroErros
    {
        private const string Origem = "EloGestao";
        private const int TamanhoMaximoDetalhe = 16000;
        private const int TempoLimiteMs = 3000;

        public static void Enviar(Exception erro)
        {
            try
            {
                string urlBase = WheraApi.UrlBase;
                bool previa = string.Equals(ConfigurationManager.AppSettings["WheraDevPreview"], "true", StringComparison.OrdinalIgnoreCase);
                if (erro == null || urlBase.Length == 0 || previa) return;

                var http = erro as HttpException;
                string detalhe = erro.ToString();
                if (detalhe.Length > TamanhoMaximoDetalhe) detalhe = detalhe.Substring(0, TamanhoMaximoDetalhe);

                string corpo = new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(new
                {
                    Origem = Origem,
                    // Página inexistente é aviso; só 5xx é erro do painel.
                    Nivel = http != null && http.GetHttpCode() < 500 ? "Aviso" : "Erro",
                    Mensagem = erro.GetBaseException().Message,
                    Detalhe = detalhe,
                    Rota = HttpContext.Current != null ? HttpContext.Current.Request.Path : null,
                    Plataforma = "servidor"
                });

                ThreadPool.QueueUserWorkItem(_ => Postar(urlBase + "/api/logs", corpo));
            }
            catch (Exception)
            {
            }
        }

        private static void Postar(string url, string corpo)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(corpo);
                var requisicao = (HttpWebRequest)WebRequest.Create(url);
                requisicao.Method = "POST";
                requisicao.ContentType = "application/json; charset=utf-8";
                requisicao.ContentLength = bytes.Length;
                requisicao.Timeout = TempoLimiteMs;
                requisicao.ReadWriteTimeout = TempoLimiteMs;
                using (var fluxo = requisicao.GetRequestStream())
                    fluxo.Write(bytes, 0, bytes.Length);
                using (requisicao.GetResponse()) { }
            }
            catch (Exception)
            {
            }
        }
    }
}
