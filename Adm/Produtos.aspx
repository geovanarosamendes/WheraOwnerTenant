<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Produtos.aspx.cs" Inherits="Adm_Produtos" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Produtos | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Cadastrar produto</h1><p>Depois de cadastrar, abra a galeria para colocar o produto no catálogo.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <section class="secao">
            <div class="secao-cabecalho"><h2><asp:Literal ID="litTituloFormulario" runat="server" Text="Novo produto" /></h2></div>
            <div class="campos">
                <asp:HiddenField ID="hidProdutoId" runat="server" />
                <label class="campo c6"><span>Nome <sup>*</sup></span><asp:TextBox ID="txtNome" runat="server" MaxLength="160" /></label>
                <label class="campo c2"><span>Preço</span><asp:TextBox ID="txtPreco" runat="server" MaxLength="12" placeholder="29,90" /></label>
                <label class="campo c3"><span>SKU</span><asp:TextBox ID="txtSku" runat="server" MaxLength="60" placeholder="Código do produto" /></label>
                <label class="campo c3"><span>Estoque <sup>*</sup></span><asp:TextBox ID="txtEstoque" runat="server" MaxLength="7" TextMode="Number" min="0" placeholder="0" /><small>Sem estoque, o produto não vai para o catálogo.</small></label>
                <label class="campo c4"><span>Foto</span><asp:FileUpload ID="fuImagem" runat="server" accept="image/jpeg,image/png,image/webp" /><small>JPG, PNG ou WebP de até 5 MB.</small></label>
                <label class="campo"><span>Descrição</span><asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="2" MaxLength="2000" /></label>
                <div class="campo-opcao c4"><asp:CheckBox ID="chkDisponivel" runat="server" Text="Ativo, disponível para venda" Checked="true" /></div>
                <div class="acoes acoes-fim c8"><asp:Button ID="btnCancelar" runat="server" Text="Cancelar edição" CssClass="botao-secundario" OnClick="btnCancelar_Click" Visible="false" CausesValidation="false" /><asp:Button ID="btnSalvar" runat="server" Text="Salvar produto" CssClass="botao-primario" OnClick="btnSalvar_Click" /></div>
            </div>
        </section>
        <section class="secao">
            <div class="secao-cabecalho"><h2>Seus produtos</h2><a runat="server" class="botao-texto botao-pequeno" href="~/Adm/ProdutosGaleria.aspx">Abrir galeria</a></div>
            <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><p>Nenhum produto cadastrado ainda. Use o formulário acima para adicionar o primeiro.</p></asp:Panel>
            <asp:Repeater ID="rptProdutos" runat="server" OnItemCommand="rptProdutos_ItemCommand">
                <HeaderTemplate><div class="tabela-rolagem" tabindex="0"><table class="tabela"><thead><tr><th class="coluna-foto">Foto</th><th>Produto</th><th class="numero">Preço</th><th>Situação</th><th><span class="oculto-visual">Ações</span></th></tr></thead><tbody></HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="coluna-foto"><%# Foto((WheraOwnerTenant.ProdutoTenant)Container.DataItem) %></td>
                        <td><strong><%# HttpUtility.HtmlEncode(Eval("Nome")) %></strong></td>
                        <td class="numero"><%# Preco((WheraOwnerTenant.ProdutoTenant)Container.DataItem) %></td>
                        <td><%# Situacao((WheraOwnerTenant.ProdutoTenant)Container.DataItem) %></td>
                        <td><div class="linha-acoes">
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("Id") %>' CssClass="botao-secundario botao-pequeno" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Excluir" CommandArgument='<%# Eval("Id") %>' CssClass="botao-perigo botao-pequeno" CausesValidation="false" data-confirmar="Excluir este produto?" data-confirmar-acao="Excluir">Excluir</asp:LinkButton>
                        </div></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></tbody></table></div></FooterTemplate>
            </asp:Repeater>
        </section>
    </asp:Panel>
</asp:Content>
