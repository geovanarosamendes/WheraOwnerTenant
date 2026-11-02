<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="MinhaEmpresa.aspx.cs" Inherits="Adm_MinhaEmpresa" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Dados da empresa | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho">
        <div><h1>Dados da empresa</h1><p>O que os clientes leem no seu perfil.</p></div>
        <asp:Literal ID="litStatus" runat="server" />
    </div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlFormulario" runat="server" data-alteracoes="" Visible="false">
        <section class="secao">
            <div class="secao-cabecalho"><h2>Apresentação</h2></div>
            <div class="campos">
                <label class="campo"><span>Nome da empresa <sup>*</sup></span><asp:TextBox ID="txtNome" runat="server" MaxLength="150" /></label>
                <label class="campo"><span>Sobre a empresa</span><asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="3" MaxLength="2000" /></label>
            </div>
        </section>
        <section class="secao">
            <div class="secao-cabecalho"><h2>Contato</h2></div>
            <div class="campos">
                <label class="campo c3"><span>WhatsApp</span><asp:TextBox ID="txtWhatsApp" runat="server" MaxLength="500" /></label>
                <label class="campo c4"><span>Instagram</span><asp:TextBox ID="txtInstagram" runat="server" MaxLength="500" placeholder="@empresa" /></label>
                <label class="campo c5"><span>Site</span><asp:TextBox ID="txtSite" runat="server" TextMode="Url" MaxLength="500" placeholder="https://" /></label>
            </div>
        </section>
        <section class="secao">
            <div class="secao-cabecalho"><h2>Endereço</h2></div>
            <div class="campos">
                <label class="campo c2"><span>CEP</span><asp:TextBox ID="txtCep" runat="server" MaxLength="9" /></label>
                <label class="campo c7"><span>Rua ou avenida</span><asp:TextBox ID="txtLogradouro" runat="server" MaxLength="200" /></label>
                <label class="campo c3"><span>Número</span><asp:TextBox ID="txtNumero" runat="server" MaxLength="20" /></label>
                <label class="campo c3"><span>Complemento</span><asp:TextBox ID="txtComplemento" runat="server" MaxLength="100" /></label>
                <label class="campo c3"><span>Bairro</span><asp:TextBox ID="txtBairro" runat="server" MaxLength="100" /></label>
                <label class="campo c4"><span>Cidade</span><asp:TextBox ID="txtCidade" runat="server" MaxLength="100" /></label>
                <label class="campo c2"><span>Estado (UF)</span><asp:TextBox ID="txtEstado" runat="server" MaxLength="2" /></label>
            </div>
        </section>
        <div class="barra-salvar">
            <p data-aviso-alteracao="">Tudo salvo.</p>
            <div class="acoes"><a runat="server" class="botao-secundario" href="~/Adm/Home.aspx">Voltar</a><asp:Button ID="btnSalvar" runat="server" Text="Salvar alterações" CssClass="botao-primario" OnClick="btnSalvar_Click" /></div>
        </div>
    </asp:Panel>
</asp:Content>
