using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

public partial class Adm_Produtos : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.Produtos.Empresa";
    private const string ChaveProdutos = "WheraOwnerTenant.Produtos.Lista";
    private const int TamanhoMaximoFoto = 5 * 1024 * 1024;
    private static readonly string[] TiposFoto = { "image/jpeg", "image/png", "image/webp" };
    private static readonly CultureInfo Brasil = new CultureInfo("pt-BR");

    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private string EmpresaId { get { return Convert.ToString(Session[ChaveEmpresa]); } }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { Mostrar("Nenhuma empresa vinculada."); return; }
            Session[ChaveEmpresa] = empresa.Id;
            Listar();
            pnlConteudo.Visible = true;
            var editar = (Session[ChaveProdutos] as List<ProdutoTenant> ?? new List<ProdutoTenant>()).FirstOrDefault(p => p.Id == Request.QueryString["editar"]);
            if (editar != null) Preencher(editar);
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    private void Listar()
    {
        var produtos = TenantApi.Produtos(Api, EmpresaId);
        Session[ChaveProdutos] = produtos;
        rptProdutos.DataSource = produtos;
        rptProdutos.DataBind();
        rptProdutos.Visible = produtos.Count > 0;
        pnlVazio.Visible = produtos.Count == 0;
    }

    protected void btnSalvar_Click(object sender, EventArgs e)
    {
        pnlErro.Visible = false; pnlSucesso.Visible = false;
        try
        {
            var nome = (txtNome.Text ?? "").Trim();
            if (nome.Length == 0) { Mostrar("Informe o nome do produto."); return; }

            decimal? preco = null;
            var precoTexto = (txtPreco.Text ?? "").Trim();
            if (precoTexto.Length > 0)
            {
                decimal valor;
                if (!decimal.TryParse(precoTexto, NumberStyles.Number, Brasil, out valor) || valor < 0) { Mostrar("Informe um preço válido, como 29,90."); return; }
                preco = valor;
            }

            int estoque;
            if (!int.TryParse((txtEstoque.Text ?? "").Trim(), out estoque) || estoque < 0) { Mostrar("Informe a quantidade em estoque."); return; }

            var foto = fuImagem.HasFile ? fuImagem.PostedFile : null;
            if (foto != null && (foto.ContentLength > TamanhoMaximoFoto || !TiposFoto.Contains((foto.ContentType ?? "").ToLowerInvariant()))) { Mostrar("Envie uma foto JPG, PNG ou WebP de até 5 MB."); return; }

            var disponivel = chkDisponivel.Checked;
            var id = hidProdutoId.Value;
            var atual = (Session[ChaveProdutos] as List<ProdutoTenant> ?? new List<ProdutoTenant>()).FirstOrDefault(p => p.Id == id);
            // A entrada no catálogo é feita na galeria ou no aplicativo; aqui só é preservada. Produto indisponível não pode ficar publicado.
            var publicado = atual != null && atual.Publicado && disponivel && estoque > 0;
            var pedido = new { Nome = nome, Descricao = (txtDescricao.Text ?? "").Trim(), Preco = preco, Disponivel = disponivel, Publicado = publicado, Sku = (txtSku.Text ?? "").Trim(), Estoque = estoque };

            var salvo = string.IsNullOrEmpty(id) ? TenantApi.CriarProduto(Api, EmpresaId, pedido) : TenantApi.AtualizarProduto(Api, EmpresaId, id, pedido);
            if (foto != null) TenantApi.EnviarImagemProduto(Api, EmpresaId, salvo.Id, foto.FileName, foto.ContentType, fuImagem.FileBytes);

            LimparFormulario();
            Listar();
            pnlSucesso.Visible = true;
            litSucesso.Text = "Produto salvo.";
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    protected void btnCancelar_Click(object sender, EventArgs e) { LimparFormulario(); }

    protected void rptProdutos_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        pnlErro.Visible = false; pnlSucesso.Visible = false;
        var id = Convert.ToString(e.CommandArgument);
        try
        {
            if (e.CommandName == "Excluir")
            {
                TenantApi.ExcluirProduto(Api, EmpresaId, id);
                if (hidProdutoId.Value == id) LimparFormulario();
                Listar();
                pnlSucesso.Visible = true;
                litSucesso.Text = "Produto excluído.";
                return;
            }

            var produto = (Session[ChaveProdutos] as List<ProdutoTenant> ?? new List<ProdutoTenant>()).FirstOrDefault(p => p.Id == id);
            if (produto == null) { Listar(); Mostrar("Este produto não está mais disponível."); return; }
            Preencher(produto);
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    private void Preencher(ProdutoTenant produto)
    {
        hidProdutoId.Value = produto.Id;
        txtNome.Text = produto.Nome;
        txtDescricao.Text = produto.Descricao;
        txtPreco.Text = produto.Preco.HasValue ? produto.Preco.Value.ToString("0.00", Brasil) : "";
        txtSku.Text = produto.Sku;
        txtEstoque.Text = produto.Estoque.HasValue ? produto.Estoque.Value.ToString(CultureInfo.InvariantCulture) : "";
        chkDisponivel.Checked = produto.Disponivel;
        litTituloFormulario.Text = "Editar produto";
        btnCancelar.Visible = true;
    }

    protected string Foto(ProdutoTenant produto)
    {
        var imagem = produto.Imagens == null ? null : produto.Imagens.OrderByDescending(i => i.Principal).FirstOrDefault();
        return imagem == null ? "<span class=\"sem-foto\">Sem foto</span>" : "<img src=\"" + HttpUtility.HtmlAttributeEncode(WheraApi.UrlMidia(imagem.Url)) + "\" alt=\"\" />";
    }

    protected string Preco(ProdutoTenant produto) { return produto.Preco.HasValue ? produto.Preco.Value.ToString("C", Brasil) : "Sem preço"; }

    protected string Situacao(ProdutoTenant produto)
    {
        if (!produto.Disponivel) return "<span class=\"selo selo-neutro\">Inativo</span>";
        return produto.Publicado ? "<span class=\"selo selo-sucesso\">No catálogo</span>" : "<span class=\"selo selo-aviso\">Fora do catálogo</span>";
    }

    private void LimparFormulario()
    {
        hidProdutoId.Value = "";
        txtNome.Text = "";
        txtDescricao.Text = "";
        txtPreco.Text = "";
        txtSku.Text = "";
        txtEstoque.Text = "";
        chkDisponivel.Checked = true;
        litTituloFormulario.Text = "Novo produto";
        btnCancelar.Visible = false;
    }

    private void Erro(WheraApiException ex)
    {
        if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
        Mostrar(ex.StatusCode == 403 ? "Você não tem permissão para gerenciar os produtos desta empresa." : ex.Message);
    }

    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
