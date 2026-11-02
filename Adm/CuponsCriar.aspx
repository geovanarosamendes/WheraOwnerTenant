<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="CuponsCriar.aspx.cs" Inherits="Adm_CuponsCriar" ValidateRequestMode="Disabled" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Criar cupom | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Criar cupom</h1><p>O cupom fica disponível assim que for criado, para quem segue ou favorita a empresa.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" data-alteracoes="" Visible="false">
        <section class="secao">
            <div class="secao-cabecalho"><h2>Desconto</h2></div>
            <div class="campos">
                <label class="campo c4"><span>Quem pode resgatar</span><asp:DropDownList ID="ddlPublico" runat="server"><asp:ListItem Text="Comum: quem segue" Value="Seguidores" /><asp:ListItem Text="VIP: quem favorita" Value="Favoritos" /></asp:DropDownList></label>
                <label class="campo c4"><span>Tipo de desconto</span><asp:DropDownList ID="ddlTipo" runat="server"><asp:ListItem Text="Porcentagem (%)" Value="Percentual" /><asp:ListItem Text="Valor em reais (R$)" Value="ValorFixo" /></asp:DropDownList></label>
                <label class="campo c4"><span>Desconto</span><asp:TextBox ID="txtValor" runat="server" MaxLength="13" inputmode="decimal" placeholder="Ex.: 10 ou 12,50" /></label>
                <label class="campo c8"><span>Título</span><asp:TextBox ID="txtTitulo" runat="server" MaxLength="120" placeholder="Ex.: Desconto na primeira compra" /></label>
                <label class="campo"><span>Condições (opcional)</span><asp:TextBox ID="txtCondicoes" runat="server" TextMode="MultiLine" Rows="3" MaxLength="1000" /></label>
            </div>
        </section>
        <section class="secao">
            <div class="secao-cabecalho"><h2>Disponibilidade</h2></div>
            <div class="campos">
                <label class="campo c4"><span>Fica disponível por</span><asp:DropDownList ID="ddlDias" runat="server"><asp:ListItem Text="1 dia" Value="1" /><asp:ListItem Text="3 dias" Value="3" /><asp:ListItem Text="7 dias" Value="7" Selected="True" /><asp:ListItem Text="15 dias" Value="15" /><asp:ListItem Text="30 dias" Value="30" /></asp:DropDownList></label>
                <label class="campo c4"><span>Para quantas pessoas (opcional)</span><asp:TextBox ID="txtLimite" runat="server" MaxLength="7" inputmode="numeric" placeholder="Vazio: sem limite" /></label>
                <label class="campo c4"><span>Tempo para usar depois de resgatar</span><asp:DropDownList ID="ddlPrazo" runat="server"><asp:ListItem Text="Até o fim do cupom" Value="" /><asp:ListItem Text="30 minutos" Value="30" /><asp:ListItem Text="1 hora" Value="60" /><asp:ListItem Text="2 horas" Value="120" Selected="True" /><asp:ListItem Text="6 horas" Value="360" /><asp:ListItem Text="24 horas" Value="1440" /><asp:ListItem Text="3 dias" Value="4320" /><asp:ListItem Text="7 dias" Value="10080" /></asp:DropDownList></label>
            </div>
        </section>
        <div class="barra-salvar">
            <p data-aviso-alteracao="">Com limite de pessoas, escolha um tempo para usar: se alguém não usar nesse tempo, a vaga volta a ficar livre.</p>
            <div class="acoes"><a runat="server" class="botao-secundario" href="~/Adm/Cupons.aspx">Cancelar</a><asp:Button ID="btnCriar" runat="server" Text="Criar cupom" CssClass="botao-primario" OnClick="btnCriar_Click" /></div>
        </div>
    </asp:Panel>
</asp:Content>
