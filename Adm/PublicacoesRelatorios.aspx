<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="PublicacoesRelatorios.aspx.cs" Inherits="Adm_PublicacoesRelatorios" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Exportar publicações | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Exportar publicações</h1><p>Filtre por período, tipo e situação, e exporte para planilha.</p></div></div>
    <nav class="abas" aria-label="Relatório de publicações"><a runat="server" class="aba" href="~/Adm/PublicacoesDashboard.aspx">Painel</a><a runat="server" class="aba" href="~/Adm/Conteudos.aspx">Histórico</a><a runat="server" class="aba ativa" aria-current="page" href="~/Adm/PublicacoesRelatorios.aspx">Exportar</a></nav>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <section class="secao">
            <div class="campos">
                <label class="campo c2"><span>De</span><asp:TextBox ID="txtDe" runat="server" TextMode="Date" /></label>
                <label class="campo c2"><span>Até</span><asp:TextBox ID="txtAte" runat="server" TextMode="Date" /></label>
                <label class="campo c2"><span>Tipo</span><asp:DropDownList ID="ddlTipo" runat="server"><asp:ListItem Text="Todos" Value="" /><asp:ListItem Text="Stories" Value="Story" /><asp:ListItem Text="Promoções" Value="Promocao" /></asp:DropDownList></label>
                <label class="campo c2"><span>Situação</span><asp:DropDownList ID="ddlSituacao" runat="server"><asp:ListItem Text="Todas" Value="" /><asp:ListItem Text="Ativa" /><asp:ListItem Text="Programada" /><asp:ListItem Text="Encerrada" /><asp:ListItem Text="Suspensa" /><asp:ListItem Text="Rascunho" /></asp:DropDownList></label>
                <div class="acoes c4"><asp:Button ID="btnAplicar" runat="server" Text="Aplicar filtros" CssClass="botao-primario" OnClick="btnAplicar_Click" /><asp:Button ID="btnExportar" runat="server" Text="Exportar CSV" CssClass="botao-secundario" OnClick="btnExportar_Click" /></div>
            </div>
        </section>
        <section class="secao">
            <div class="secao-cabecalho"><h2><asp:Literal ID="litTotal" runat="server" /></h2></div>
            <asp:Repeater ID="rptItens" runat="server">
                <HeaderTemplate><div class="tabela-rolagem" tabindex="0"><table class="tabela"><thead><tr><th>Título</th><th>Tipo</th><th>Situação</th><th>Início</th><th>Término</th><th class="numero">Preço</th></tr></thead><tbody></HeaderTemplate>
                <ItemTemplate><tr>
                    <td><strong><%# HttpUtility.HtmlEncode(Eval("Titulo")) %></strong></td>
                    <td><%# Eval("TipoRotulo") %><%# (bool)Eval("Relampago") ? " relâmpago" : "" %></td>
                    <td><%# WheraOwnerTenant.PublicacoesTenant.Selo(Convert.ToString(Eval("Situacao"))) %></td>
                    <td><%# WheraOwnerTenant.PublicacoesTenant.DataHora((DateTime?)Eval("InicioUtc")) %></td>
                    <td><%# WheraOwnerTenant.PublicacoesTenant.DataHora((DateTime?)Eval("FimUtc")) %></td>
                    <td class="numero"><%# WheraOwnerTenant.PublicacoesTenant.PrecoTexto((decimal?)Eval("Preco")) %></td>
                </tr></ItemTemplate>
                <FooterTemplate></tbody></table></div></FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><p>Nenhuma publicação para estes filtros. Amplie o período ou limpe o tipo e a situação.</p></asp:Panel>
        </section>
    </asp:Panel>
</asp:Content>
