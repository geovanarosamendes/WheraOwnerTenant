using System;
using System.Linq;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_Home : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) Carregar();
    }

    protected void btnAtualizar_Click(object sender, EventArgs e) { Carregar(); }

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
        catch (WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(ex.Message); }
    }
}
