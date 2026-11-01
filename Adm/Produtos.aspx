<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Produtos.aspx.cs" Inherits="Adm_Produtos" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Produtos | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
  <div class="cabecalho-pagina"><div><span class="sobretitulo">MINHA EMPRESA</span><h1>Produtos</h1><p>Cadastre aqui os produtos da empresa. O gerente escolhe no aplicativo Elo quais aparecem no perfil.</p></div></div>
  <asp:Panel ID="pnlErro" runat="server" CssClass="erro" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
  <asp:Panel ID="pnlSucesso" runat="server" CssClass="sucesso-msg" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
  <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
    <div class="painel">
      <div class="painel-cabecalho"><h2><asp:Literal ID="litTituloFormulario" runat="server" Text="Novo produto" /></h2></div>
      <asp:HiddenField ID="hidProdutoId" runat="server" />
      <label class="campo"><span>Nome</span><asp:TextBox ID="txtNome" runat="server" MaxLength="160" /></label>
      <label class="campo"><span>Descrição</span><asp:TextBox ID="txtDescricao" runat="server" TextMode="MultiLine" Rows="3" MaxLength="2000" /></label>
      <label class="campo"><span>Preço (opcional)</span><asp:TextBox ID="txtPreco" runat="server" MaxLength="12" placeholder="Ex.: 29,90" /></label>
      <label class="campo"><span>Disponibilidade</span><asp:CheckBox ID="chkDisponivel" runat="server" Text=" Disponível para venda" Checked="true" /></label>
      <label class="campo"><span>Foto (JPG, PNG ou WebP, até 5 MB)</span><asp:FileUpload ID="fuImagem" runat="server" accept="image/jpeg,image/png,image/webp" /></label>
      <asp:Button ID="btnSalvar" runat="server" Text="Salvar produto" CssClass="botao-primario" OnClick="btnSalvar_Click" />
      <asp:Button ID="btnCancelar" runat="server" Text="Cancelar edição" CssClass="botao-secundario" OnClick="btnCancelar_Click" Visible="false" CausesValidation="false" />
    </div>
    <div class="painel">
      <div class="painel-cabecalho"><h2>Produtos cadastrados</h2></div>
      <asp:Panel ID="pnlVazio" runat="server" Visible="false"><p>Nenhum produto cadastrado ainda.</p></asp:Panel>
      <div class="tabela-rolagem">
        <asp:Repeater ID="rptProdutos" runat="server" OnItemCommand="rptProdutos_ItemCommand">
          <HeaderTemplate><table class="tabela"><thead><tr><th>Foto</th><th>Produto</th><th>Preço</th><th>Situação</th><th>Ações</th></tr></thead><tbody></HeaderTemplate>
          <ItemTemplate><tr>
            <td><%# Foto((WheraOwnerTenant.ProdutoTenant)Container.DataItem) %></td>
            <td><%# HttpUtility.HtmlEncode(Eval("Nome")) %></td>
            <td><%# Preco((WheraOwnerTenant.ProdutoTenant)Container.DataItem) %></td>
            <td><%# Situacao((WheraOwnerTenant.ProdutoTenant)Container.DataItem) %></td>
            <td>
              <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("Id") %>' CssClass="botao-secundario" CausesValidation="false">Editar</asp:LinkButton>
              <asp:LinkButton runat="server" CommandName="Excluir" CommandArgument='<%# Eval("Id") %>' CssClass="botao-perigo" CausesValidation="false" OnClientClick="return confirm('Excluir este produto?');">Excluir</asp:LinkButton>
            </td>
          </tr></ItemTemplate>
          <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
      </div>
    </div>
  </asp:Panel>
</asp:Content>
