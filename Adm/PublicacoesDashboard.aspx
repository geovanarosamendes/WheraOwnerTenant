<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="PublicacoesDashboard.aspx.cs" Inherits="Adm_PublicacoesDashboard" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Relatório de publicações | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Relatório de publicações</h1><p>O ritmo de divulgação da sua empresa nos últimos seis meses.</p></div></div>
    <nav class="abas" aria-label="Relatório de publicações"><a runat="server" class="aba ativa" aria-current="page" href="~/Adm/PublicacoesDashboard.aspx">Painel</a><a runat="server" class="aba" href="~/Adm/Conteudos.aspx">Histórico</a><a runat="server" class="aba" href="~/Adm/PublicacoesRelatorios.aspx">Exportar</a></nav>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma publicação ainda</h2><p>Crie um story ou uma promoção pelo aplicativo Elo. Os números aparecem aqui.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <dl class="resumo">
            <div><dt>Ativas agora</dt><dd><asp:Literal ID="litAtivas" runat="server" /></dd></div>
            <div><dt>Terminam em 7 dias</dt><dd><asp:Literal ID="litTerminam" runat="server" /></dd></div>
            <div><dt>Publicadas em 30 dias</dt><dd><asp:Literal ID="litTrintaDias" runat="server" /></dd></div>
            <div><dt>Relâmpago no total</dt><dd><asp:Literal ID="litRelampago" runat="server" /></dd></div>
        </dl>
        <div class="colunas">
            <section class="secao">
                <div class="secao-cabecalho"><h2>Publicações por mês</h2><asp:Literal ID="litLegenda" runat="server" /></div>
                <asp:Literal ID="litGrafico" runat="server" />
            </section>
            <section class="secao">
                <div class="secao-cabecalho"><h2>Por tipo</h2></div>
                <asp:Literal ID="litProporcao" runat="server" />
            </section>
        </div>
        <section class="secao">
            <div class="secao-cabecalho"><h2>Terminam primeiro</h2><a runat="server" class="botao-texto botao-pequeno" href="~/Adm/Conteudos.aspx">Ver histórico</a></div>
            <asp:Repeater ID="rptProximas" runat="server"><HeaderTemplate><ul class="linhas"></HeaderTemplate><ItemTemplate><%# Linha(Container.DataItem) %></ItemTemplate><FooterTemplate></ul></FooterTemplate></asp:Repeater>
            <asp:Panel ID="pnlSemProximas" runat="server" CssClass="vazio" Visible="false"><p>Nenhuma publicação no ar neste momento.</p></asp:Panel>
        </section>
    </asp:Panel>
</asp:Content>
