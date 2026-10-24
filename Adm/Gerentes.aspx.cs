using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

public partial class Adm_Gerentes : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.Gerentes.Empresa";

    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private void Carregar()
    {
        if (!SessaoTenant.EhTenant) { MostrarTexto("A administração de gerentes é exclusiva do proprietário Tenant."); return; }
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            Session[ChaveEmpresa] = empresa;
            litEmpresa.Text = HttpUtility.HtmlEncode(empresa.NomeFantasia);
            litProprietario.Text = HttpUtility.HtmlEncode(empresa.Proprietario == null ? "Conta proprietária" : empresa.Proprietario.Email);
            pnlConteudo.Visible = true;
            Bind(TenantApi.Gerentes(Api, empresa.Id));
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    private void Bind(List<GerenteTenant> gerentes)
    {
        rptGerentes.DataSource = gerentes; rptGerentes.DataBind(); pnlVazio.Visible = gerentes.Count == 0;
    }

    protected void btnAdicionar_Click(object sender, EventArgs e)
    {
        var empresa = Session[ChaveEmpresa] as PerfilEmpresa;
        if (empresa == null) { Carregar(); return; }
        if (string.IsNullOrWhiteSpace(txtEmail.Text)) { MostrarTexto("Informe o e-mail do gerente."); return; }
        try { TenantApi.AdicionarGerente(Api, empresa.Id, txtEmail.Text.Trim()); txtEmail.Text = ""; litSucesso.Text = "Gerente adicionado."; pnlSucesso.Visible = true; Bind(TenantApi.Gerentes(Api, empresa.Id)); }
        catch (WheraApiException ex) { Erro(ex); }
    }

    protected void btnRemover_Command(object sender, CommandEventArgs e)
    {
        var empresa = Session[ChaveEmpresa] as PerfilEmpresa;
        if (empresa == null) { Carregar(); return; }
        try { TenantApi.RemoverGerente(Api, empresa.Id, Convert.ToString(e.CommandArgument)); litSucesso.Text = "Gerente removido."; pnlSucesso.Visible = true; Bind(TenantApi.Gerentes(Api, empresa.Id)); }
        catch (WheraApiException ex) { Erro(ex); }
    }

    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } MostrarTexto(ex.StatusCode == 403 ? "Apenas o proprietário pode administrar gerentes." : ex.Message); }
    private void MostrarTexto(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
