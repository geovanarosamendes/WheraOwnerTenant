<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Promocoes.aspx.cs" Inherits="Adm_Promocoes" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Promoções | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Promoções</h1><p>As promoções comuns e relâmpago da sua empresa. Para criar ou encerrar, use o aplicativo Elo.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <dl class="resumo">
            <div><dt>No ar agora</dt><dd><asp:Literal ID="litAtivas" runat="server" /></dd></div>
            <div><dt>Programadas</dt><dd><asp:Literal ID="litProgramadas" runat="server" /></dd></div>
            <div><dt>Comuns</dt><dd><asp:Literal ID="litComuns" runat="server" /></dd></div>
            <div><dt>Relâmpago</dt><dd><asp:Literal ID="litRelampago" runat="server" /></dd></div>
        </dl>
        <nav class="abas" aria-label="Tipo de promoção"><asp:Literal ID="litAbas" runat="server" /></nav>
        <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><h2><asp:Literal ID="litVazioTitulo" runat="server" /></h2><p>Crie pelo aplicativo Elo, em Gestão da empresa. Ela aparece aqui em seguida.</p></asp:Panel>
        <asp:Panel ID="pnlAtivas" runat="server" CssClass="secao" Visible="false">
            <div class="secao-cabecalho"><h2>No ar agora</h2></div>
            <asp:Repeater ID="rptAtivas" runat="server"><HeaderTemplate><ul class="linhas"></HeaderTemplate><ItemTemplate><%# Linha(Container.DataItem) %></ItemTemplate><FooterTemplate></ul></FooterTemplate></asp:Repeater>
        </asp:Panel>
        <asp:Panel ID="pnlProgramadas" runat="server" CssClass="secao" Visible="false">
            <div class="secao-cabecalho"><h2>Programadas</h2></div>
            <asp:Repeater ID="rptProgramadas" runat="server"><HeaderTemplate><ul class="linhas"></HeaderTemplate><ItemTemplate><%# Linha(Container.DataItem) %></ItemTemplate><FooterTemplate></ul></FooterTemplate></asp:Repeater>
        </asp:Panel>
        <asp:Panel ID="pnlRascunhos" runat="server" CssClass="secao" Visible="false">
            <div class="secao-cabecalho"><h2>Rascunhos</h2></div>
            <asp:Repeater ID="rptRascunhos" runat="server"><HeaderTemplate><ul class="linhas"></HeaderTemplate><ItemTemplate><%# Linha(Container.DataItem) %></ItemTemplate><FooterTemplate></ul></FooterTemplate></asp:Repeater>
        </asp:Panel>
        <asp:Panel ID="pnlPassadas" runat="server" CssClass="secao" Visible="false">
            <div class="secao-cabecalho"><h2>Encerradas</h2></div>
            <asp:Repeater ID="rptPassadas" runat="server"><HeaderTemplate><ul class="linhas"></HeaderTemplate><ItemTemplate><%# Linha(Container.DataItem) %></ItemTemplate><FooterTemplate></ul></FooterTemplate></asp:Repeater>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
