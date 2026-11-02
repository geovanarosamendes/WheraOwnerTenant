using System;
using System.Globalization;
using System.Text;
using System.Web;

namespace WheraOwnerTenant
{
    /// <summary>
    /// Telas do elo-mobile no passo a passo de validar um cupom, sempre com dados de exemplo.
    /// Ao mudar uma tela no app, atualize aqui: app/conta/cupons.tsx com features/coupons/CouponCard.tsx,
    /// app/gerente/index.tsx e app/gerente/validar.tsx. Os estilos ficam em CSS/previa-app.css.
    /// </summary>
    public static class PreviaValidacao
    {
        private const string Codigo = "A7K2P9", Cliente = "Ana Souza", Desconto = "10% OFF", Titulo = "Desconto na primeira compra", Condicoes = "Válido para compras na loja.";
        private static readonly CultureInfo Br = new CultureInfo("pt-BR");

        private static string H(string texto) { return HttpUtility.HtmlEncode(texto ?? ""); }
        private static string I(string codigo, string classe = "") { return "<span class=\"a-ic " + classe + "\" aria-hidden=\"true\">&#x" + codigo + ";</span>"; }
        private static string Cabecalho(string titulo) { return "<div class=\"a-cabecalho a-cabecalho-titulo\">" + I("f229") + "<strong>" + H(titulo) + "</strong><span></span></div>"; }
        private static string Botao(string classe, string icone, string texto) { return "<span class=\"a-botao " + classe + " largo\">" + (icone == null ? "" : I(icone)) + H(texto) + "</span>"; }
        private static string Quando(DateTime data) { return data.ToString("dd/MM/yyyy HH:mm", Br); }

        /// <summary>Desenho no formato de um QR Code, sem conteúdo real: serve só para ilustrar.</summary>
        private static string QrIlustrativo()
        {
            const int lado = 21;
            var svg = new StringBuilder("<svg class=\"a-qr\" viewBox=\"0 0 21 21\" role=\"img\" aria-label=\"QR Code ilustrativo\" shape-rendering=\"crispEdges\"><rect width=\"21\" height=\"21\" fill=\"#fff\" />");
            Func<int, int, bool> canto = (x, y) => (x < 8 && y < 8) || (x > 12 && y < 8) || (x < 8 && y > 12);
            for (var y = 0; y < lado; y++)
                for (var x = 0; x < lado; x++)
                    if (!canto(x, y) && (x * 7 + y * 13 + x * y) % 3 == 0) svg.Append("<rect x=\"").Append(x).Append("\" y=\"").Append(y).Append("\" width=\"1\" height=\"1\" fill=\"#111827\" />");
            foreach (var par in new[] { new[] { 0, 0 }, new[] { 14, 0 }, new[] { 0, 14 } })
                svg.Append("<path fill=\"#111827\" fill-rule=\"evenodd\" d=\"M").Append(par[0]).Append(' ').Append(par[1]).Append("h7v7h-7zM").Append(par[0] + 1).Append(' ').Append(par[1] + 1).Append("v5h5v-5zM").Append(par[0] + 2).Append(' ').Append(par[1] + 2).Append("h3v3h-3z\" />");
            return svg.Append("</svg>").ToString();
        }

        public static string TelaCliente(string empresa)
        {
            return "<div class=\"a-tela a-com-margem\">" + Cabecalho("Meus cupons")
                + "<div class=\"a-ingresso\"><div class=\"a-ingresso-topo\"><b>" + Desconto + "</b><span>" + I("f4a6") + "Quem segue</span></div><div class=\"a-ingresso-corpo\"><p class=\"a-meta-pequena\">" + H(empresa) + "</p><strong>" + Titulo + "</strong><p class=\"a-meta-pequena\">" + Condicoes + "</p>"
                + "<div class=\"a-codigo a-toque\">" + QrIlustrativo() + "<b>" + Codigo + "</b><p class=\"a-meta-pequena\">Mostre este código na loja. Tempo para usar:</p><em>01:59:48</em></div></div></div></div>";
        }

        public static string TelaGestao(string empresa)
        {
            Func<string, string, string, bool, string> linha = (icone, titulo, sub, toque) => "<div class=\"a-linha" + (toque ? " a-toque" : "") + "\"><span class=\"a-linha-icone\">" + I(icone) + "</span><div class=\"a-flex\"><strong>" + titulo + "</strong>" + (sub == null ? "" : "<p class=\"a-meta\">" + sub + "</p>") + "</div>" + I("f23b", "a-mudo") + "</div>";
            return "<div class=\"a-tela a-com-margem\"><div class=\"a-cabecalho\">" + I("f229") + "</div><h3 class=\"a-titulo-grande\">" + H(empresa) + "</h3><p class=\"a-paragrafo\">Publicada no Elo</p>"
                + "<p class=\"a-grupo-titulo\">Criar</p><div class=\"a-grupo\">" + linha("f105", "Nova promoção", "Em formato de story, com a duração que você escolher", false) + linha("f31a", "Promoção relâmpago", "Até 24 horas, com contagem regressiva", false) + linha("f5db", "Novo cupom", "Para quem segue ou favorita", false) + "</div>"
                + "<p class=\"a-grupo-titulo\">Acompanhar</p><div class=\"a-grupo\">" + linha("f110", "Minhas promoções", null, false) + linha("f392", "Fotos da loja", "Fachada e galeria", false) + linha("f299", "Produtos no perfil", "Escolha quais aparecem", false) + linha("f4ee", "Cupons da empresa", null, false) + linha("f55a", "Validar cupom", "Confira o código do cliente", true) + "</div></div>";
        }

        public static string TelaCamera()
        {
            return "<div class=\"a-tela a-com-margem\">" + Cabecalho("Validar cupom") + "<div class=\"a-scanner-area\"><div class=\"a-camera a-toque\">" + QrIlustrativo() + "</div><p class=\"a-dica\">Aponte a câmera para o QR Code no celular do cliente.</p>" + Botao("contorno", "f3a7", "Digitar o código") + "</div></div>";
        }

        public static string TelaDigitar()
        {
            return "<div class=\"a-tela a-com-margem\">" + Cabecalho("Validar cupom") + "<div class=\"a-scanner-area\"><div class=\"a-camera a-camera-parada\">" + I("f4fd") + "</div><div class=\"a-campo-app\"><label>Código do cupom</label><span>" + Codigo + "</span></div><span class=\"a-botao cheio largo a-toque\">Conferir código</span>" + Botao("contorno", null, "Voltar para a câmera") + "</div></div>";
        }

        public static string TelaConferir(DateTime agora) { return Resultado(agora, false); }
        public static string TelaValidado(DateTime agora) { return Resultado(agora, true); }

        private static string Resultado(DateTime agora, bool validado)
        {
            var html = new StringBuilder("<div class=\"a-tela a-com-margem\">").Append(Cabecalho("Conferir cupom"));
            html.Append("<div class=\"a-conferir\"><div class=\"a-status-caixa").Append(validado ? " a-toque" : "").Append("\">").Append(I(validado ? "f21e" : "f578")).Append(validado ? "Cupom validado!" : "Cupom válido. Confira os dados antes de confirmar.").Append("</div>");
            html.Append("<div class=\"a-cartao a-pessoa-validar\"><span class=\"a-avatar-grande\">A</span><h3>").Append(Cliente).Append("</h3><div class=\"a-vinculos\"><span>").Append(I("f21e")).Append("Segue a loja</span><span>").Append(I("f21e")).Append("Favoritou a loja</span></div></div>");
            html.Append("<div class=\"a-cartao a-cupom-validar\"><b>").Append(Desconto).Append("</b><strong>").Append(Titulo).Append("</strong><span class=\"a-fato-rotulo\">Cupom para quem segue</span><p>").Append(Condicoes).Append("</p><hr />");
            html.Append(Fato("Resgatado em", Quando(agora.AddMinutes(-2)))).Append(Fato("Pode usar até", Quando(agora.AddHours(2)))).Append(Fato("Cupom criado em", Quando(agora.AddDays(-3)))).Append(Fato("Validade do cupom", Quando(agora.AddDays(-3)) + " a " + Quando(agora.AddDays(4)))).Append("</div></div>");
            if (validado) html.Append("<span class=\"a-botao cheio largo\">").Append(I("f4fd")).Append("Escanear outro cupom</span>");
            else html.Append("<span class=\"a-botao cheio largo a-toque\">").Append(I("f21d")).Append("Validar cupom</span>").Append(Botao("contorno", null, "Cancelar"));
            return html.Append("</div>").ToString();
        }

        private static string Fato(string rotulo, string valor) { return "<div class=\"a-fato\"><span class=\"a-fato-rotulo\">" + rotulo + "</span><strong>" + valor + "</strong></div>"; }
    }
}
