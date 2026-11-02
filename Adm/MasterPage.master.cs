using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class Adm_MasterPage : MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        litUsuario.Text = HttpUtility.HtmlEncode(WheraOwnerTenant.SessaoTenant.Nome ?? WheraOwnerTenant.SessaoTenant.Email ?? "")
            + (WheraOwnerTenant.SessaoTenant.DevPreviewAtiva ? " (prévia local)" : "");
    }

    protected static readonly string[] TelasProdutos = { "ProdutosGaleria.aspx", "Produtos.aspx", "ProdutosImportar.aspx", "Catalogo.aspx", "VitrineElo.aspx" };

    protected static readonly string[] TelasCupons = { "Cupons.aspx", "CuponsResgatados.aspx", "CuponsCriar.aspx", "CuponsValidar.aspx" };
    protected static readonly string[] TelasPublicacoes = { "PublicacoesDashboard.aspx", "Conteudos.aspx", "PublicacoesRelatorios.aspx" };

    protected string Ativo(params string[] telas)
    {
        return EhTelaAtual(telas) ? "ativo" : "";
    }

    protected string Atual(params string[] telas)
    {
        return EhTelaAtual(telas) ? "aria-current=\"page\"" : "";
    }

    private bool EhTelaAtual(string[] telas)
    {
        string atual = Path.GetFileName(Request.Path);
        return telas.Any(t => string.Equals(t, atual, StringComparison.OrdinalIgnoreCase));
    }
}
