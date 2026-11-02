using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WheraOwnerTenant;

/// <summary>Indicadores calculados só com os dados que a API guarda. Não há visualizações nem cliques: a API não os registra.</summary>
public partial class Adm_PublicacoesDashboard : PaginaTenant
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
            var publicadas = dados.Itens.Where(x => x.FoiPublicada).ToList();
            if (publicadas.Count == 0) { pnlVazio.Visible = true; return; }

            var ativas = publicadas.Where(x => x.Situacao == Publicacao.Ativa).OrderBy(x => x.FimUtc).ToList();
            litAtivas.Text = ativas.Count.ToString();
            litTerminam.Text = ativas.Count(x => x.FimUtc.HasValue && x.FimUtc.Value <= agora.AddDays(7)).ToString();
            litTrintaDias.Text = publicadas.Count(x => x.ReferenciaUtc.HasValue && x.ReferenciaUtc.Value <= agora && x.ReferenciaUtc.Value > agora.AddDays(-30)).ToString();
            litRelampago.Text = publicadas.Count(x => x.Relampago).ToString();
            litLegenda.Text = "<ul class=\"legenda\"><li><span class=\"marca-story\"></span>Stories</li><li><span class=\"marca-promocao\"></span>Promoções</li></ul>";
            litGrafico.Text = Grafico(publicadas);
            litProporcao.Text = Proporcao(publicadas);

            var proximas = ativas.Take(5).ToList();
            rptProximas.DataSource = proximas; rptProximas.DataBind(); rptProximas.Visible = proximas.Count > 0; pnlSemProximas.Visible = proximas.Count == 0;
            pnlConteudo.Visible = true;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            pnlErro.Visible = true;
            litErro.Text = HttpUtility.HtmlEncode(ex.StatusCode == 403 ? "Você não tem permissão para ver as publicações desta empresa." : ex.Message);
        }
    }

    /// <summary>Barras empilhadas dos últimos seis meses, pelo mês de início no horário de Brasília.</summary>
    private string Grafico(List<Publicacao> publicadas)
    {
        var hoje = PublicacoesTenant.HoraBrasil(agora);
        var primeiroMes = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-5);
        var meses = Enumerable.Range(0, 6).Select(i => primeiroMes.AddMonths(i)).Select(m =>
        {
            var doMes = publicadas.Where(x => x.ReferenciaUtc.HasValue && Mesmo(PublicacoesTenant.HoraBrasil(x.ReferenciaUtc.Value), m)).ToList();
            return new { Mes = m, Stories = doMes.Count(x => x.Tipo == "Story"), Promocoes = doMes.Count(x => x.Tipo == "Promocao") };
        }).ToList();
        var maximo = Math.Max(1, meses.Max(m => m.Stories + m.Promocoes));

        var html = new StringBuilder("<div class=\"grafico\">");
        foreach (var m in meses)
        {
            var total = m.Stories + m.Promocoes;
            var nome = PublicacoesTenant.Mes(m.Mes);
            var descricao = nome + ": " + m.Stories + (m.Stories == 1 ? " story, " : " stories, ") + m.Promocoes + (m.Promocoes == 1 ? " promoção" : " promoções");
            html.Append("<div class=\"coluna\" tabindex=\"0\" role=\"img\" aria-label=\"").Append(descricao).Append("\">")
                .Append("<span class=\"coluna-dica\">").Append(descricao).Append("</span>")
                .Append("<div class=\"coluna-area\"><span class=\"coluna-total\">").Append(total).Append("</span>");
            if (m.Promocoes > 0) html.Append("<span class=\"barra-promocao\" style=\"height:").Append(Pct(m.Promocoes, maximo)).Append("%\"></span>");
            if (m.Stories > 0) html.Append("<span class=\"barra-story\" style=\"height:").Append(Pct(m.Stories, maximo)).Append("%\"></span>");
            html.Append("</div><span class=\"coluna-mes\">").Append(nome).Append("</span></div>");
        }
        return html.Append("</div>").ToString();
    }

    private static bool Mesmo(DateTime data, DateTime mes) { return data.Year == mes.Year && data.Month == mes.Month; }

    /// <summary>Reserva 15% da altura para o total acima da barra.</summary>
    private static string Pct(int valor, int maximo) { return (valor * 85.0 / maximo).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture); }

    private static string Proporcao(List<Publicacao> publicadas)
    {
        int stories = publicadas.Count(x => x.Tipo == "Story"), promocoes = publicadas.Count - stories, total = publicadas.Count;
        var html = new StringBuilder("<p class=\"proporcao-total\"><strong>").Append(total).Append("</strong> ").Append(total == 1 ? "publicação" : "publicações").Append("</p><div class=\"proporcao\" role=\"img\" aria-label=\"")
            .Append(stories).Append(" stories e ").Append(promocoes).Append(" promoções\">");
        if (stories > 0) html.Append("<span class=\"barra-story\" style=\"flex:").Append(stories).Append("\"></span>");
        if (promocoes > 0) html.Append("<span class=\"barra-promocao\" style=\"flex:").Append(promocoes).Append("\"></span>");
        html.Append("</div><ul class=\"proporcao-itens\"><li><span class=\"marca-story\"></span>Stories<strong>").Append(stories).Append("</strong></li><li><span class=\"marca-promocao\"></span>Promoções<strong>").Append(promocoes).Append("</strong></li></ul>");
        return html.ToString();
    }

    protected string Linha(object item) { return PublicacoesTenant.LinhaHtml((Publicacao)item, agora); }
}
