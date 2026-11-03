<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="ProdutosImportar.aspx.cs" Inherits="Adm_ProdutosImportar" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Importar produtos | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Importar produtos</h1><p>Cadastre vários produtos de uma vez a partir de uma planilha. As fotos são adicionadas depois, em cada produto.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <section class="secao">
        <div class="secao-cabecalho"><h2>1. Baixe o modelo</h2><p>Preencha uma linha por produto, a partir da linha 2. Não renomeie as colunas: colunas com outro nome são ignoradas.</p></div>
        <div class="acoes">
            <a runat="server" class="botao-primario" href="~/Arquivos/modelo-produtos.xlsx" download="modelo-produtos.xlsx">Baixar modelo Excel</a>
            <a runat="server" class="botao-secundario" href="~/Arquivos/modelo-produtos.csv" download="modelo-produtos.csv">Baixar modelo CSV</a>
        </div>
        <div class="tabela-rolagem colunas-modelo" tabindex="0">
            <table class="tabela">
                <thead><tr><th>Coluna</th><th>Preenchimento</th><th>Exemplo</th></tr></thead>
                <tbody>
                    <tr><td><strong>nome</strong></td><td>Obrigatório, até 160 caracteres</td><td>Pão italiano</td></tr>
                    <tr><td><strong>descricao</strong></td><td>Opcional, até 2000 caracteres</td><td>Fermentação natural</td></tr>
                    <tr><td><strong>preco</strong></td><td>Opcional, de 0 a 99.999.999, com até 2 casas decimais</td><td>24,90 ou 1.234,50</td></tr>
                    <tr><td><strong>disponivel</strong></td><td>Sim ou não. Vazio vale sim. Com não, o produto entra inativo</td><td>sim</td></tr>
                    <tr><td><strong>no_catalogo</strong></td><td>Sim ou não. Vazio vale não. Com sim, disponivel não pode ser não e o estoque precisa ser maior que zero</td><td>não</td></tr>
                    <tr><td><strong>estoque</strong></td><td>Quantidade inteira. Sem estoque, o produto não entra no catálogo</td><td>10</td></tr>
                    <tr><td><strong>sku</strong></td><td>Opcional. Código do produto, sem repetir</td><td>BOLO-01</td></tr>
                </tbody>
            </table>
        </div>
    </section>
    <section class="secao">
        <div class="secao-cabecalho"><h2>2. Envie a planilha</h2><p>Arquivo .xlsx ou .csv de até 5 MB, com até 1000 produtos. O formato .xls antigo não é aceito. Se o nome já existir, a linha é ignorada, então reenviar a planilha não duplica produtos.</p></div>
        <div class="campos">
            <label class="campo c6"><span class="oculto-visual">Planilha</span><asp:FileUpload ID="fuPlanilha" runat="server" accept=".xlsx,.csv" /></label>
            <div class="acoes c3"><asp:Button ID="btnImportar" runat="server" Text="Importar produtos" CssClass="botao-primario" OnClick="btnImportar_Click" /></div>
        </div>
    </section>
    <asp:Panel ID="pnlResultado" runat="server" CssClass="secao" Visible="false">
        <div class="secao-cabecalho"><h2>Resultado</h2><a runat="server" class="botao-texto botao-pequeno" href="~/Adm/ProdutosGaleria.aspx?filtro=todos">Ver na galeria</a></div>
        <dl class="resumo">
            <div><dt>Linhas lidas</dt><dd><asp:Literal ID="litTotal" runat="server" /></dd></div>
            <div><dt>Importados</dt><dd><asp:Literal ID="litImportados" runat="server" /></dd></div>
            <div><dt>Ignorados</dt><dd><asp:Literal ID="litIgnorados" runat="server" /></dd></div>
            <div><dt>Com erro</dt><dd><asp:Literal ID="litErros" runat="server" /></dd></div>
        </dl>
        <asp:Repeater ID="rptOcorrencias" runat="server">
            <HeaderTemplate><div class="tabela-rolagem" tabindex="0"><table class="tabela"><thead><tr><th>Linha</th><th>Produto</th><th>Situação</th><th>O que fazer</th></tr></thead><tbody></HeaderTemplate>
            <ItemTemplate><tr>
                <td><%# Eval("Linha") %></td>
                <td><strong><%# HttpUtility.HtmlEncode(Eval("Nome")) %></strong></td>
                <td><span class='selo <%# Convert.ToString(Eval("Situacao")) == "Erro" ? "selo-erro" : "selo-neutro" %>'><%# HttpUtility.HtmlEncode(Eval("Situacao")) %></span></td>
                <td><%# HttpUtility.HtmlEncode(Eval("Mensagem")) %></td>
            </tr></ItemTemplate>
            <FooterTemplate></tbody></table></div></FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
</asp:Content>
