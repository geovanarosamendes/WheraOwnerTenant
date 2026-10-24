using System;
using System.Web;
using WheraOwnerTenant;

public partial class AcessoSeguro_CadastroInicial : System.Web.UI.Page
{
    private const string ChaveEmpresaId = "WheraOwnerTenant.PreCadastro.EmpresaId";
    private const string ChaveCodigo = "WheraOwnerTenant.PreCadastro.Codigo";

    protected void btnCriar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNomeEmpresa.Text) || string.IsNullOrWhiteSpace(txtCnpj.Text) || string.IsNullOrWhiteSpace(txtNomeContato.Text) || string.IsNullOrWhiteSpace(txtEmailCriar.Text))
        { MostrarErro("Informe nome, CNPJ, contato e e-mail para solicitar o pré-cadastro."); return; }
        try
        {
            var resposta = TenantApi.CriarPreCadastro(new WheraApi(null), new { NomeFantasia = txtNomeEmpresa.Text.Trim(), Email = txtEmailCriar.Text.Trim(), Cnpj = txtCnpj.Text.Trim(), Slug = (string)null, Modalidade = ddlModalidade.SelectedValue, NomeContato = txtNomeContato.Text.Trim() });
            ViewState[ChaveEmpresaId] = resposta.EmpresaId; ViewState[ChaveCodigo] = resposta.CodigoContinuacao;
            txtEmpresaId.Text = resposta.EmpresaId; txtCodigo.Text = resposta.CodigoContinuacao; litSucesso.Text = "Pré-cadastro criado. Guarde o Empresa ID e o código para continuar."; pnlSucesso.Visible = true;
        }
        catch (WheraApiException ex) { MostrarErro(MensagemPreCadastro(ex)); }
    }

    protected void btnContinuar_Click(object sender, EventArgs e)
    {
        Guid empresaId;
        if (!Guid.TryParse(txtEmpresaId.Text.Trim(), out empresaId) || string.IsNullOrWhiteSpace(txtCodigo.Text)) { MostrarErro("Informe um Empresa ID válido e o código de continuação."); return; }
        try
        {
            var validacao = TenantApi.ValidarPreCadastro(new WheraApi(null), txtEmpresaId.Text.Trim(), txtCodigo.Text.Trim());
            ViewState[ChaveEmpresaId] = empresaId.ToString(); ViewState[ChaveCodigo] = txtCodigo.Text.Trim(); ViewState["WheraOwnerTenant.PreCadastro.Validacao"] = validacao.IdentificadorTemporario;
            litEmpresaId.Text = HttpUtility.HtmlEncode(txtEmpresaId.Text.Trim()); pnlCriar.Visible = false; pnlVincular.Visible = true;
        }
        catch (WheraApiException ex) { MostrarErro(MensagemPreCadastro(ex)); }
    }

    protected void btnVincular_Click(object sender, EventArgs e)
    {
        var empresaId = Convert.ToString(ViewState[ChaveEmpresaId]); var codigo = Convert.ToString(ViewState[ChaveCodigo]);
        Guid id;
        if (!Guid.TryParse(empresaId, out id) || string.IsNullOrWhiteSpace(codigo)) { MostrarErro("O código de continuação não está disponível. Comece novamente."); return; }
        if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || txtSenha.Text.Length < 8 || txtSegundaSenha.Text.Length < 8)
        { MostrarErro("Informe nome, e-mail, senha comum e PIN com pelo menos 8 caracteres."); return; }
        try
        {
            var resposta = TenantApi.VincularPreCadastro(new WheraApi(null), empresaId, codigo, new { Email = txtEmail.Text.Trim(), Nome = txtNome.Text.Trim(), Senha = txtSenha.Text, SegundaSenha = txtSegundaSenha.Text, SegundaSenhaAtual = string.IsNullOrWhiteSpace(txtSegundaSenhaAtual.Text) ? null : txtSegundaSenhaAtual.Text, IdentificadorTemporario = Convert.ToString(ViewState["WheraOwnerTenant.PreCadastro.Validacao"]) });
            if (resposta == null || resposta.Sessao == null || !string.Equals(resposta.Sessao.Papel, AutenticacaoTenant.PapelTenant, StringComparison.OrdinalIgnoreCase)) { MostrarErro("A API não devolveu uma sessão Tenant válida."); return; }
            AutenticacaoTenant.IniciarSessao(resposta.Sessao);
            ViewState.Remove(ChaveEmpresaId); ViewState.Remove(ChaveCodigo); ViewState.Remove("WheraOwnerTenant.PreCadastro.Validacao");
            Response.Redirect("~/Adm/Home.aspx", true);
        }
        catch (WheraApiException ex) { MostrarErro(MensagemPreCadastro(ex)); }
    }

    private void MostrarErro(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }

    private static string MensagemPreCadastro(WheraApiException ex)
    {
        if (ex.StatusCode == 401) return "Código inválido, expirado ou já utilizado. Confirme o Empresa ID e valide o código novamente.";
        if (ex.StatusCode == 403) return "O pré-cadastro ainda não foi aprovado para configuração ou esta conta não pode assumir a empresa.";
        if (ex.StatusCode == 409) return "A conta ou o pré-cadastro já possui um vínculo incompatível.";
        return ex.Message;
    }
}
