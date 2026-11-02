<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Horarios.aspx.cs" Inherits="Adm_Horarios" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Horários | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho">
        <div><h1>Horários</h1><p>Marque os dias em que a empresa abre e informe os horários de atendimento.</p></div>
    </div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSugestao" runat="server" CssClass="alerta alerta-aviso" Visible="false">Preenchemos com o horário mais comum: segunda a sexta, das 8h às 18h. Ajuste o que for preciso e salve para aparecer no seu perfil.</asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" data-alteracoes="" Visible="false">
        <section class="secao">
            <div class="secao-cabecalho"><h2>Semana</h2><button id="btnRepetirSegunda" type="button" class="botao-secundario botao-pequeno">Repetir segunda nos dias úteis</button></div>
            <div class="semana-cabecalho" aria-hidden="true"><span>Dia</span><span>Abre</span><span>Fecha</span><span>Almoço</span><span>Sai para o almoço</span><span>Volta do almoço</span></div>
            <asp:Repeater ID="rptDias" runat="server">
                <ItemTemplate>
                    <div class='dia <%# (bool)Eval("Aberto") ? "" : "fechado" %> <%# (bool)Eval("Almoco") ? "com-almoco" : "" %>' data-dia='<%# Eval("Dia") %>'>
                        <asp:HiddenField ID="hidDia" runat="server" Value='<%# Eval("Dia") %>' />
                        <asp:HiddenField ID="hidNome" runat="server" Value='<%# Eval("Nome") %>' />
                        <asp:HiddenField ID="hidFixos" runat="server" Value='<%# Eval("Fixos") %>' />
                        <div class="dia-nome"><asp:CheckBox ID="chkAberto" runat="server" Text='<%# Eval("Nome") %>' Checked='<%# Eval("Aberto") %>' /></div>
                        <%# Convert.ToString(Eval("Fixos")).Length > 0 ? "<p class=\"dia-fixos\">" + HttpUtility.HtmlEncode(Convert.ToString(Eval("Fixos")).Replace(";", ", ").Replace("-", " às ")) + ". Estes períodos foram mantidos: para alterar, use o site do Elo.</p>" : "" %>
                        <asp:Panel ID="pnlCampos" runat="server" CssClass="dia-campos" Visible='<%# Convert.ToString(Eval("Fixos")).Length == 0 %>'>
                            <label class="campo-dia"><span class="dia-rotulo">Abre</span><asp:TextBox ID="txtAbre" runat="server" TextMode="Time" Text='<%# Eval("Abre") %>' /></label>
                            <label class="campo-dia"><span class="dia-rotulo">Fecha</span><asp:TextBox ID="txtFecha" runat="server" TextMode="Time" Text='<%# Eval("Fecha") %>' /></label>
                            <div class="dia-almoco campo-opcao"><asp:CheckBox ID="chkAlmoco" runat="server" Text="Fecha para almoço" Checked='<%# Eval("Almoco") %>' /></div>
                            <label class="campo-dia campo-almoco"><span class="dia-rotulo">Sai para o almoço</span><asp:TextBox ID="txtSaida" runat="server" TextMode="Time" Text='<%# Eval("Saida") %>' /></label>
                            <label class="campo-dia campo-almoco"><span class="dia-rotulo">Volta do almoço</span><asp:TextBox ID="txtVolta" runat="server" TextMode="Time" Text='<%# Eval("Volta") %>' /></label>
                        </asp:Panel>
                        <span class="dia-status">Fechado</span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </section>
        <div class="barra-salvar">
            <p data-aviso-alteracao=""><asp:Literal ID="litAviso" runat="server" /></p>
            <div class="acoes"><asp:Button ID="btnSalvar" runat="server" Text="Salvar horários" CssClass="botao-primario" OnClick="btnSalvar_Click" /></div>
        </div>
    </asp:Panel>
</asp:Content>
