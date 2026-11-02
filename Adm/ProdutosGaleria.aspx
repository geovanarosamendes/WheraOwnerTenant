<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="ProdutosGaleria.aspx.cs" Inherits="Adm_ProdutosGaleria" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Galeria de produtos | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho">
        <div><h1>Galeria de produtos</h1><p>Selecione produtos para colocar no catálogo, tirar do catálogo, inativar ou excluir.</p></div>
        <div class="cabecalho-acoes"><a runat="server" class="botao-secundario" href="~/Adm/ProdutosImportar.aspx">Importar planilha</a><a runat="server" class="botao-primario" href="~/Adm/Produtos.aspx">Cadastrar produto</a></div>
    </div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <nav class="abas" aria-label="Filtrar produtos"><asp:Literal ID="litAbas" runat="server" /></nav>
        <div class="barra-selecao" id="barraSelecao">
            <label class="selecionar-todos"><input type="checkbox" id="chkTodos" /> Selecionar todos</label>
            <span id="totalSelecionados" role="status">Nenhum selecionado</span>
            <div class="acoes">
                <asp:Button ID="btnAdicionar" runat="server" Text="Adicionar ao catálogo" CssClass="botao-primario botao-pequeno" data-acao="adicionar" OnClick="btnAdicionar_Click" />
                <asp:Button ID="btnRemover" runat="server" Text="Remover do catálogo" CssClass="botao-secundario botao-pequeno" data-acao="remover" OnClick="btnRemover_Click" />
                <asp:Button ID="btnInativar" runat="server" Text="Inativar" CssClass="botao-secundario botao-pequeno" data-acao="inativar" OnClick="btnInativar_Click" />
                <asp:Button ID="btnReativar" runat="server" Text="Reativar" CssClass="botao-secundario botao-pequeno" data-acao="reativar" OnClick="btnReativar_Click" />
                <asp:Button ID="btnExcluir" runat="server" Text="Excluir" CssClass="botao-perigo botao-pequeno" data-acao="excluir" data-confirmar="Excluir os produtos selecionados?" data-confirmar-acao="Excluir" OnClick="btnExcluir_Click" />
            </div>
        </div>
        <asp:Repeater ID="rptProdutos" runat="server">
            <HeaderTemplate><div class="grade-produtos"></HeaderTemplate>
            <ItemTemplate>
                <article class='produto <%# (bool)Eval("Disponivel") ? "" : "produto-inativo" %>' data-no-catalogo='<%# (bool)Eval("Publicado") ? "1" : "0" %>' data-inativo='<%# (bool)Eval("Disponivel") ? "0" : "1" %>'>
                    <asp:HiddenField ID="hidId" runat="server" Value='<%# Eval("Id") %>' />
                    <div class="produto-foto">
                        <%# TenantApi_Foto(Container.DataItem) %>
                        <span class="produto-marca"><asp:CheckBox ID="chkSelecionado" runat="server" /></span>
                    </div>
                    <div class="produto-corpo">
                        <%# Selo(Container.DataItem) %>
                        <strong><%# HttpUtility.HtmlEncode(Eval("Nome")) %></strong>
                        <span class="produto-preco"><%# Preco(Container.DataItem) %></span>
                        <a class="botao-texto botao-pequeno" href='<%# ResolveUrl("~/Adm/Produtos.aspx?editar=" + HttpUtility.UrlEncode(Convert.ToString(Eval("Id")))) %>'>Editar</a>
                    </div>
                </article>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><asp:Literal ID="litVazio" runat="server" /></asp:Panel>
    </asp:Panel>
</asp:Content>
