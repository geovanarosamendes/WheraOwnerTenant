<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Atendentes.aspx.cs" Inherits="Adm_Atendentes" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Atendentes | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Atendentes</h1><p>Pessoas que atendem seus clientes em <asp:Literal ID="litEmpresa" runat="server" />. Elas aparecem no perfil, mas não entram na gestão.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSemEmpresa" runat="server" CssClass="vazio" Visible="false"><h2>Nenhuma empresa vinculada a esta conta</h2><p>Peça ao dono da empresa para adicionar seu e-mail como gerente.</p></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <section class="secao">
            <div class="secao-cabecalho"><h2>Dados do atendente</h2></div>
            <div class="campos">
                <asp:HiddenField ID="hidId" runat="server" />
                <label class="campo c4"><span>Nome <sup>*</sup></span><asp:TextBox ID="txtNome" runat="server" MaxLength="100" /></label>
                <label class="campo c4"><span>Função</span><asp:TextBox ID="txtFuncao" runat="server" MaxLength="80" placeholder="Vendas, suporte" /></label>
                <label class="campo c4"><span>E-mail</span><asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="254" /></label>
                <label class="campo c4"><span>Telefone <sup>*</sup></span><asp:TextBox ID="txtTelefone" runat="server" MaxLength="20" /></label>
                <label class="campo c4"><span>WhatsApp <sup>*</sup></span><asp:TextBox ID="txtWhatsApp" runat="server" MaxLength="20" /></label>
                <p id="avisoContato" class="c12" role="alert" style="display:none;color:#c0392b;margin:0">Informe o telefone ou o WhatsApp. Sem contato, o atendente não aparece para os clientes.</p>
                <div class="acoes c4"><asp:Button ID="btnSalvar" runat="server" Text="Salvar atendente" CssClass="botao-primario" OnClick="btnSalvar_Click" /><asp:Button ID="btnNovo" runat="server" Text="Limpar" CssClass="botao-secundario" OnClick="btnNovo_Click" /></div>
            </div>
            <script>
                // Sem telefone nem WhatsApp, os campos ficam vermelhos e o botão não salva. A API confere de novo.
                (function () {
                    var telefone = document.getElementById("<%= txtTelefone.ClientID %>"), whatsapp = document.getElementById("<%= txtWhatsApp.ClientID %>"), salvar = document.getElementById("<%= btnSalvar.ClientID %>"), aviso = document.getElementById("avisoContato");
                    function conferir() {
                        var falta = !telefone.value.trim() && !whatsapp.value.trim();
                        [telefone, whatsapp].forEach(function (campo) { campo.style.borderColor = falta ? "#c0392b" : ""; campo.setAttribute("aria-invalid", falta ? "true" : "false"); });
                        salvar.disabled = falta;
                        salvar.style.backgroundColor = falta ? "#c0392b" : "";
                        salvar.style.borderColor = falta ? "#c0392b" : "";
                        aviso.style.display = falta ? "" : "none";
                    }
                    [telefone, whatsapp].forEach(function (campo) { campo.addEventListener("input", conferir); });
                    conferir();
                }());
            </script>
        </section>
        <section class="secao">
            <div class="secao-cabecalho"><h2>Ordem no perfil</h2></div>
            <asp:Repeater ID="rptAtendentes" runat="server">
                <HeaderTemplate><ul class="linhas"></HeaderTemplate>
                <ItemTemplate>
                    <li class="linha-item">
                        <span class="avatar" aria-hidden="true"><%# Inicial(Eval("NomePublico")) %></span>
                        <div class="linha-principal"><strong><%# HttpUtility.HtmlEncode(Eval("NomePublico")) %></strong><small><%# HttpUtility.HtmlEncode(Eval("Funcao")) %></small></div>
                        <small><%# HttpUtility.HtmlEncode(Meios(Eval("Telefone"), Eval("WhatsApp"))) %></small>
                        <div class="linha-acoes">
                            <asp:LinkButton ID="btnSubir" runat="server" CssClass="botao-secundario botao-pequeno botao-so-icone" CommandArgument='<%# "up|" + Eval("Id") %>' OnCommand="btnOrdem_Command" aria-label="Subir na ordem" title="Subir na ordem">↑</asp:LinkButton>
                            <asp:LinkButton ID="btnDescer" runat="server" CssClass="botao-secundario botao-pequeno botao-so-icone" CommandArgument='<%# "down|" + Eval("Id") %>' OnCommand="btnOrdem_Command" aria-label="Descer na ordem" title="Descer na ordem">↓</asp:LinkButton>
                            <asp:LinkButton ID="btnEditar" runat="server" CssClass="botao-secundario botao-pequeno" CommandArgument='<%# Eval("Id") %>' OnCommand="btnEditar_Command">Editar</asp:LinkButton>
                            <asp:LinkButton ID="btnRemover" runat="server" CssClass="botao-perigo botao-pequeno" CommandArgument='<%# Eval("Id") %>' OnCommand="btnRemover_Command" data-confirmar="Remover este atendente?" data-confirmar-acao="Remover">Remover</asp:LinkButton>
                        </div>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlVazio" runat="server" CssClass="vazio" Visible="false"><p>Nenhum atendente ainda. Adicione o primeiro no formulário acima.</p></asp:Panel>
        </section>
    </asp:Panel>
</asp:Content>
