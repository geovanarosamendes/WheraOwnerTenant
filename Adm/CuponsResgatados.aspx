<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="CuponsResgatados.aspx.cs" Inherits="Adm_CuponsResgatados" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Cupons resgatados | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Cupons resgatados</h1><p>Resgates já usados ou ainda dentro do prazo, por cupom. Quem resgatou e não usou a tempo deixa de contar. <a runat="server" href="~/Adm/CuponsValidar.aspx">Veja como validar um cupom</a>.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <dl class="resumo">
            <div><dt>Resgates</dt><dd><asp:Literal ID="litTotal" runat="server" /></dd></div>
            <div><dt>Em cupons comuns</dt><dd><asp:Literal ID="litComuns" runat="server" /></dd></div>
            <div><dt>Em cupons VIP</dt><dd><asp:Literal ID="litVip" runat="server" /></dd></div>
            <div><dt>Cupons resgatados</dt><dd><asp:Literal ID="litCupons" runat="server" /></dd></div>
        </dl>
        <asp:Repeater ID="rptCupons" runat="server">
            <HeaderTemplate><div class="tabela-rolagem" tabindex="0"><table class="tabela"><thead><tr><th>Cupom</th><th>Para quem</th><th class="numero">Resgates</th><th class="numero">Vagas livres</th><th>Situação</th></tr></thead><tbody></HeaderTemplate>
            <ItemTemplate><tr>
                <td><strong><%# HttpUtility.HtmlEncode(Eval("Desconto")) %></strong><br /><small><%# HttpUtility.HtmlEncode(Eval("Titulo")) %></small></td>
                <td><%# HttpUtility.HtmlEncode(Eval("Publico")) %></td>
                <td class="numero"><%# Eval("Resgates") %></td>
                <td class="numero"><%# Eval("Vagas") %></td>
                <td><%# Eval("Selo") %></td>
            </tr></ItemTemplate>
            <FooterTemplate></tbody></table></div></FooterTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><h2>Nenhum cupom resgatado ainda</h2><p>Quando um cliente resgatar um cupom, ele aparece aqui.</p><a runat="server" class="botao-secundario" href="~/Adm/CuponsCriar.aspx">Criar cupom</a></asp:Panel>
    </asp:Panel>
</asp:Content>
