<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="AcessoSeguro_Login" %>

<!DOCTYPE html>
<html lang="pt-br">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <script src="../Scripts/tema.js?v=1"></script>
    <title>Entrar | Elo Gestão</title>
    <link runat="server" href="~/CSS/site.css?v=17" rel="stylesheet" type="text/css" />
</head>
<body class="pagina-login" data-visual-context="management">
    <form id="formLogin" runat="server">
        <div class="login">
            <section class="login-marca">
                <img class="login-arte" src='<%= ResolveUrl(ImagemLogin) %>' alt="" />
                <h1>Sua empresa, perto de quem procura.</h1>
                <p>Cuide do perfil, das publicações e dos produtos que os clientes encontram no Elo.</p>
            </section>
            <main class="login-painel">
                <div class="login-logo-area"><img class="login-logo" src='<%= ResolveUrl("~/Images/elo-wordmark.png") %>' alt="Elo" /></div>
                <h2>Entrar na gestão</h2>
                <p class="login-intro">Use a conta vinculada à sua empresa.</p>

                <asp:Panel ID="pnlExpirou" runat="server" CssClass="alerta alerta-aviso" role="status" Visible="false">
                    Sua sessão expirou. Entre novamente.
                </asp:Panel>
                <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false">
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
                    <p class="login-etapa">Entrando como <strong><asp:Literal ID="litEmail" runat="server" /></strong>. Informe sua segunda senha.</p>
                    <label class="campo">
                        <span>Segunda senha</span>
                        <asp:TextBox ID="txtSegundaSenha" runat="server" TextMode="Password" MaxLength="128" autocomplete="off" />
                    </label>
                    <asp:Button ID="btnEntrar" runat="server" Text="Entrar" CssClass="botao" OnClick="btnEntrar_Click" />
                    <asp:LinkButton ID="btnVoltar" runat="server" CssClass="link-voltar" OnClick="btnVoltar_Click">Usar outra conta</asp:LinkButton>
                </asp:Panel>

                <asp:Panel ID="pnlDefinirSegundaSenha" runat="server" Visible="false" DefaultButton="btnDefinir">
                    <p class="login-etapa">Este é o seu primeiro acesso à gestão. Crie uma segunda senha, usada só aqui.</p>
                    <label class="campo"><span>Nova segunda senha</span><asp:TextBox ID="txtNovaSegundaSenha" runat="server" TextMode="Password" MaxLength="128" autocomplete="new-password" /></label>
                    <label class="campo"><span>Confirme a segunda senha</span><asp:TextBox ID="txtConfirmaSegundaSenha" runat="server" TextMode="Password" MaxLength="128" autocomplete="new-password" /></label>
                    <asp:Button ID="btnDefinir" runat="server" Text="Criar senha e entrar" CssClass="botao" OnClick="btnDefinir_Click" />
                    <asp:LinkButton ID="btnVoltarDefinicao" runat="server" CssClass="link-voltar" OnClick="btnVoltar_Click">Usar outra conta</asp:LinkButton>
                </asp:Panel>
                <asp:Panel ID="pnlPreviaLocal" runat="server" CssClass="alerta alerta-aviso" Visible="false">
                    <p>Modo de desenvolvimento local</p>
                    <asp:Button ID="btnPreviaLocal" runat="server" Text="Abrir prévia do painel" CssClass="botao-secundario" OnClick="btnPreviaLocal_Click" />
                </asp:Panel>
                <p class="login-rodape">Sua empresa ainda não está no Elo? Cadastre pelo aplicativo Elo.</p>
            </main>
        </div>
    </form>
</body>
</html>
