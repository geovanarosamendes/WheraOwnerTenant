<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Previa.aspx.cs" Inherits="Adm_Previa" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Prévia pública | Whera Tenant</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho-pagina"><div><span class="sobretitulo">MINHA EMPRESA</span><h1>Prévia pública</h1><p>Confira como os dados já cadastrados podem aparecer no perfil da empresa.</p></div><a runat="server" class="botao botao-secundario" href="~/Adm/MinhaEmpresa.aspx">Editar perfil</a></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="erro" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="painel" Visible="false"><h2>Nenhuma empresa vinculada</h2><p>A API não retornou uma empresa autorizada para esta conta.</p></asp:Panel>
    <asp:Panel ID="pnlPrevia" runat="server" CssClass="painel previa" Visible="false">
        <asp:Image ID="imgLogo" runat="server" CssClass="previa-logo" AlternateText="Logo da empresa" />
        <div class="previa-conteudo"><span class="etiqueta"><asp:Literal ID="litStatus" runat="server" /></span><h2><asp:Literal ID="litNome" runat="server" /></h2><p class="previa-local"><asp:Literal ID="litCategoria" runat="server" /><asp:Literal ID="litCidade" runat="server" /></p><p><asp:Literal ID="litDescricao" runat="server" /></p><div class="previa-canais"><asp:Literal ID="litCanais" runat="server" /></div></div>
    </asp:Panel>
</asp:Content>
