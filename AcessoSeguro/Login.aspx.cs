using System;
using System.Web;
using System.Web.UI;
using WheraOwnerTenant;

public partial class AcessoSeguro_Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (SessaoTenant.Ativa)
        {
            IrParaDestino();
            return;
        }

        // A mensagem de erro vale só para a tentativa atual (o ViewState guardaria a visibilidade da anterior).
        pnlErro.Visible = false;

        if (!IsPostBack)
        {
            AutenticacaoTenant.DescartarPendente();
            pnlExpirou.Visible = Request.QueryString["expirou"] == "1";
            txtEmail.Focus();
        }
    }

    protected void btnContinuar_Click(object sender, EventArgs e)
    {
        string email = txtEmail.Text.Trim();
        string senha = txtSenha.Text;

        if (email.Length == 0 || senha.Length == 0)
        {
            MostrarErro("Informe e-mail e senha.");
            return;
        }

        LoginResposta resposta;
        try
        {
            resposta = AutenticacaoTenant.Enviar(email, senha, null, null);
        }
        catch (WheraApiException ex)
        {
            MostrarErro(AutenticacaoTenant.MensagemErro(ex));
            return;
        }

        if (resposta != null && (resposta.Etapa == AutenticacaoTenant.EtapaSegundaSenha || resposta.Etapa == AutenticacaoTenant.EtapaDefinirSegundaSenha))
        {
            AutenticacaoTenant.GuardarPendente(email, senha);
            if (resposta.Etapa == AutenticacaoTenant.EtapaDefinirSegundaSenha) MostrarDefinirSegundaSenha(); else MostrarSegundaEtapa(email);
            return;
        }

        Concluir(resposta);
    }

    protected void btnEntrar_Click(object sender, EventArgs e)
    {
        string email, senha;
        if (!AutenticacaoTenant.ObterPendente(out email, out senha))
        {
            MostrarPrimeiraEtapa();
            MostrarErro("O tempo para concluir o login acabou. Entre novamente.");
            return;
        }

        string segundaSenha = txtSegundaSenha.Text;
        if (segundaSenha.Length == 0)
        {
            MostrarSegundaEtapa(email);
            MostrarErro("Informe a senha adicional.");
            return;
        }

        LoginResposta resposta;
        try
        {
            resposta = AutenticacaoTenant.Enviar(email, senha, segundaSenha, null);
        }
        catch (WheraApiException ex)
        {
            // 403 = senha de administrador errada (ou bloqueada): continua na etapa 2.
            if (ex.StatusCode == 403) MostrarSegundaEtapa(email);
            else
            {
                AutenticacaoTenant.DescartarPendente();
                MostrarPrimeiraEtapa();
            }
            MostrarErro(AutenticacaoTenant.MensagemErro(ex));
            return;
        }

        Concluir(resposta);
    }

    protected void btnDefinir_Click(object sender, EventArgs e)
    {
        string email, senha;
        if (!AutenticacaoTenant.ObterPendente(out email, out senha)) { MostrarPrimeiraEtapa(); MostrarErro("O tempo para concluir o cadastro acabou. Entre novamente."); return; }
        if (string.IsNullOrWhiteSpace(txtNovaSegundaSenha.Text) || txtNovaSegundaSenha.Text != txtConfirmaSegundaSenha.Text) { MostrarDefinirSegundaSenha(); MostrarErro("As duas senhas precisam ser iguais."); return; }
        try { Concluir(AutenticacaoTenant.Enviar(email, senha, null, txtNovaSegundaSenha.Text)); }
        catch (WheraApiException ex) { MostrarDefinirSegundaSenha(); MostrarErro(AutenticacaoTenant.MensagemErro(ex)); }
    }

    protected void btnVoltar_Click(object sender, EventArgs e)
    {
        AutenticacaoTenant.DescartarPendente();
        MostrarPrimeiraEtapa();
    }

    private void Concluir(LoginResposta resposta)
    {
        AutenticacaoTenant.DescartarPendente();

        if (resposta == null || resposta.Etapa != AutenticacaoTenant.EtapaConcluido || resposta.Sessao == null
            || (resposta.Sessao.Papel != AutenticacaoTenant.PapelOwner && resposta.Sessao.Papel != AutenticacaoTenant.PapelTenant && resposta.Sessao.Papel != AutenticacaoTenant.PapelGerente))
        {
            MostrarPrimeiraEtapa();
            MostrarErro("Esta aplicação aceita apenas sessões Owner, Tenant ou Gerente autorizadas pela API Whera.");
            return;
        }

        AutenticacaoTenant.IniciarSessao(resposta.Sessao);
        IrParaDestino();
    }

    private void MostrarPrimeiraEtapa()
    {
        pnlCredenciais.Visible = true;
        pnlSegundaSenha.Visible = false;
        pnlDefinirSegundaSenha.Visible = false;
        txtEmail.Focus();
    }

    private void MostrarSegundaEtapa(string email)
    {
        litEmail.Text = HttpUtility.HtmlEncode(email);
        pnlCredenciais.Visible = false;
        pnlSegundaSenha.Visible = true;
        pnlDefinirSegundaSenha.Visible = false;
        txtSegundaSenha.Focus();
    }

    private void MostrarDefinirSegundaSenha()
    {
        pnlCredenciais.Visible = false;
        pnlSegundaSenha.Visible = false;
        pnlDefinirSegundaSenha.Visible = true;
        txtNovaSegundaSenha.Focus();
    }

    private void MostrarErro(string mensagem)
    {
        pnlExpirou.Visible = false;
        litErro.Text = HttpUtility.HtmlEncode(mensagem);
        pnlErro.Visible = true;
    }

    /// <summary>Volta para a tela pedida antes do login; só aceita caminho interno do próprio site.</summary>
    private void IrParaDestino()
    {
        string volta = Request.QueryString["volta"];
        bool interno = !string.IsNullOrEmpty(volta) && volta.StartsWith("/") && !volta.StartsWith("//")
            && !volta.StartsWith("/\\") && volta.IndexOf("/AcessoSeguro/", StringComparison.OrdinalIgnoreCase) < 0;

        // endResponse = true: quem já está logado não pode seguir para os cliques do formulário.
        Response.Redirect(interno ? volta : ResolveUrl("~/Adm/Home.aspx"), true);
    }
}
