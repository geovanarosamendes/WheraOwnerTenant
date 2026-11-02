<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Conteudos.aspx.cs" Inherits="Adm_Conteudos" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Histórico de publicações | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Histórico de publicações</h1><p>Stories e promoções da sua empresa. Para publicar, use o aplicativo Elo.</p></div></div>
    <nav class="abas" aria-label="Relatório de publicações"><a runat="server" class="aba" href="~/Adm/PublicacoesDashboard.aspx">Painel</a><a runat="server" class="aba ativa" aria-current="page" href="~/Adm/Conteudos.aspx">Histórico</a><a runat="server" class="aba" href="~/Adm/PublicacoesRelatorios.aspx">Exportar</a></nav>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <dl class="resumo">
            <div><dt>Ativas agora</dt><dd><asp:Literal ID="litAtivas" runat="server" /></dd></div>
            <div><dt>Programadas</dt><dd><asp:Literal ID="litProgramadas" runat="server" /></dd></div>
            <div><dt>Passadas</dt><dd><asp:Literal ID="litPassadas" runat="server" /></dd></div>
            <div><dt>Stories</dt><dd><asp:Literal ID="litStories" runat="server" /></dd></div>
            <div><dt>Promoções</dt><dd><asp:Literal ID="litPromocoes" runat="server" /></dd></div>
        </dl>
        <section class="secao">
            <div class="secao-cabecalho"><h2>No ar agora</h2></div>
            <asp:Repeater ID="rptAtivas" runat="server"><HeaderTemplate><ul class="linhas"></HeaderTemplate><ItemTemplate><%# Linha(Container.DataItem) %></ItemTemplate><FooterTemplate></ul></FooterTemplate></asp:Repeater>
            <asp:Panel ID="pnlSemAtivas" runat="server" CssClass="vazio" Visible="false"><p>Nenhuma publicação no ar neste momento. Crie uma pelo aplicativo Elo.</p></asp:Panel>
        </section>
        <asp:Panel ID="pnlProgramadas" runat="server" CssClass="secao" Visible="false">
            <div class="secao-cabecalho"><h2>Programadas</h2></div>
            <asp:Repeater ID="rptProgramadas" runat="server"><HeaderTemplate><ul class="linhas"></HeaderTemplate><ItemTemplate><%# Linha(Container.DataItem) %></ItemTemplate><FooterTemplate></ul></FooterTemplate></asp:Repeater>
        </asp:Panel>
        <section class="secao">
            <div class="secao-cabecalho">
                <h2>Publicações passadas</h2>
                <label><span class="oculto-visual">Filtrar por tipo</span><asp:DropDownList ID="ddlTipo" runat="server" CssClass="controle controle-curto" AutoPostBack="true" OnSelectedIndexChanged="ddlTipo_SelectedIndexChanged"><asp:ListItem Text="Todos os tipos" Value="" /><asp:ListItem Text="Stories" Value="Story" /><asp:ListItem Text="Promoções" Value="Promocao" /></asp:DropDownList></label>
            </div>
            <asp:Repeater ID="rptPassadas" runat="server"><HeaderTemplate><ul class="linhas"></HeaderTemplate><ItemTemplate><%# Linha(Container.DataItem) %></ItemTemplate><FooterTemplate></ul></FooterTemplate></asp:Repeater>
            <asp:Panel ID="pnlSemPassadas" runat="server" CssClass="vazio" Visible="false"><p>Nenhuma publicação passada para este filtro.</p></asp:Panel>
        </section>
    </asp:Panel>
</asp:Content>
