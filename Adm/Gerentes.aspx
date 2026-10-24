<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Gerentes.aspx.cs" Inherits="Adm_Gerentes" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Usuários e acesso | Whera Tenant</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho-pagina"><div><span class="sobretitulo">CONFIGURAÇÕES</span><h1>Usuários e acesso</h1><p>Gerencie as pessoas autorizadas a operar a empresa.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="erro" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="sucesso-msg" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="painel" Visible="false"><h2>Nenhuma empresa vinculada</h2><p>Não há uma empresa autorizada para esta conta.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <div class="painel"><div class="painel-cabecalho"><h2><asp:Literal ID="litEmpresa" runat="server" /></h2><p>Proprietário: <asp:Literal ID="litProprietario" runat="server" /></p></div>
            <div class="gestao-linha"><label class="campo campo-largo"><span>E-mail do novo gerente</span><asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="254" placeholder="gerente@empresa.com" /></label><asp:Button ID="btnAdicionar" runat="server" Text="Adicionar gerente" CssClass="botao-primario" OnClick="btnAdicionar_Click" /></div>
        </div>
        <div class="painel"><div class="painel-cabecalho"><h2>Gerentes autorizados</h2><p>A API valida o vínculo e as permissões da conta.</p></div><asp:Repeater ID="rptGerentes" runat="server"><HeaderTemplate><div class="tabela-rolagem"><table class="tabela"><thead><tr><th>Nome</th><th>E-mail</th><th>Status</th><th></th></tr></thead><tbody></HeaderTemplate><ItemTemplate><tr><td><%# HttpUtility.HtmlEncode(Eval("Nome")) %></td><td><%# HttpUtility.HtmlEncode(Eval("Email")) %></td><td><%# (bool)Eval("Ativo") ? "Ativo" : "Inativo" %></td><td><asp:LinkButton ID="btnRemover" runat="server" CssClass="botao-perigo" CommandArgument='<%# Eval("Id") %>' OnCommand="btnRemover_Command" OnClientClick="return confirm('Remover este gerente?');">Remover</asp:LinkButton></td></tr></ItemTemplate><FooterTemplate></tbody></table></div></FooterTemplate></asp:Repeater><asp:Panel ID="pnlVazio" runat="server" CssClass="tabela-vazia" Visible="false">Nenhum gerente cadastrado.</asp:Panel></div>
    </asp:Panel>
</asp:Content>
