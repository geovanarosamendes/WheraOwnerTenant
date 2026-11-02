<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Cupons.aspx.cs" Inherits="Adm_Cupons" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Cupons | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Cupons</h1><p>Cupons comuns, para quem segue, e VIP, para quem favorita a empresa. Para encerrar ou validar, use o aplicativo Elo. <a runat="server" href="~/Adm/CuponsValidar.aspx">Veja como validar</a>.</p></div><div class="cabecalho-acoes"><a runat="server" class="botao-primario" href="~/Adm/CuponsCriar.aspx">Criar cupom</a></div></div>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <dl class="resumo">
            <div><dt>Disponíveis agora</dt><dd><asp:Literal ID="litDisponiveis" runat="server" /></dd></div>
            <div><dt>Comuns</dt><dd><asp:Literal ID="litComuns" runat="server" /></dd></div>
            <div><dt>VIP</dt><dd><asp:Literal ID="litVip" runat="server" /></dd></div>
            <div><dt>Resgates</dt><dd><asp:Literal ID="litResgates" runat="server" /></dd></div>
        </dl>
        <nav class="abas" aria-label="Tipo de cupom"><asp:Literal ID="litAbas" runat="server" /></nav>
        <asp:Repeater ID="rptCupons" runat="server">
            <HeaderTemplate><div class="tabela-rolagem" tabindex="0"><table class="tabela"><thead><tr><th>Cupom</th><th>Para quem</th><th class="numero">Resgates</th><th>Validade</th><th>Situação</th></tr></thead><tbody></HeaderTemplate>
            <ItemTemplate><tr>
                <td><strong><%# HttpUtility.HtmlEncode(Eval("Desconto")) %></strong><br /><small><%# HttpUtility.HtmlEncode(Eval("Titulo")) %></small></td>
                <td><%# HttpUtility.HtmlEncode(Eval("Publico")) %></td>
                <td class="numero"><%# HttpUtility.HtmlEncode(Eval("Resgates")) %></td>
                <td><%# HttpUtility.HtmlEncode(Eval("Validade")) %><br /><small><%# HttpUtility.HtmlEncode(Eval("Prazo")) %></small></td>
                <td><%# Eval("Selo") %></td>
            </tr></ItemTemplate>
            <FooterTemplate></tbody></table></div></FooterTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><h2><asp:Literal ID="litVazioTitulo" runat="server" /></h2><a runat="server" class="botao-secundario" href="~/Adm/CuponsCriar.aspx">Criar cupom</a></asp:Panel>
    </asp:Panel>
</asp:Content>
