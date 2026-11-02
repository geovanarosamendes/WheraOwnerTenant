using System;
using System.Linq;
using System.Web;
using WheraOwnerTenant;

/// <summary>Lista dos cupons, comuns e VIP. Encerrar e validar é feito no aplicativo Elo.</summary>
public partial class Adm_Cupons : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private void Carregar()
    {
        var aviso = RetirarAviso();
        if (!string.IsNullOrWhiteSpace(aviso)) { pnlSucesso.Visible = true; litSucesso.Text = HttpUtility.HtmlEncode(aviso); }
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            var cupons = TenantApi.Cupons(Api, empresa.Id);
            var agora = DateTime.UtcNow;
            var comuns = cupons.Where(c => c.Publico != CuponsTenant.Vip).ToList();
            var vips = cupons.Where(c => c.Publico == CuponsTenant.Vip).ToList();

            litDisponiveis.Text = cupons.Count(c => CuponsTenant.Situacao(c, agora) == CuponsTenant.Disponivel).ToString();
            litComuns.Text = comuns.Count.ToString();
            litVip.Text = vips.Count.ToString();
            litResgates.Text = cupons.Sum(c => c.TotalResgates).ToString();

            var tipo = Request.QueryString["tipo"];
            if (tipo != "comuns" && tipo != "vip") tipo = "todos";
            litAbas.Text = Aba("todos", "Todos", cupons.Count, tipo) + Aba("comuns", "Comuns", comuns.Count, tipo) + Aba("vip", "VIP", vips.Count, tipo);
            var lista = tipo == "comuns" ? comuns : tipo == "vip" ? vips : cupons;

            rptCupons.DataSource = lista.OrderBy(c => CuponsTenant.Ordem(CuponsTenant.Situacao(c, agora))).ThenBy(c => PublicacoesTenant.ParaUtc(c.FimEm)).Select(c => new
            {
                Desconto = CuponsTenant.Desconto(c),
                c.Titulo,
                Publico = CuponsTenant.Publico(c),
                Resgates = c.LimiteResgates.HasValue ? c.TotalResgates + " de " + c.LimiteResgates.Value : c.TotalResgates.ToString(),
                Validade = CuponsTenant.Validade(c),
                Prazo = CuponsTenant.PrazoUso(c),
                Selo = CuponsTenant.Selo(CuponsTenant.Situacao(c, agora))
            }).ToList();
            rptCupons.DataBind();
            rptCupons.Visible = lista.Count > 0;
            pnlVazio.Visible = lista.Count == 0;
            litVazioTitulo.Text = tipo == "comuns" ? "Nenhum cupom comum ainda" : tipo == "vip" ? "Nenhum cupom VIP ainda" : "Nenhum cupom ainda";
            pnlConteudo.Visible = true;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            pnlErro.Visible = true;
            litErro.Text = HttpUtility.HtmlEncode(ex.StatusCode == 403 ? "Você não tem permissão para ver os cupons desta empresa." : ex.Message);
        }
    }

    private string Aba(string valor, string rotulo, int total, string atual)
    {
        return "<a class=\"aba" + (valor == atual ? " ativa\" aria-current=\"page" : "") + "\" href=\"" + ResolveUrl("~/Adm/Cupons.aspx" + (valor == "todos" ? "" : "?tipo=" + valor)) + "\">" + rotulo + " <span>" + total + "</span></a>";
    }
}
