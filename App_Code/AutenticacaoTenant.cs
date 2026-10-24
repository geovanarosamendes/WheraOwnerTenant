using System;
using System.Globalization;
using System.Web;

namespace WheraOwnerTenant
{
    // Compativel com C# 5 (sem interpolacao de string, sem ?., sem async/await).

    // Formato devolvido por POST api/autenticacao/login (LoginResponse da API Whera).
    public class LoginResposta
    {
        public string Etapa { get; set; }
        public string PapelReal { get; set; }
        public SessaoResposta Sessao { get; set; }
    }

    public class SessaoResposta
    {
        public string AccessToken { get; set; }
        /// <summary>Data ISO 8601 em UTC (o JavaScriptSerializer não lê esse formato direto em DateTime).</summary>
        public string ExpiraEm { get; set; }
        public UsuarioResposta Usuario { get; set; }
        public string Papel { get; set; }
    }

    public class UsuarioResposta
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Login do painel tenant em duas etapas na mesma rota da API:
    /// 1) e-mail + senha → a API responde "SegundaSenha" (sem token);
    /// 2) e-mail + senha + senha de administrador → a API devolve o token.
    /// Entre as etapas, e-mail e senha ficam só na Session do servidor, por no máximo 5 minutos.
    /// </summary>
    public static class AutenticacaoTenant
    {
        public const string PapelOwner = "Owner";
        public const string EtapaConcluido = "Concluido";
        public const string EtapaSegundaSenha = "SegundaSenha";

        private const string ChavePendente = "WheraOwnerTenant.LoginPendente";
        private static readonly TimeSpan ValidadePendente = TimeSpan.FromMinutes(5);

        private class LoginPendente
        {
            public string Email;
            public string Senha;
            public DateTime CriadoEm;
        }

        public static LoginResposta Enviar(string email, string senha, string segundaSenha)
        {
            return new WheraApi(null).Post<LoginResposta>("api/autenticacao/login",
                new { Email = email, Senha = senha, SegundaSenha = segundaSenha });
        }

        public static void GuardarPendente(string email, string senha)
        {
            HttpContext.Current.Session[ChavePendente] = new LoginPendente { Email = email, Senha = senha, CriadoEm = DateTime.UtcNow };
        }

        /// <summary>E-mail e senha da etapa 1, ou null se não houver ou tiver passado do prazo.</summary>
        public static bool ObterPendente(out string email, out string senha)
        {
            email = null;
            senha = null;

            var pendente = HttpContext.Current.Session[ChavePendente] as LoginPendente;
            if (pendente == null) return false;

            if (DateTime.UtcNow - pendente.CriadoEm > ValidadePendente)
            {
                DescartarPendente();
                return false;
            }

            email = pendente.Email;
            senha = pendente.Senha;
            return true;
        }

        public static void DescartarPendente()
        {
            HttpContext.Current.Session.Remove(ChavePendente);
        }

        /// <summary>Grava a sessão do site a partir da resposta "Concluido" da API.</summary>
        public static void IniciarSessao(SessaoResposta sessao)
        {
            DateTime expiraEm = DateTime.Parse(sessao.ExpiraEm, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

            SessaoTenant.Iniciar(sessao.AccessToken, expiraEm,
                sessao.Usuario != null ? sessao.Usuario.Nome : null,
                sessao.Usuario != null ? sessao.Usuario.Email : null);
        }

        /// <summary>Mensagem para a tela a partir do erro da API.</summary>
        public static string MensagemErro(WheraApiException ex)
        {
            if (ex.StatusCode == 0) return "Não foi possível falar com a API Whera. Tente novamente em instantes.";
            if (ex.StatusCode == 429) return "Muitas tentativas seguidas. Aguarde um minuto e tente de novo.";
            return ex.Message;
        }
    }
}
