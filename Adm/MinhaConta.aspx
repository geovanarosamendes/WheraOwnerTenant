<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="MinhaConta.aspx.cs" Inherits="Adm_MinhaConta" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Minha conta | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Minha conta</h1><p>Seus dados pessoais. Para trocar a senha ou o tema, abra <a runat="server" href="~/Adm/Configuracoes.aspx">Configurações</a>.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <section class="secao">
        <div class="secao-cabecalho"><h2>Dados pessoais</h2></div>
        <div class="campos">
            <label class="campo c4"><span>Nome</span><asp:TextBox ID="txtNome" runat="server" MaxLength="150" /></label>
            <label class="campo c4"><span>E-mail</span><asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="254" /></label>
            <div class="acoes c4"><asp:Button ID="btnSalvarConta" runat="server" Text="Salvar dados" CssClass="botao-primario" OnClick="btnSalvarConta_Click" /></div>
        </div>
    </section>
</asp:Content>
