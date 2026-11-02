<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="CuponsValidar.aspx.cs" Inherits="Adm_CuponsValidar" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Como validar um cupom | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <link href='<%= ResolveUrl("~/CSS/previa-app.css?v=6") %>' rel="stylesheet" type="text/css" />
    <div class="cabecalho"><div><h1>Como validar um cupom</h1><p>A validação é feita no aplicativo Elo, no atendimento. Veja o caminho, do cliente mostrando o código até a confirmação.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <div class="previa-app">
        <div class="previa-coluna-celular">
            <div class="celular-app"><asp:Literal ID="litTelas" runat="server" /></div>
            <div class="passos-navegacao">
                <button type="button" class="botao-secundario botao-pequeno" data-passo-anterior="">Anterior</button>
                <span aria-live="polite" data-passo-contagem=""></span>
                <button type="button" class="botao-secundario botao-pequeno" data-passo-proximo="">Próximo</button>
            </div>
        </div>
        <div class="previa-recursos">
            <p class="previa-legenda"><span class="previa-amostra previa-amostra-toque" aria-hidden="true"></span>Os dados são apenas um exemplo. O contorno azul mostra onde olhar ou tocar em cada tela.</p>
            <section class="secao">
                <div class="secao-cabecalho"><h2>Passo a passo</h2></div>
                <ol class="recursos passos"><asp:Literal ID="litPassos" runat="server" /></ol>
            </section>
            <section class="secao">
                <div class="secao-cabecalho"><h2>Se aparecer um aviso</h2><p>O cupom não é validado nesses casos.</p></div>
                <div class="tabela-rolagem" tabindex="0"><table class="tabela tabela-avisos"><thead><tr><th>Aviso no aplicativo</th><th>O que fazer</th></tr></thead><tbody>
                    <tr><td>Este cupom já foi usado em (data)</td><td>Não dê o desconto de novo. O código só vale uma vez.</td></tr>
                    <tr><td>O prazo da pessoa para usar este cupom terminou</td><td>O tempo depois do resgate acabou. Se ainda houver vagas, oriente o cliente a consultar os cupons disponíveis da loja.</td></tr>
                    <tr><td>Este cupom está vencido</td><td>A validade do cupom terminou. Veja se há outro cupom ativo em Cupons.</td></tr>
                    <tr><td>Este QR Code não é de um cupom do Elo</td><td>Peça ao cliente para abrir Meus cupons no app e mostrar o código de lá.</td></tr>
                    <tr><td>Confira o código digitado</td><td>Digite de novo, só com letras e números, ou leia o QR Code.</td></tr>
                    <tr><td>Código não encontrado para esta empresa</td><td>O cupom pode ser de outra loja. Confira o nome da empresa no cupom do cliente.</td></tr>
                </tbody></table></div>
            </section>
        </div>
    </div>
    <script src='<%= ResolveUrl("~/Scripts/validar-cupom.js?v=1") %>'></script>
</asp:Content>
