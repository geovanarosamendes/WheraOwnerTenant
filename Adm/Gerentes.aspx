<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Gerentes.aspx.cs" Inherits="Adm_Gerentes" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Gerentes | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Gerentes</h1><p>Pessoas que ajudam você a cuidar de <asp:Literal ID="litEmpresa" runat="server" />. Dono da empresa: <asp:Literal ID="litProprietario" runat="server" />.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Cadastre sua empresa pelo aplicativo Elo para adicionar gerentes.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <section class="secao">
            <div class="secao-cabecalho"><h2>Adicionar gerente</h2><p>A pessoa precisa ter uma conta no Elo com este e-mail.</p></div>
            <div class="campos">
                <label class="campo c6"><span>E-mail do gerente</span><asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="254" placeholder="gerente@empresa.com" /></label>
                <div class="acoes c3"><asp:Button ID="btnAdicionar" runat="server" Text="Adicionar gerente" CssClass="botao-primario" OnClick="btnAdicionar_Click" /></div>
            </div>
        </section>
        <section class="secao">
            <div class="secao-cabecalho"><h2>Quem tem acesso</h2></div>
            <asp:Repeater ID="rptGerentes" runat="server">
                <HeaderTemplate><ul class="linhas"></HeaderTemplate>
                <ItemTemplate>
                    <li class="linha-item">
                        <span class="avatar" aria-hidden="true"><%# Inicial(Eval("Nome")) %></span>
                        <div class="linha-principal"><strong><%# HttpUtility.HtmlEncode(Eval("Nome")) %></strong><small><%# HttpUtility.HtmlEncode(Eval("Email")) %></small></div>
                        <span class='selo <%# (bool)Eval("Ativo") ? "selo-sucesso" : "selo-neutro" %>'><%# (bool)Eval("Ativo") ? "Ativo" : "Inativo" %></span>
                        <div class="linha-acoes"><asp:LinkButton ID="btnRemover" runat="server" CssClass="botao-perigo botao-pequeno" CommandArgument='<%# Eval("Id") %>' OnCommand="btnRemover_Command" data-confirmar="Remover este gerente?" data-confirmar-acao="Remover">Remover</asp:LinkButton></div>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><p>Nenhum gerente ainda. Só você tem acesso à gestão desta empresa.</p></asp:Panel>
        </section>
    </asp:Panel>
</asp:Content>
