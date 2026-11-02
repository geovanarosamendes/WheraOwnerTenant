<%@ Page Language="C#" MasterPageFile="~/Adm/MasterPage.master" AutoEventWireup="true" CodeFile="Imagens.aspx.cs" Inherits="Adm_Imagens" %>
<asp:Content ID="TituloPagina" ContentPlaceHolderID="Titulo" runat="server">Imagens | Elo Gestão</asp:Content>
<asp:Content ID="CorpoPagina" ContentPlaceHolderID="Conteudo" runat="server">
    <div class="cabecalho"><div><h1>Imagens</h1><p>Cada imagem aparece em um lugar diferente. Veja ao lado de cada uma onde o cliente vai encontrá-la.</p></div></div>
    <asp:Panel ID="pnlErro" runat="server" CssClass="alerta alerta-erro" role="alert" Visible="false"><asp:Literal ID="litErro" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlSucesso" runat="server" CssClass="alerta alerta-sucesso" role="status" Visible="false"><asp:Literal ID="litSucesso" runat="server" /></asp:Panel>
    <asp:Panel ID="pnlConteudo" runat="server" data-alteracoes="" Visible="false">
        <section class="secao midia slot">
            <div class="midia-texto">
                <h2>Logo</h2>
                <p>A marca da sua empresa. Aparece em um círculo, sobre a capa e nas listas de empresas.</p>
                <p><strong>Use:</strong> o símbolo ou o nome da empresa, com fundo liso e centralizado.</p>
            </div>
            <div>
                <label class="slot-quadro slot-redondo">
                    <span class="slot-vazio">Adicionar logo</span>
                    <asp:Image ID="imgLogo" runat="server" AlternateText="Logo atual" />
                    <span class="slot-acao">Escolher imagem</span>
                    <asp:FileUpload ID="fuLogo" runat="server" accept="image/jpeg,image/png,image/webp" aria-label="Escolher logo" data-previa-arquivo="" />
                </label>
                <p class="slot-erro" role="alert"></p>
            </div>
            <figure class="guia">
                <div class="guia-tela">
                    <span class="guia-capa"></span>
                    <span class="guia-logo guia-destaque"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M5 8h12v6a5 5 0 0 1-5 5h-2a5 5 0 0 1-5-5zM17 10h1.500a2.500 2.500 0 0 1 0 5H17M8 2.500v2.500M11 2.500v2.500M14 2.500v2.500" /></svg></span>
                    <span class="guia-texto guia-texto-perfil"></span><span class="guia-texto guia-texto-curto"></span>
                </div>
                <figcaption>No perfil e nas listas</figcaption>
            </figure>
        </section>

        <section class="secao midia slot">
            <div class="midia-texto">
                <h2>Capa</h2>
                <p>O topo do seu perfil, a primeira coisa que o cliente vê ao abrir a página da empresa.</p>
                <p><strong>Use:</strong> uma foto na horizontal de dentro da empresa: o ambiente, os produtos ou a equipe.</p>
            </div>
            <div>
                <label class="slot-quadro slot-capa">
                    <span class="slot-vazio">Adicionar capa</span>
                    <asp:Image ID="imgCapa" runat="server" AlternateText="Capa atual" />
                    <span class="slot-acao">Escolher imagem</span>
                    <asp:FileUpload ID="fuCapa" runat="server" accept="image/jpeg,image/png,image/webp" aria-label="Escolher capa" data-previa-arquivo="" />
                </label>
                <p class="slot-erro" role="alert"></p>
            </div>
            <figure class="guia">
                <div class="guia-tela">
                    <img class="guia-capa guia-destaque" src='<%= ResolveUrl("~/Images/guia/exemplo-capa.jpg") %>' alt="" />
                    <span class="guia-logo"></span>
                    <span class="guia-texto guia-texto-perfil"></span><span class="guia-texto guia-texto-curto"></span>
                </div>
                <figcaption>Exemplo: dentro da loja, no topo do perfil</figcaption>
            </figure>
        </section>

        <section class="secao midia slot">
            <div class="midia-texto">
                <h2>Fachada</h2>
                <p>A frente da empresa, vista da rua. Aparece nos destaques e ajuda o cliente a reconhecer o local ao chegar.</p>
                <p><strong>Use:</strong> uma foto de fora, com a entrada e o letreiro visíveis.</p>
            </div>
            <div>
                <label class="slot-quadro slot-fachada">
                    <span class="slot-vazio">Adicionar fachada</span>
                    <asp:Image ID="imgFachada" runat="server" AlternateText="Fachada atual" />
                    <span class="slot-acao">Escolher imagem</span>
                    <asp:FileUpload ID="fuFachada" runat="server" accept="image/jpeg,image/png,image/webp" aria-label="Escolher fachada" data-previa-arquivo="" />
                </label>
                <p class="slot-erro" role="alert"></p>
            </div>
            <figure class="guia">
                <div class="guia-tela">
                    <img class="guia-fachada guia-destaque" src='<%= ResolveUrl("~/Images/guia/exemplo-fachada.jpg") %>' alt="" />
                    <span class="guia-texto guia-texto-card"></span><span class="guia-texto guia-texto-curto guia-texto-card2"></span>
                </div>
                <figcaption>Exemplo: a loja vista de fora, nos destaques</figcaption>
            </figure>
        </section>

        <section class="secao">
            <div class="secao-cabecalho"><h2>Galeria</h2><small><asp:Literal ID="litGaleriaTotal" runat="server" /></small><p>Mais fotos para o cliente conhecer o ambiente, os produtos e a equipe.</p></div>
            <div class="galeria">
                <asp:Repeater ID="rptGaleria" runat="server">
                    <ItemTemplate><img src="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Container.DataItem)) %>" alt="Foto da galeria" /></ItemTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlAdicionarGaleria" runat="server" CssClass="slot">
                    <label class="slot-quadro slot-galeria">
                        <span class="slot-vazio">Adicionar foto</span>
                        <img alt="" />
                        <span class="slot-acao">Escolher imagem</span>
                        <asp:FileUpload ID="fuGaleria" runat="server" accept="image/jpeg,image/png,image/webp" aria-label="Adicionar foto à galeria" data-previa-arquivo="" />
                    </label>
                    <p class="slot-erro" role="alert"></p>
                </asp:Panel>
            </div>
        </section>

        <div class="barra-salvar">
            <p data-aviso-alteracao="">JPG, PNG ou WebP de até 5 MB cada. Clique em um quadro ou arraste a imagem até ele.</p>
            <div class="acoes"><asp:Button ID="btnSalvar" runat="server" Text="Enviar imagens" CssClass="botao-primario" OnClick="btnSalvar_Click" /></div>
        </div>
    </asp:Panel>
</asp:Content>
