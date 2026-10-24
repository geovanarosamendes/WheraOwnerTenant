using System;
using System.Web;

namespace WheraOwnerTenant
{
    // Compativel com C# 5 (Web Site sem Roslyn): sem interpolacao de string, sem ?., sem async/await.

    /// <summary>
    /// Sessão deste site: só o token devolvido pela API Whera (POST api/autenticacao/login).
    /// Quando o token vence, o usuário volta para o login.
    /// </summary>
    public static class SessaoTenant
    {
        private const string ChaveToken = "WheraOwnerTenant.Token";
        private const string ChaveExpiraEm = "WheraOwnerTenant.ExpiraEm";
        private const string ChaveNome = "WheraOwnerTenant.Nome";
        private const string ChaveEmail = "WheraOwnerTenant.Email";

        public static void Iniciar(string token, DateTime expiraEmUtc, string nome, string email)
        {
            var sessao = HttpContext.Current.Session;
            sessao[ChaveToken] = token;
            sessao[ChaveExpiraEm] = expiraEmUtc;
            sessao[ChaveNome] = nome;
            sessao[ChaveEmail] = email;
        }

        public static void Encerrar()
        {
            var sessao = HttpContext.Current.Session;
            if (sessao == null) return;
            sessao.Remove(ChaveToken);
            sessao.Remove(ChaveExpiraEm);
            sessao.Remove(ChaveNome);
            sessao.Remove(ChaveEmail);
        }

        /// <summary>Token válido ou null (sem login ou vencido).</summary>
        public static string Token
        {
            get
            {
                var sessao = HttpContext.Current.Session;
                if (sessao == null || sessao[ChaveToken] == null) return null;
                if (!(sessao[ChaveExpiraEm] is DateTime) || (DateTime)sessao[ChaveExpiraEm] <= DateTime.UtcNow) return null;
                return (string)sessao[ChaveToken];
            }
        }

        public static bool Ativa
        {
            get { return Token != null; }
        }

        /// <summary>Havia login, mas o token venceu (para avisar na tela de login).</summary>
        public static bool Expirou
        {
            get
            {
                var sessao = HttpContext.Current.Session;
                return sessao != null && sessao[ChaveToken] != null && Token == null;
            }
        }

        public static string Nome
        {
            get { return HttpContext.Current.Session[ChaveNome] as string; }
        }

        public static string Email
        {
            get { return HttpContext.Current.Session[ChaveEmail] as string; }
        }
    }
}
