using System;
using System.Linq;
using System.Web;
using WheraOwnerTenant;

/// <summary>Totais por cupom: a API ainda não informa cada resgate individualmente.</summary>
public partial class Adm_CuponsResgatados : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            var agora = DateTime.UtcNow;
            var resgatados = TenantApi.Cupons(Api, empresa.Id).Where(c => c.TotalResgates > 0).ToList();
            litTotal.Text = resgatados.Sum(c => c.TotalResgates).ToString();
            litComuns.Text = resgatados.Where(c => c.Publico != CuponsTenant.Vip).Sum(c => c.TotalResgates).ToString();
            litVip.Text = resgatados.Where(c => c.Publico == CuponsTenant.Vip).Sum(c => c.TotalResgates).ToString();
            litCupons.Text = resgatados.Count.ToString();
            rptCupons.DataSource = resgatados.OrderByDescending(c => c.TotalResgates).Select(c => new
            {
                Desconto = CuponsTenant.Desconto(c),
                c.Titulo,
                Publico = CuponsTenant.Publico(c),
                Resgates = c.TotalResgates,
                Vagas = c.LimiteResgates.HasValue ? Math.Max(0, c.LimiteResgates.Value - c.TotalResgates).ToString() : "Sem limite",
                Selo = CuponsTenant.Selo(CuponsTenant.Situacao(c, agora))
            }).ToList();
            rptCupons.DataBind();
            rptCupons.Visible = resgatados.Count > 0;
            pnlVazio.Visible = resgatados.Count == 0;
            pnlConteudo.Visible = true;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            pnlErro.Visible = true;
            litErro.Text = HttpUtility.HtmlEncode(ex.StatusCode == 403 ? "Você não tem permissão para ver os cupons desta empresa." : ex.Message);
        }
    }
}
