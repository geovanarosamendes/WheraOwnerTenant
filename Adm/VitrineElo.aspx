<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="VitrineElo.aspx.cs" Inherits="Adm_VitrineElo" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Vitrine Elo | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho">
        <div><h1>Vitrine Elo</h1><p>Uma página da sua empresa, com um link só seu, para vender fora do Elo.</p></div>
        <div class="cabecalho-acoes"><span class="selo selo-aviso">Em desenvolvimento</span><span class="selo selo-neutro">Serviço pago</span></div>
    </div>
    <section class="secao">
        <div class="secao-cabecalho"><h2>Como vai funcionar</h2></div>
        <ol class="etapas">
            <li><strong>Escolha os produtos</strong><span>Use os produtos que você já cadastrou no Elo.</span></li>
            <li><strong>Deixe com a cara da empresa</strong><span>Organize as seções, as cores e as imagens.</span></li>
            <li><strong>Compartilhe o link</strong><span>Envie pelo WhatsApp e coloque no Instagram.</span></li>
        </ol>
    </section>
    <section class="secao">
        <div class="secao-cabecalho"><h2>Montagem da Vitrine</h2><p>O editor ainda não está disponível, e nada foi cobrado.</p></div>
        <div class="acoes"><button type="button" class="botao-primario" disabled="disabled">Disponível em breve</button><a runat="server" class="botao-secundario" href="~/Adm/Catalogo.aspx">Voltar ao catálogo</a></div>
    </section>
</asp:Content>
