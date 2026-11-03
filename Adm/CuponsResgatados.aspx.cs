using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

/// <summary>Totais por cupom e a lista de cada resgate, com filtros e paginação.</summary>
public partial class Adm_CuponsResgatados : PaginaTenant
{
    private const int TamanhoPagina = 20;

    private string EmpresaId
    {
        get { return ViewState["EmpresaId"] as string; }
        set { ViewState["EmpresaId"] = value; }
    }

    private int PaginaAtual
    {
        get { return ViewState["Pagina"] is int ? (int)ViewState["Pagina"] : 1; }
        set { ViewState["Pagina"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Executar(Carregar); }

    protected void Filtro_Changed(object sender, EventArgs e)
    {
        PaginaAtual = 1;
        Executar(CarregarResgates);
    }

    protected void Pagina_Command(object sender, CommandEventArgs e)
    {
        PaginaAtual = Math.Max(1, PaginaAtual + (e.CommandName == "Proxima" ? 1 : -1));
        Executar(CarregarResgates);
    }

    private void Executar(Action acao)
    {
        try
        {
            acao();
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            pnlConteudo.Visible = false;
            pnlErro.Visible = true;
            litErro.Text = HttpUtility.HtmlEncode(ex.StatusCode == 403 ? "Você não tem permissão para ver os cupons desta empresa." : ex.Message);
        }
    }

    private void Carregar()
    {
        var empresa = TenantApi.MinhaEmpresa(Api);
        if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
        EmpresaId = empresa.Id;

        var agora = DateTime.UtcNow;
        var cupons = TenantApi.Cupons(Api, empresa.Id);
        var resgatados = cupons.Where(c => c.TotalResgates > 0).ToList();
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

        // Inclui cupons zerados: um resgate vencido deixa de contar nos totais, mas continua na lista.
        ddlCupom.Items.Clear();
        ddlCupom.Items.Add(new ListItem("Todos os cupons", ""));
        foreach (var cupom in cupons.OrderBy(c => c.Titulo)) ddlCupom.Items.Add(new ListItem(cupom.Titulo, cupom.Id));

        CarregarResgates();
        pnlConteudo.Visible = true;
    }

    private void CarregarResgates()
    {
        var pagina = Buscar(PaginaAtual);
        // A lista pode encolher entre um clique e outro: volta para a última página que ainda existe.
        if (pagina.Itens.Count == 0 && pagina.TotalPaginas > 0 && PaginaAtual > pagina.TotalPaginas)
        {
            PaginaAtual = pagina.TotalPaginas;
            pagina = Buscar(PaginaAtual);
        }

        rptResgates.DataSource = pagina.Itens.Select(r => new
        {
            r.NomeCliente,
            r.CupomTitulo,
            Desconto = CuponsTenant.Desconto(r.TipoDesconto, r.Valor),
            r.Codigo,
            ResgatadoEm = PublicacoesTenant.DataHora(PublicacoesTenant.ParaUtc(r.ResgatadoEm)),
            ValeAte = PublicacoesTenant.DataHora(PublicacoesTenant.ParaUtc(r.ExpiraEm)),
            UtilizadoEm = PublicacoesTenant.DataHora(PublicacoesTenant.ParaUtc(r.UtilizadoEm)),
            Selo = CuponsTenant.SeloResgate(r.Situacao)
        }).ToList();
        rptResgates.DataBind();

        var filtrando = ddlSituacao.SelectedValue.Length > 0 || ddlCupom.SelectedValue.Length > 0;
        rptResgates.Visible = pagina.TotalItens > 0;
        pnlSemResgates.Visible = pagina.TotalItens == 0;
        litSemResgates.Text = filtrando ? "Nenhum resgate com estes filtros." : "Quando um cliente resgatar um cupom, ele aparece aqui.";
        pnlResgates.Visible = pagina.TotalItens > 0 || filtrando;

        pnlPaginas.Visible = pagina.TotalPaginas > 1;
        litPagina.Text = "Página " + PaginaAtual + " de " + pagina.TotalPaginas;
        btnAnterior.Enabled = PaginaAtual > 1;
        btnProxima.Enabled = PaginaAtual < pagina.TotalPaginas;
        pnlVazio.Visible = !pnlResgates.Visible && !rptCupons.Visible;
    }

    private PaginaResgatesCupom Buscar(int pagina)
    {
        return TenantApi.ResgatesCupons(Api, EmpresaId, ddlCupom.SelectedValue, ddlSituacao.SelectedValue, pagina, TamanhoPagina);
    }
}
