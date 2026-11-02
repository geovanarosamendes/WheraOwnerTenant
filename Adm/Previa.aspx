<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Previa.aspx.cs" Inherits="Adm_Previa" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Como aparece no app | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <link href='<%= ResolveUrl("~/CSS/previa-app.css?v=6") %>' rel="stylesheet" type="text/css" />
    <div class="cabecalho"><div><h1>Como aparece no app</h1><p>As telas do Elo com os dados da sua empresa e os recursos que você ainda pode usar.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlParcial" runat="server" CssClass="alerta alerta-aviso" role="status" Visible="false">Não foi possível conferir alguns dados agora. Os itens marcados como “Não verificado” podem já estar em uso.</asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlPrevia" runat="server" Visible="false">
        <nav class="abas" aria-label="Telas do aplicativo"><asp:Literal ID="litAbas" runat="server" /></nav>
        <div class="previa-app">
            <div class="previa-coluna-celular">
                <div class="celular-app"><asp:Literal ID="litTela" runat="server" /></div>
            </div>
            <div class="previa-recursos">
                <div class="previa-resumo">
                    <div><strong><asp:Literal ID="litContagem" runat="server" /></strong><asp:Literal ID="litStatus" runat="server" /></div>
                    <div class="previa-barra" aria-hidden="true"><span style="width:<%= PercentualEmUso %>%"></span></div>
                    <p class="previa-legenda"><span class="previa-amostra" aria-hidden="true"></span><asp:Literal ID="litLegenda" runat="server" /></p>
                </div>
                <asp:Panel ID="pnlFaltando" runat="server" CssClass="secao">
                    <div class="secao-cabecalho"><h2><asp:Literal ID="litTituloFaltando" runat="server" /></h2></div>
                    <asp:Literal ID="litFaltando" runat="server" />
                </asp:Panel>
                <asp:Panel ID="pnlEmUso" runat="server" CssClass="secao">
                    <div class="secao-cabecalho"><h2><asp:Literal ID="litTituloEmUso" runat="server" /></h2></div>
                    <asp:Literal ID="litEmUso" runat="server" />
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
    <script src='<%= ResolveUrl("~/Scripts/previa-app.js?v=3") %>'></script>
</asp:Content>
