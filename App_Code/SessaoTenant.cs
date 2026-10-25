using System;
using System.Configuration;
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
        private const string ChavePapel = "WheraOwnerTenant.Papel";
        private const string ChaveEmpresaSelecionada = "WheraOwnerTenant.EmpresaSelecionada";
        private const string ChaveAviso = "WheraOwnerTenant.Aviso";
        private const string TokenPreview = "whera-dev-preview";

        public static void Iniciar(string token, DateTime expiraEmUtc, string nome, string email, string papel)
        {
            var sessao = HttpContext.Current.Session;
            sessao[ChaveToken] = token;
            sessao[ChaveExpiraEm] = expiraEmUtc;
            sessao[ChaveNome] = nome;
            sessao[ChaveEmail] = email;
            sessao[ChavePapel] = papel;
            sessao.Remove(ChaveEmpresaSelecionada);
            sessao.Remove(ChaveAviso);
        }

        public static void Encerrar()
        {
            var sessao = HttpContext.Current.Session;
            if (sessao == null) return;
            sessao.Remove(ChaveToken);
            sessao.Remove(ChaveExpiraEm);
            sessao.Remove(ChaveNome);
            sessao.Remove(ChaveEmail);
            sessao.Remove(ChavePapel);
            sessao.Remove(ChaveEmpresaSelecionada);
            sessao.Remove(ChaveAviso);
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

        public static bool PreviewDisponivel
        {
            get
            {
                bool configurado;
                return HttpContext.Current != null && HttpContext.Current.Request.IsLocal
                    && bool.TryParse(ConfigurationManager.AppSettings["WheraDevPreview"], out configurado) && configurado;
            }
        }

        public static bool DevPreviewAtiva
        {
            get { return PreviewDisponivel && string.Equals(Token, TokenPreview, StringComparison.Ordinal); }
        }

        public static void IniciarPreview()
        {
            if (!PreviewDisponivel) throw new InvalidOperationException("A prévia local está desativada.");
            Iniciar(TokenPreview, DateTime.UtcNow.AddHours(8), "Prévia local", "preview@local", "Tenant");
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

        public static string Papel { get { return HttpContext.Current.Session[ChavePapel] as string; } }
        public static bool EhTenant { get { return string.Equals(Papel, "Tenant", StringComparison.OrdinalIgnoreCase); } }
        public static bool EhGerente { get { return string.Equals(Papel, "Gerente", StringComparison.OrdinalIgnoreCase); } }
        public static bool EhOwner { get { return string.Equals(Papel, "Owner", StringComparison.OrdinalIgnoreCase); } }
        public static string EmpresaSelecionadaId { get { return HttpContext.Current.Session[ChaveEmpresaSelecionada] as string; } }
        public static void SelecionarEmpresa(string empresaId) { HttpContext.Current.Session[ChaveEmpresaSelecionada] = empresaId; }
        public static void DefinirAviso(string mensagem) { HttpContext.Current.Session[ChaveAviso] = mensagem; }
        public static string RetirarAviso() { var aviso = HttpContext.Current.Session[ChaveAviso] as string; HttpContext.Current.Session.Remove(ChaveAviso); return aviso; }
    }
}
