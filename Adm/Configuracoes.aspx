<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Configuracoes.aspx.cs" Inherits="Adm_Configuracoes" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Configurações | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Configurações</h1><p>Aparência do painel e segurança do seu acesso.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <section class="secao">
        <div class="secao-cabecalho"><h2>Aparência</h2><p>Vale para este navegador.</p></div>
        <fieldset class="opcoes-tema" id="opcoesTema">
            <legend class="oculto-visual">Tema do painel</legend>
            <label class="opcao-tema"><input type="radio" name="tema" value="claro" /><span class="tema-amostra tema-amostra-claro" aria-hidden="true"></span><strong>Claro</strong></label>
            <label class="opcao-tema"><input type="radio" name="tema" value="escuro" /><span class="tema-amostra tema-amostra-escuro" aria-hidden="true"></span><strong>Escuro</strong></label>
            <label class="opcao-tema"><input type="radio" name="tema" value="sistema" /><span class="tema-amostra tema-amostra-sistema" aria-hidden="true"></span><strong>Igual ao dispositivo</strong></label>
        </fieldset>
    </section>
    <section class="secao">
        <div class="secao-cabecalho"><h2>Senha</h2><p>A mesma senha da sua conta no aplicativo.</p></div>
        <div class="campos">
            <label class="campo c4"><span>Senha atual</span><asp:TextBox ID="txtSenhaAtual" runat="server" TextMode="Password" autocomplete="current-password" /></label>
            <label class="campo c4"><span>Nova senha</span><asp:TextBox ID="txtNovaSenha" runat="server" TextMode="Password" autocomplete="new-password" /></label>
            <div class="acoes c4"><asp:Button ID="btnSenha" runat="server" Text="Alterar senha" CssClass="botao-secundario" OnClick="btnSenha_Click" /></div>
        </div>
    </section>
    <section class="secao">
        <div class="secao-cabecalho"><h2>Segunda senha</h2><p>Pedida ao entrar na gestão. É diferente da senha da conta.</p></div>
        <div class="campos">
            <label class="campo c4"><span>Segunda senha atual</span><asp:TextBox ID="txtSegundaAtual" runat="server" TextMode="Password" autocomplete="off" /></label>
            <label class="campo c4"><span>Nova segunda senha</span><asp:TextBox ID="txtNovaSegunda" runat="server" TextMode="Password" autocomplete="new-password" /></label>
            <div class="acoes c4"><asp:Button ID="btnSegunda" runat="server" Text="Alterar segunda senha" CssClass="botao-secundario" OnClick="btnSegunda_Click" /></div>
        </div>
    </section>
</asp:Content>
