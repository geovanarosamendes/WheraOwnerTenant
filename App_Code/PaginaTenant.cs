using System;
using System.Web;
using System.Web.UI;

namespace WheraOwnerTenant
{
    /// <summary>
    /// Base de toda tela do painel: sem sessão de tenant válida, volta para o login
    /// (com aviso quando foi o token que venceu) e depois retorna à mesma tela.
    /// </summary>
    public class PaginaTenant : Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (!SessaoTenant.Ativa)
            {
                string login = "~/AcessoSeguro/Login.aspx?volta=" + HttpUtility.UrlEncode(Request.RawUrl);
                if (SessaoTenant.Expirou)
                {
                    SessaoTenant.Encerrar();
                    login += "&expirou=1";
                }
                // endResponse = true: sem isso o Page_Load da tela ainda rodaria (e chamaria a API sem token).
                Response.Redirect(login, true);
            }

            base.OnInit(e);
        }

        protected WheraApi Api
        {
            get { return new WheraApi(SessaoTenant.Token); }
        }

        private const string ChaveAviso = "WheraOwnerTenant.Aviso";

        /// <summary>Guarda um aviso para a próxima tela (o que é registrado antes de um Redirect se perde).</summary>
        protected void AvisarNaProximaTela(string mensagem)
        {
            Session[ChaveAviso] = mensagem;
        }

        /// <summary>Aviso deixado pela tela anterior (lido uma vez só), ou null.</summary>
        protected string RetirarAviso()
        {
            var mensagem = Session[ChaveAviso] as string;
            Session.Remove(ChaveAviso);
            return mensagem;
        }

        /// <summary>
        /// Token recusado pela API no meio do uso (401): encerra e manda para o login.
        /// </summary>
        protected void TratarSessaoRecusada(WheraApiException ex)
        {
            if (ex.StatusCode != 401) throw ex;

            SessaoTenant.Encerrar();
            Response.Redirect("~/AcessoSeguro/Login.aspx?expirou=1&volta=" + HttpUtility.UrlEncode(Request.RawUrl), true);
        }
    }
}
