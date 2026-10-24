using System;
using System.Web;
using WheraOwnerTenant;

public partial class AcessoSeguro_CadastroInicial : System.Web.UI.Page
{
    private const string ChaveEmpresaId = "WheraOwnerTenant.PreCadastro.EmpresaId";
    private const string ChaveCodigo = "WheraOwnerTenant.PreCadastro.Codigo";

    protected void btnCriar_Click(object sender, EventArgs e)
    {
        try
        {
            var resposta = TenantApi.CriarPreCadastro(new WheraApi(null), new { NomeFantasia = txtNomeEmpresa.Text.Trim(), Email = txtEmailCriar.Text.Trim(), Cnpj = txtCnpj.Text.Trim(), Slug = (string)null, Modalidade = ddlModalidade.SelectedValue, NomeContato = txtNomeContato.Text.Trim() });
            ViewState[ChaveEmpresaId] = resposta.EmpresaId; ViewState[ChaveCodigo] = resposta.CodigoContinuacao;
            txtEmpresaId.Text = resposta.EmpresaId; txtCodigo.Text = resposta.CodigoContinuacao; litSucesso.Text = "Pré-cadastro criado. Guarde o Empresa ID e o código para continuar."; pnlSucesso.Visible = true;
        }
        catch (WheraApiException ex) { MostrarErro(ex.Message); }
    }

    protected void btnContinuar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtEmpresaId.Text) || string.IsNullOrWhiteSpace(txtCodigo.Text)) { MostrarErro("Informe o Empresa ID e o código de continuação."); return; }
        ViewState[ChaveEmpresaId] = txtEmpresaId.Text.Trim(); ViewState[ChaveCodigo] = txtCodigo.Text.Trim(); litEmpresaId.Text = HttpUtility.HtmlEncode(txtEmpresaId.Text.Trim()); pnlCriar.Visible = false; pnlVincular.Visible = true;
    }

    protected void btnVincular_Click(object sender, EventArgs e)
    {
        var empresaId = Convert.ToString(ViewState[ChaveEmpresaId]); var codigo = Convert.ToString(ViewState[ChaveCodigo]);
        if (string.IsNullOrWhiteSpace(empresaId) || string.IsNullOrWhiteSpace(codigo)) { MostrarErro("O código de continuação não está disponível. Comece novamente."); return; }
        try
        {
            var resposta = TenantApi.VincularPreCadastro(new WheraApi(null), empresaId, codigo, new { Email = txtEmail.Text.Trim(), Nome = txtNome.Text.Trim(), Senha = txtSenha.Text, SegundaSenha = txtSegundaSenha.Text, SegundaSenhaAtual = (string)null });
            AutenticacaoTenant.IniciarSessao(resposta.Sessao);
            Response.Redirect("~/Adm/Home.aspx", true);
        }
        catch (WheraApiException ex) { MostrarErro(ex.Message); }
    }

    private void MostrarErro(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
