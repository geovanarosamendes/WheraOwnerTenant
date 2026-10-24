<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="MinhaEmpresa.aspx.cs" Inherits="Adm_MinhaEmpresa" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Minha empresa | Whera Tenant</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho-pagina"><div><span class="sobretitulo">MINHA EMPRESA</span><h1>Perfil da empresa</h1><p>Atualize as informações que aparecem no perfil público do Whera.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="erro" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="sucesso-msg" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="painel" Visible="false"><h2>Nenhuma empresa vinculada</h2><p>A API não retornou uma empresa autorizada para esta conta.</p></asp:Panel>
    <asp:Panel ID="pnlFormulario" runat="server" CssClass="painel" Visible="false">
        <div class="painel-cabecalho"><h2><asp:Literal ID="litNomeCabecalho" runat="server" /></h2><p>Status: <strong><asp:Literal ID="litStatus" runat="server" /></strong> · versão <asp:Literal ID="litVersao" runat="server" /></p></div>
        <div class="grade-form">
            <label class="campo"><span>Nome da empresa <sup>*</sup></span><asp:TextBox ID="txtNome" runat="server" MaxLength="150" /></label>
            <label class="campo"><span>Slug público</span><asp:TextBox ID="txtSlug" runat="server" Enabled="false" /></label>
            <label class="campo campo-largo"><span>Descrição</span><asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="4" MaxLength="2000" /></label>
            <label class="campo"><span>Site</span><asp:TextBox ID="txtSite" runat="server" TextMode="Url" MaxLength="500" placeholder="https://" /></label>
            <label class="campo"><span>WhatsApp</span><asp:TextBox ID="txtWhatsApp" runat="server" MaxLength="500" /></label>
            <label class="campo"><span>Instagram</span><asp:TextBox ID="txtInstagram" runat="server" MaxLength="500" placeholder="@empresa" /></label>
            <label class="campo"><span>CEP</span><asp:TextBox ID="txtCep" runat="server" MaxLength="9" /></label>
            <label class="campo"><span>Logradouro</span><asp:TextBox ID="txtLogradouro" runat="server" MaxLength="200" /></label>
            <label class="campo"><span>Número</span><asp:TextBox ID="txtNumero" runat="server" MaxLength="20" /></label>
            <label class="campo"><span>Bairro</span><asp:TextBox ID="txtBairro" runat="server" MaxLength="100" /></label>
            <label class="campo"><span>Cidade</span><asp:TextBox ID="txtCidade" runat="server" MaxLength="100" /></label>
            <label class="campo"><span>Estado</span><asp:TextBox ID="txtEstado" runat="server" MaxLength="2" /></label>
        </div>
        <div class="acoes-form"><asp:Button ID="btnSalvar" runat="server" Text="Salvar alterações" CssClass="botao-primario" OnClick="btnSalvar_Click" /><a class="botao botao-secundario" href="~/Adm/Home.aspx">Cancelar</a></div>
        <p class="dica">A API valida a permissão da empresa e a versão do perfil antes de gravar.</p>
    </asp:Panel>
</asp:Content>
