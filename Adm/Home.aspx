<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Adm_Home" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Visão geral | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlAviso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litAviso" runat="server" /></asp:Panel>

    <asp:Panel ID="pnlTrocaEmpresa" runat="server" CssClass="troca-empresa" Visible="false">
        <label for="<%= ddlEmpresas.ClientID %>">Empresa</label>
        <asp:DropDownList ID="ddlEmpresas" runat="server" CssClass="controle controle-curto" AutoPostBack="true" OnSelectedIndexChanged="ddlEmpresas_SelectedIndexChanged" />
    </asp:Panel>

    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente, ou cadastre sua empresa pelo aplicativo Elo.</p></asp:Panel>

    <asp:Panel ID="pnlDashboard" runat="server" Visible="false">
        <div class="cabecalho">
            <div><h1><asp:Literal ID="litEmpresa" runat="server" /></h1><p><asp:Literal ID="litCidade" runat="server" /></p></div>
            <div class="cabecalho-acoes"><asp:Literal ID="litStatus" runat="server" /><asp:Button ID="btnAtualizar" runat="server" Text="Atualizar" CssClass="botao-secundario botao-pequeno" OnClick="btnAtualizar_Click" /></div>
        </div>
        <dl class="resumo">
            <div class="indicador-perfil">
                <svg class="anel" viewBox="0 0 120 120" role="img" aria-label='<%= Feitos %> de <%= Total %> etapas do perfil concluídas'>
                    <% if (Feitos == Total) { %>
                    <line class="anel-raio" x1="104" y1="8" x2="99" y2="20" />
                    <line class="anel-raio" x1="116" y1="26" x2="107" y2="33" />
                    <% } %>
                    <circle class="anel-trilho" cx="60" cy="60" r="44" />
                    <circle class="anel-valor" cx="60" cy="60" r="44" pathLength="100" stroke-dasharray='<%= Percentual %> 100' />
                    <text class="anel-numero" x="60" y="72"><%= Feitos %>/<%= Total %></text>
                </svg>
                <div><dt>Perfil</dt><dd class="dd-texto"><%= Feitos == Total ? "Completo" : "Incompleto" %></dd></div>
            </div>
            <div><dt>Publicações criadas</dt><dd><asp:Literal ID="litConteudos" runat="server" /></dd></div>
        </dl>
        <div class="visao">
            <div>
                <section class="secao">
                    <div class="secao-cabecalho"><h2>Seu perfil</h2></div>
                    <asp:Repeater ID="rptPassos" runat="server">
                        <ItemTemplate>
                            <a class='passo <%# (bool)Eval("Feito") ? "passo-feito" : "" %>' href='<%# ResolveUrl(Convert.ToString(Eval("Url"))) %>'>
                                <span class="passo-marca"><svg class="icone" viewBox="0 0 24 24" aria-hidden="true"><path d="m5 12.5 4.5 4.5L19 7.5" /></svg></span>
                                <span class="passo-texto"><strong><%# HttpUtility.HtmlEncode(Eval("Titulo")) %></strong><small><%# HttpUtility.HtmlEncode(Eval("Detalhe")) %></small></span>
                                <span class="passo-acao"><%# (bool)Eval("Feito") ? "Revisar" : "Completar" %></span>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                </section>
            </div>
            <div>
                <section class="secao">
                    <div class="secao-cabecalho"><h2>Como o cliente vê</h2></div>
                    <div class="card-cliente">
                        <span class="card-capa"><asp:Image ID="imgCapa" runat="server" AlternateText="" Visible="false" /></span>
                        <div class="card-corpo">
                            <span class="card-logo"><asp:Literal ID="litInicial" runat="server" /><asp:Image ID="imgLogo" runat="server" AlternateText="" Visible="false" /></span>
                            <strong><asp:Literal ID="litCardNome" runat="server" /></strong>
                            <small><asp:Literal ID="litCardLocal" runat="server" /></small>
                        </div>
                    </div>
                </section>
                <section class="secao">
                    <div class="secao-cabecalho"><h2>Atalhos</h2></div>
                    <a class="atalho" runat="server" href="~/Adm/PublicacoesDashboard.aspx"><strong>Ver publicações</strong><small>O que está no ar e o que já passou</small></a>
                    <a class="atalho" runat="server" href="~/Adm/Produtos.aspx"><strong>Cadastrar produto</strong><small>Preço, foto e disponibilidade</small></a>
                    <a class="atalho" runat="server" href="~/Adm/Previa.aspx"><strong>Ver perfil completo</strong><small>Confira antes de divulgar</small></a>
                </section>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
