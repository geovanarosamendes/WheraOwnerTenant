<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Adm_Home" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Visão geral | Whera Tenant</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho-pagina"><div><span class="sobretitulo">OWNER TENANT</span><h1>Visão geral</h1><p>Base inicial conectada à API Whera.</p></div></div>
    <div class="painel"><h2>Setup concluído</h2><p>A autenticação, a sessão e o cliente HTTP estão preparados para receber as primeiras funcionalidades do tenant.</p><p class="aviso">As áreas de redes sociais, métricas e relatórios serão implementadas somente após confirmação dos contratos disponíveis na API.</p></div>
</asp:Content>
