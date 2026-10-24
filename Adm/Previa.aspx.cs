using System;
using System.Linq;
using System.Text;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_Previa : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            litStatus.Text = HttpUtility.HtmlEncode(StatusEmpresa.Rotulo(empresa.Status));
            litNome.Text = HttpUtility.HtmlEncode(empresa.NomeFantasia);
            litCategoria.Text = HttpUtility.HtmlEncode(empresa.Categoria == null ? "" : empresa.Categoria.Nome);
            var cidade = empresa.Endereco == null ? "" : empresa.Endereco.Cidade;
            litCidade.Text = string.IsNullOrWhiteSpace(cidade) ? "" : " · " + HttpUtility.HtmlEncode(cidade);
            litDescricao.Text = HttpUtility.HtmlEncode(empresa.Descricao ?? "Descrição ainda não informada.");
            var logo = empresa.Imagens == null ? null : empresa.Imagens.Logo;
            imgLogo.Visible = !string.IsNullOrWhiteSpace(logo);
            imgLogo.ImageUrl = logo ?? "";
            litCanais.Text = Canais(empresa);
            pnlPrevia.Visible = true;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(ex.StatusCode == 403 ? "Você não tem permissão para visualizar esta empresa." : ex.Message);
        }
    }

    private static string Canais(PerfilEmpresa empresa)
    {
        if (empresa.Canais == null || empresa.Canais.Count == 0) return "Nenhum canal configurado.";
        var html = new StringBuilder();
        foreach (var canal in empresa.Canais.Where(x => !string.IsNullOrWhiteSpace(x.Valor) || !string.IsNullOrWhiteSpace(x.Link)))
        {
            var valor = canal.Valor ?? canal.Link;
            html.Append("<span class=\"canal\">").Append(HttpUtility.HtmlEncode(canal.Rotulo ?? canal.Tipo)).Append(": ").Append(HttpUtility.HtmlEncode(valor)).Append("</span>");
        }
        return html.Length == 0 ? "Nenhum canal configurado." : html.ToString();
    }
}
