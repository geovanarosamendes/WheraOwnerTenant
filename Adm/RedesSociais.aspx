<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="RedesSociais.aspx.cs" Inherits="Adm_RedesSociais" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Redes sociais | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Redes sociais</h1><p>Conectar contas e acompanhar resultados ainda não está disponível.</p></div></div>
    <section class="secao">
        <div class="secao-cabecalho"><h2>Enquanto isso, mostre seu Instagram no perfil</h2><p>Informe o endereço em Dados da empresa para os clientes encontrarem você.</p></div>
        <a runat="server" class="botao-secundario" href="~/Adm/MinhaEmpresa.aspx">Abrir dados da empresa</a>
    </section>
</asp:Content>
