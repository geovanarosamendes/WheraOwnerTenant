using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

public partial class Adm_Home : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var aviso = SessaoTenant.RetirarAviso();
            if (!string.IsNullOrWhiteSpace(aviso)) { litAviso.Text = HttpUtility.HtmlEncode(aviso); pnlAviso.Visible = true; }
            pnlSelecaoOwner.Visible = SessaoTenant.EhOwner;
            if (SessaoTenant.EhOwner) PesquisarEmpresas("");
            Carregar();
        }
    }

    protected void btnAtualizar_Click(object sender, EventArgs e) { Carregar(); }

    protected void btnBuscarEmpresa_Click(object sender, EventArgs e) { PesquisarEmpresas(txtBuscaEmpresa.Text.Trim()); }

    protected void btnSelecionarEmpresa_Click(object sender, EventArgs e)
    {
        if (!SessaoTenant.EhOwner || string.IsNullOrWhiteSpace(ddlEmpresas.SelectedValue)) { MostrarErro("Selecione uma empresa autorizada pela API."); return; }
        try
        {
            TenantApi.PerfilEmpresa(Api, ddlEmpresas.SelectedValue);
            SessaoTenant.SelecionarEmpresa(ddlEmpresas.SelectedValue);
            Carregar();
            litOwnerMensagem.Text = "Empresa selecionada.";
        }
        catch (WheraApiException ex) { TratarErro(ex); }
    }

    private void PesquisarEmpresas(string busca)
    {
        try
        {
            var resultado = TenantApi.ListarEmpresas(Api, busca);
            ddlEmpresas.Items.Clear();
            foreach (var empresa in resultado == null || resultado.Itens == null ? new System.Collections.Generic.List<EmpresaResumoTenant>() : resultado.Itens)
                ddlEmpresas.Items.Add(new ListItem(empresa.NomeFantasia + " — " + StatusEmpresa.Rotulo(empresa.Status), empresa.Id));
            litOwnerMensagem.Text = ddlEmpresas.Items.Count == 0 ? "Nenhuma empresa encontrada." : ddlEmpresas.Items.Count + " empresa(s) encontrada(s).";
        }
        catch (WheraApiException ex) { TratarErro(ex); }
    }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            pnlDashboard.Visible = true;
            litEmpresa.Text = HttpUtility.HtmlEncode(empresa.NomeFantasia);
            litCidade.Text = HttpUtility.HtmlEncode(empresa.Cidade ?? "Localidade não informada");
            litStatus.Text = HttpUtility.HtmlEncode(StatusEmpresa.Rotulo(empresa.Status));
            int conteudos = 0;
            try { conteudos = (TenantApi.ListarConteudos(Api, empresa.Id, "Story").Itens ?? new System.Collections.Generic.List<ConteudoTenant>()).Count + (TenantApi.ListarConteudos(Api, empresa.Id, "Promocao").Itens ?? new System.Collections.Generic.List<ConteudoTenant>()).Count; } catch (WheraApiException) { }
            litConteudos.Text = conteudos.ToString();
        }
        catch (WheraApiException ex) { TratarErro(ex); }
    }

    private void TratarErro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } MostrarErro(ex.Message); }
    private void MostrarErro(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
