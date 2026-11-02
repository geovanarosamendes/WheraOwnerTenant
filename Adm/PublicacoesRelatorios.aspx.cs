using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_PublicacoesRelatorios : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Atualizar(); }
    protected void btnAplicar_Click(object sender, EventArgs e) { Atualizar(); }

    protected void btnExportar_Click(object sender, EventArgs e)
    {
        var itens = Filtrar();
        if (itens == null) return;
        var csv = new StringBuilder("Título;Tipo;Relâmpago;Situação;Início;Término;Preço\r\n");
        foreach (var p in itens)
            csv.Append(string.Join(";", new[] { p.Titulo, p.TipoRotulo, p.Relampago ? "Sim" : "Não", p.Situacao, PublicacoesTenant.DataHora(p.InicioUtc), PublicacoesTenant.DataHora(p.FimUtc), p.Preco.HasValue ? p.Preco.Value.ToString("0.00", new CultureInfo("pt-BR")) : "" }.Select(Celula))).Append("\r\n");
        Response.Clear();
        Response.ContentType = "text/csv";
        Response.ContentEncoding = Encoding.UTF8;
        Response.AddHeader("Content-Disposition", "attachment; filename=publicacoes-" + PublicacoesTenant.HoraBrasil(DateTime.UtcNow).ToString("yyyy-MM-dd") + ".csv");
        Response.BinaryWrite(Encoding.UTF8.GetPreamble());
        Response.Write(csv.ToString());
        Response.End();
    }

    /// <summary>Aspas para o Excel e proteção contra fórmula: o título é escrito por quem publica.</summary>
    private static string Celula(string valor)
    {
        var texto = valor ?? "";
        if (texto.Length > 0 && "=+-@\t\r".IndexOf(texto[0]) >= 0) texto = "'" + texto;
        return "\"" + texto.Replace("\"", "\"\"") + "\"";
    }

    private void Atualizar()
    {
        var itens = Filtrar();
        if (itens == null) return;
        pnlConteudo.Visible = true;
        litTotal.Text = itens.Count + (itens.Count == 1 ? " publicação" : " publicações");
        rptItens.DataSource = itens; rptItens.DataBind(); rptItens.Visible = itens.Count > 0; pnlVazio.Visible = itens.Count == 0;
    }

    /// <summary>Devolve null quando há erro ou falta de empresa, já mostrando o aviso na tela.</summary>
    private List<Publicacao> Filtrar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; pnlConteudo.Visible = false; return null; }
            pnlConteudo.Visible = true;
            DateTime? de = Data(txtDe.Text), ate = Data(txtAte.Text);
            if (de.HasValue && ate.HasValue && de > ate) { Mostrar("A data inicial precisa ser anterior à data final."); return null; }
            IEnumerable<Publicacao> itens = PublicacoesTenant.Carregar(Api, empresa.Id).Itens;
            if (!string.IsNullOrEmpty(ddlTipo.SelectedValue)) itens = itens.Where(x => x.Tipo == ddlTipo.SelectedValue);
            if (!string.IsNullOrEmpty(ddlSituacao.SelectedValue)) itens = itens.Where(x => x.Situacao == ddlSituacao.SelectedValue);
            if (de.HasValue) itens = itens.Where(x => x.ReferenciaUtc.HasValue && PublicacoesTenant.HoraBrasil(x.ReferenciaUtc.Value).Date >= de.Value);
            if (ate.HasValue) itens = itens.Where(x => x.ReferenciaUtc.HasValue && PublicacoesTenant.HoraBrasil(x.ReferenciaUtc.Value).Date <= ate.Value);
            return itens.ToList();
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return null; }
            Mostrar(ex.StatusCode == 403 ? "Você não tem permissão para ver as publicações desta empresa." : ex.Message);
            return null;
        }
    }

    private static DateTime? Data(string valor)
    {
        DateTime data;
        return DateTime.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data) ? (DateTime?)data : null;
    }

    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
