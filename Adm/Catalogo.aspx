<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Catalogo.aspx.cs" Inherits="Adm_Catalogo" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Catálogo | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <aside class="oferta">
        <div>
            <span class="oferta-selo">Serviço pago</span>
            <h2>Leve seus produtos para fora do Elo com a Vitrine Elo</h2>
            <p>Monte uma página com a cara da sua empresa e envie o link pelo WhatsApp e pelo Instagram.</p>
        </div>
        <a runat="server" class="oferta-botao" href="~/Adm/VitrineElo.aspx">Montar minha Vitrine</a>
    </aside>
    <div class="cabecalho"><div><h1>Catálogo</h1><p>Os produtos que aparecem no perfil da sua empresa.</p></div><div class="cabecalho-acoes"><a runat="server" class="botao-secundario" href="~/Adm/ProdutosGaleria.aspx">Gerenciar produtos</a></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <div class="visao catalogo-visao">
            <section class="secao">
                <div class="secao-cabecalho"><h2><asp:Literal ID="litTotalCatalogo" runat="server" /></h2></div>
                <asp:Repeater ID="rptProdutos" runat="server">
                    <HeaderTemplate><div class="grade-produtos"></HeaderTemplate>
                    <ItemTemplate><article class="produto"><div class="produto-foto"><%# Foto(Container.DataItem) %></div><div class="produto-corpo"><strong><%# HttpUtility.HtmlEncode(Eval("Nome")) %></strong><span class="produto-preco"><%# Preco(Container.DataItem) %></span></div></article></ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlSemProdutos" runat="server" CssClass="vazio" Visible="false"><h2>Seu catálogo está vazio</h2><p>Abra a galeria, selecione produtos e adicione ao catálogo.</p><a runat="server" class="botao-secundario" href="~/Adm/ProdutosGaleria.aspx?filtro=fora">Abrir galeria</a></asp:Panel>
            </section>
            <section class="secao">
                <div class="secao-cabecalho"><h2>Como o cliente vê</h2></div>
                <div class="celular-tela catalogo-tela">
                    <div class="catalogo-topo"><span class="card-logo"><asp:Literal ID="litInicial" runat="server" /></span><strong><asp:Literal ID="litEmpresa" runat="server" /></strong></div>
                    <p class="catalogo-titulo">Produtos</p>
                    <asp:Repeater ID="rptPrevia" runat="server">
                        <HeaderTemplate><div class="catalogo-grade"></HeaderTemplate>
                        <ItemTemplate><div class="catalogo-item"><div class="catalogo-foto"><%# Foto(Container.DataItem) %></div><span><%# HttpUtility.HtmlEncode(Eval("Nome")) %></span><strong><%# Preco(Container.DataItem) %></strong></div></ItemTemplate>
                        <FooterTemplate></div></FooterTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlPreviaVazia" runat="server" CssClass="catalogo-vazio" Visible="false">Os produtos do catálogo aparecem aqui.</asp:Panel>
                </div>
                <small>Prévia aproximada, com até 6 produtos.</small>
            </section>
        </div>
        <section class="secao">
            <div class="secao-cabecalho"><h2>Fotos em destaque</h2><p>Até seis fotos com título e link, usadas no site do Elo. No aplicativo, o perfil mostra os produtos do catálogo.</p></div>
            <div class="campos">
                <label class="campo c6"><span class="oculto-visual">Foto</span><asp:FileUpload ID="fuImagem" runat="server" accept="image/jpeg,image/png,image/webp" /></label>
                <div class="acoes c3"><asp:Button ID="btnAdicionar" runat="server" Text="Enviar foto" CssClass="botao-secundario" OnClick="btnAdicionar_Click" /></div>
            </div>
            <div data-alteracoes="">
                <asp:Repeater ID="rptCatalogo" runat="server">
                    <ItemTemplate>
                        <div class="item-catalogo">
                            <asp:HiddenField ID="hidImagem" runat="server" Value='<%# Eval("Imagem") %>' />
                            <img src="<%# HttpUtility.HtmlAttributeEncode(WheraOwnerTenant.WheraApi.UrlMidia(Convert.ToString(Eval("Imagem")))) %>" alt="Foto em destaque" />
                            <label class="campo"><span>Título</span><asp:TextBox ID="txtTitulo" runat="server" Text='<%# Eval("Titulo") %>' MaxLength="80" /></label>
                            <label class="campo"><span>Link</span><asp:TextBox ID="txtLink" runat="server" Text='<%# Eval("Link") %>' MaxLength="500" placeholder="https://" /></label>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <div class="barra-salvar">
                    <p data-aviso-alteracao="">Salve depois de editar títulos e links.</p>
                    <div class="acoes"><asp:Button ID="btnSalvar" runat="server" Text="Salvar fotos" CssClass="botao-primario" OnClick="btnSalvar_Click" /></div>
                </div>
            </div>
        </section>
    </asp:Panel>
</asp:Content>
