using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

/// <summary>
/// No sistema: "no catálogo" é Publicado; "inativo" é Disponivel = false. Excluir usa a exclusão lógica da API,
/// que não devolve mais o produto. Inativos só aparecem nos filtros Inativos e Todos.
/// </summary>
public partial class Adm_ProdutosGaleria : PaginaTenant
{
    private const string FiltroCatalogo = "catalogo", FiltroFora = "fora", FiltroInativos = "inativos", FiltroTodos = "todos";
    private string empresaId;

    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { Mostrar("Nenhuma empresa vinculada."); return; }
            empresaId = empresa.Id;
            Listar(TenantApi.Produtos(Api, empresaId));
            pnlConteudo.Visible = true;
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    private static bool NoCatalogo(ProdutoTenant p) { return p.Disponivel && p.Publicado; }
    private static bool Fora(ProdutoTenant p) { return p.Disponivel && !p.Publicado; }

    private void Listar(List<ProdutoTenant> produtos)
    {
        var filtro = Request.QueryString["filtro"];
        if (filtro != FiltroCatalogo && filtro != FiltroFora && filtro != FiltroInativos && filtro != FiltroTodos)
            filtro = produtos.Any(NoCatalogo) || !produtos.Any(Fora) ? FiltroCatalogo : FiltroFora;

        var abas = new StringBuilder();
        abas.Append(Aba(FiltroCatalogo, "No catálogo", produtos.Count(NoCatalogo), filtro));
        abas.Append(Aba(FiltroFora, "Fora do catálogo", produtos.Count(Fora), filtro));
        abas.Append(Aba(FiltroInativos, "Inativos", produtos.Count(p => !p.Disponivel), filtro));
        abas.Append(Aba(FiltroTodos, "Todos", produtos.Count, filtro));
        litAbas.Text = abas.ToString();

        var visiveis = filtro == FiltroCatalogo ? produtos.Where(NoCatalogo)
            : filtro == FiltroFora ? produtos.Where(Fora)
            : filtro == FiltroInativos ? produtos.Where(p => !p.Disponivel)
            : produtos;
        var lista = visiveis.ToList();
        rptProdutos.DataSource = lista; rptProdutos.DataBind(); rptProdutos.Visible = lista.Count > 0;
        pnlVazio.Visible = lista.Count == 0;
        litVazio.Text = produtos.Count == 0 ? "<h2>Nenhum produto ainda</h2><p>Cadastre um produto ou importe uma planilha para começar.</p>"
            : filtro == FiltroCatalogo ? "<p>Nenhum produto no catálogo. Abra Fora do catálogo, selecione produtos e adicione.</p>"
            : "<p>Nenhum produto neste filtro.</p>";
    }

    private string Aba(string valor, string rotulo, int total, string atual)
    {
        var ativa = valor == atual;
        return "<a class=\"aba" + (ativa ? " ativa" : "") + "\"" + (ativa ? " aria-current=\"page\"" : "") + " href=\"" + ResolveUrl("~/Adm/ProdutosGaleria.aspx?filtro=" + valor) + "\">" + rotulo + " <span>" + total.ToString(CultureInfo.InvariantCulture) + "</span></a>";
    }

    protected void btnAdicionar_Click(object sender, EventArgs e) { Aplicar("adicionado", "adicionados ao catálogo", Fora, p => Salvar(p, true, true)); }
    protected void btnRemover_Click(object sender, EventArgs e) { Aplicar("removido", "removidos do catálogo", NoCatalogo, p => Salvar(p, true, false)); }
    protected void btnInativar_Click(object sender, EventArgs e) { Aplicar("inativado", "inativados", p => p.Disponivel, p => Salvar(p, false, false)); }
    protected void btnReativar_Click(object sender, EventArgs e) { Aplicar("reativado", "reativados", p => !p.Disponivel, p => Salvar(p, true, false)); }
    protected void btnExcluir_Click(object sender, EventArgs e) { Aplicar("excluído", "excluídos", p => true, p => TenantApi.ExcluirProduto(Api, empresaId, p.Id)); }

    private void Salvar(ProdutoTenant p, bool disponivel, bool noCatalogo)
    {
        TenantApi.AtualizarProduto(Api, empresaId, p.Id, new { Nome = p.Nome, Descricao = p.Descricao, Preco = p.Preco, Disponivel = disponivel, Publicado = noCatalogo });
    }

    /// <summary>Confere a seleção contra a lista atual da API e relata o resultado real de cada produto.</summary>
    private void Aplicar(string singular, string plural, Func<ProdutoTenant, bool> seAplica, Action<ProdutoTenant> acao)
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { Mostrar("Nenhuma empresa vinculada."); return; }
            empresaId = empresa.Id;
            pnlConteudo.Visible = true;
            var produtos = TenantApi.Produtos(Api, empresaId);
            var selecionados = new HashSet<string>(rptProdutos.Items.Cast<RepeaterItem>()
                .Where(i => ((CheckBox)i.FindControl("chkSelecionado")).Checked)
                .Select(i => ((HiddenField)i.FindControl("hidId")).Value));
            if (selecionados.Count == 0) { Mostrar("Selecione ao menos um produto."); Listar(produtos); return; }

            int feitos = 0, falhas = 0;
            string ultimaFalha = null;
            foreach (var produto in produtos.Where(p => selecionados.Contains(p.Id)).Where(seAplica))
            {
                try { acao(produto); feitos++; }
                catch (WheraApiException ex)
                {
                    if (ex.StatusCode == 401) throw;
                    falhas++; ultimaFalha = ex.Message;
                }
            }
            var ignorados = selecionados.Count - feitos - falhas;
            if (feitos > 0)
            {
                pnlSucesso.Visible = true;
                litSucesso.Text = HttpUtility.HtmlEncode((feitos == 1 ? "1 produto " + singular : feitos + " produtos " + plural) + "." + (ignorados > 0 ? " " + ignorados + " não precisava(m) desta ação." : ""));
            }
            else if (falhas == 0) Mostrar("Esta ação não se aplica aos produtos selecionados.");
            if (falhas > 0) Mostrar(falhas + " produto(s) não foram alterados. " + ultimaFalha);
            Listar(TenantApi.Produtos(Api, empresaId));
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    protected string TenantApi_Foto(object item)
    {
        var url = TenantApi.FotoPrincipal((ProdutoTenant)item);
        return url == null ? "<span class=\"produto-sem-foto\">Sem foto</span>" : "<img loading=\"lazy\" alt=\"\" src=\"" + HttpUtility.HtmlAttributeEncode(url) + "\" />";
    }

    protected string Selo(object item)
    {
        var p = (ProdutoTenant)item;
        return !p.Disponivel ? "<span class=\"selo selo-neutro\">Inativo</span>"
            : p.Publicado ? "<span class=\"selo selo-sucesso\">No catálogo</span>"
            : "<span class=\"selo selo-aviso\">Fora do catálogo</span>";
    }

    protected string Preco(object item) { var p = (ProdutoTenant)item; return p.Preco.HasValue ? p.Preco.Value.ToString("C", new CultureInfo("pt-BR")) : "Sem preço"; }

    private void Erro(WheraApiException ex)
    {
        if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
        Mostrar(ex.StatusCode == 403 ? "Você não tem permissão para gerenciar os produtos desta empresa." : ex.Message);
    }

    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
