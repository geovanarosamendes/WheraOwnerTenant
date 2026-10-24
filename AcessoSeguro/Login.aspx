<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="AcessoSeguro_Login" %>

<!DOCTYPE html>
<html lang="pt-br">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Entrar | Whera Tenant</title>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600,700" />
    <link href="~/CSS/site.css" rel="stylesheet" type="text/css" />
</head>
<body class="pagina-login">
    <form id="formLogin" runat="server">
        <div class="caixa-login">
            <h1>Whera Tenant</h1>

            <asp:Panel ID="pnlExpirou" runat="server" CssClass="aviso" Visible="false">
                Sua sessão expirou. Entre novamente.
            </asp:Panel>
            <asp:Panel ID="pnlErro" runat="server" CssClass="erro" Visible="false">
                <asp:Literal ID="litErro" runat="server" />
            </asp:Panel>

            <%-- Etapa 1: e-mail e senha --%>
            <asp:Panel ID="pnlCredenciais" runat="server" DefaultButton="btnContinuar">
                <label class="campo">
                    <span>E-mail</span>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="254" autocomplete="username" />
                </label>
                <label class="campo">
                    <span>Senha</span>
                    <asp:TextBox ID="txtSenha" runat="server" TextMode="Password" MaxLength="128" autocomplete="current-password" />
                </label>
                <asp:Button ID="btnContinuar" runat="server" Text="Continuar" CssClass="botao" OnClick="btnContinuar_Click" />
            </asp:Panel>

            <%-- Etapa 2: confirmação adicional exigida pela API --%>
            <asp:Panel ID="pnlSegundaSenha" runat="server" Visible="false" DefaultButton="btnEntrar">
                <p class="dica">Entrando como <strong><asp:Literal ID="litEmail" runat="server" /></strong>. Informe a senha de administrador.</p>
                <label class="campo">
                    <span>Senha adicional</span>
                    <asp:TextBox ID="txtSegundaSenha" runat="server" TextMode="Password" MaxLength="128" autocomplete="off" />
                </label>
                <asp:Button ID="btnEntrar" runat="server" Text="Entrar" CssClass="botao" OnClick="btnEntrar_Click" />
                <asp:LinkButton ID="btnVoltar" runat="server" CssClass="link-voltar" OnClick="btnVoltar_Click">Usar outra conta</asp:LinkButton>
            </asp:Panel>

            <asp:Panel ID="pnlDefinirSegundaSenha" runat="server" Visible="false" DefaultButton="btnDefinir">
                <p class="dica">Esta é a primeira entrada do proprietário. Crie uma segunda senha exclusiva para administrar a empresa.</p>
                <label class="campo"><span>Nova segunda senha</span><asp:TextBox ID="txtNovaSegundaSenha" runat="server" TextMode="Password" MaxLength="128" autocomplete="new-password" /></label>
                <label class="campo"><span>Confirme a segunda senha</span><asp:TextBox ID="txtConfirmaSegundaSenha" runat="server" TextMode="Password" MaxLength="128" autocomplete="new-password" /></label>
                <asp:Button ID="btnDefinir" runat="server" Text="Criar senha e entrar" CssClass="botao" OnClick="btnDefinir_Click" />
                <asp:LinkButton ID="btnVoltarDefinicao" runat="server" CssClass="link-voltar" OnClick="btnVoltar_Click">Usar outra conta</asp:LinkButton>
            </asp:Panel>
            <p class="dica"><a href="~/AcessoSeguro/CadastroInicial.aspx">Ainda não tenho vínculo: iniciar cadastro</a></p>
        </div>
    </form>
</body>
</html>
