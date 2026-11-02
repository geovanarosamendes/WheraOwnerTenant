using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

/// <summary>Consulta das promoções, comuns e relâmpago. Criar e encerrar é feito no aplicativo Elo.</summary>
public partial class Adm_Promocoes : PaginaTenant
{
    private DateTime agora;

    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            var dados = PublicacoesTenant.Carregar(Api, empresa.Id);
            agora = dados.AgoraUtc;
            var todas = dados.Itens.Where(x => x.Tipo == "Promocao").ToList();
            var comuns = todas.Where(x => !x.Relampago).ToList();
            var relampago = todas.Where(x => x.Relampago).ToList();

            litAtivas.Text = todas.Count(x => x.Situacao == Publicacao.Ativa).ToString();
            litProgramadas.Text = todas.Count(x => x.Situacao == Publicacao.Programada).ToString();
            litComuns.Text = comuns.Count(x => x.FoiPublicada).ToString();
            litRelampago.Text = relampago.Count(x => x.FoiPublicada).ToString();

            var tipo = Request.QueryString["tipo"];
            if (tipo != "comuns" && tipo != "relampago") tipo = "todas";
            litAbas.Text = Aba("todas", "Todas", todas.Count, tipo) + Aba("comuns", "Comuns", comuns.Count, tipo) + Aba("relampago", "Relâmpago", relampago.Count, tipo);
            var lista = tipo == "comuns" ? comuns : tipo == "relampago" ? relampago : todas;

            Preencher(pnlAtivas, rptAtivas, lista.Where(x => x.Situacao == Publicacao.Ativa).OrderBy(x => x.FimUtc));
            Preencher(pnlProgramadas, rptProgramadas, lista.Where(x => x.Situacao == Publicacao.Programada).OrderBy(x => x.InicioUtc));
            Preencher(pnlRascunhos, rptRascunhos, lista.Where(x => x.Situacao == Publicacao.Rascunho));
            Preencher(pnlPassadas, rptPassadas, lista.Where(x => x.Situacao == Publicacao.Encerrada || x.Situacao == Publicacao.Suspensa));
            pnlVazio.Visible = lista.Count == 0;
            litVazioTitulo.Text = tipo == "comuns" ? "Nenhuma promoção comum ainda" : tipo == "relampago" ? "Nenhuma promoção relâmpago ainda" : "Nenhuma promoção ainda";
            pnlConteudo.Visible = true;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            pnlErro.Visible = true;
            litErro.Text = HttpUtility.HtmlEncode(ex.StatusCode == 403 ? "Você não tem permissão para ver as promoções desta empresa." : ex.Message);
        }
    }

    private static void Preencher(Panel painel, Repeater lista, IEnumerable<Publicacao> itens)
    {
        var dados = itens.ToList();
        lista.DataSource = dados;
        lista.DataBind();
        painel.Visible = dados.Count > 0;
    }

    private string Aba(string valor, string rotulo, int total, string atual)
    {
        return "<a class=\"aba" + (valor == atual ? " ativa\" aria-current=\"page" : "") + "\" href=\"" + ResolveUrl("~/Adm/Promocoes.aspx" + (valor == "todas" ? "" : "?tipo=" + valor)) + "\">" + rotulo + " <span>" + total + "</span></a>";
    }

    protected string Linha(object item) { return PublicacoesTenant.LinhaHtml((Publicacao)item, agora); }
}
