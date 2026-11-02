using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WheraOwnerTenant
{
    /// <summary>
    /// Desenha as telas do elo-mobile em HTML, com os dados reais da empresa. O que a empresa ainda não usa fica
    /// no formato do componente, pontilhado e sem cor (.a-falta). Nenhum dado é inventado para preencher o espaço.
    /// Ao mudar uma tela no app, atualize aqui: loja = app/empresa/[slug].tsx, gerente = app/gerente/index.tsx,
    /// perfil = app/conta/empresa.tsx. Texto de usuário sempre passa por H() ou A().
    /// </summary>
    public static class PreviaTelas
    {
        private static readonly CultureInfo Br = new CultureInfo("pt-BR");
        private static string H(string texto) { return HttpUtility.HtmlEncode(texto ?? ""); }
        private static string A(string texto) { return HttpUtility.HtmlAttributeEncode(texto ?? ""); }
        private static string I(string codigo, string classe = "") { return "<span class=\"a-ic " + classe + "\" aria-hidden=\"true\">&#x" + codigo + ";</span>"; }
        private static string Barra(int largura, string classe = "") { return "<span class=\"a-barra " + classe + "\" style=\"width:" + largura + "%\"></span>"; }
        private static string Img(string url, string classe) { return "<img class=\"" + classe + "\" src=\"" + A(WheraApi.UrlMidia(url)) + "\" alt=\"\" />"; }
        /// <summary>Com corte salvo, mostra só o trecho escolhido, como FramedPhoto no app; sem ele, a foto cobre a área.</summary>
        private static string Foto(string url, EnquadramentoImagem quadro)
        {
            if (quadro == null) return Img(url, "a-cobre");
            Func<double, string> pct = valor => valor.ToString("0.###", CultureInfo.InvariantCulture) + "%";
            return "<img class=\"a-cobre\" style=\"width:" + pct(100 / quadro.Largura) + ";height:" + pct(100 / quadro.Altura) + ";left:" + pct(-quadro.X * 100 / quadro.Largura) + ";top:" + pct(-quadro.Y * 100 / quadro.Altura) + ";object-fit:fill\" src=\"" + A(WheraApi.UrlMidia(url)) + "\" alt=\"\" />";
        }

        private static string Dinheiro(decimal valor) { return valor.ToString("C2", Br); }
        private static string Nota(decimal valor) { return valor.ToString("0.0", Br); }
        private static string Plural(int total, string um, string varios) { return total + " " + (total == 1 ? um : varios); }

        private static string Estrelas(decimal valor, bool grande = false)
        {
            var html = new StringBuilder("<span class=\"a-estrelas" + (grande ? " grande" : "") + "\">");
            for (var estrela = 1; estrela <= 5; estrela++) html.Append(I(valor >= estrela ? "f595" : valor >= estrela - 0.5m ? "f596" : "f599"));
            return html.Append("</span>").ToString();
        }

        private static string Fundo(string cor)
        {
            var inicio = cor != null && Regex.IsMatch(cor, "^#[0-9a-fA-F]{6}$") ? cor : "#2563EB";
            return "background:linear-gradient(" + inicio + ",#111827)";
        }

        // ---------- Tela da loja (o que o cliente vê) ----------
        public static string Loja(PreviaDados d)
        {
            var e = d.Empresa;
            var html = new StringBuilder("<div class=\"a-tela\">");
            html.Append("<span class=\"a-voltar\">").Append(I("f229")).Append("</span>");
            html.Append(string.IsNullOrWhiteSpace(d.Capa)
                ? "<div class=\"a-capa a-falta\" data-recurso=\"capa\">" + I("f38f") + "</div>"
                : "<div class=\"a-capa\" data-recurso=\"capa\">" + Foto(d.Capa, d.RecorteCapa) + "</div>");
            html.Append("<div class=\"a-corpo\">");

            var status = d.Status;
            html.Append(status == null
                ? "<div class=\"a-pilula a-falta\" data-recurso=\"horarios\">" + I("f5de") + Barra(100, "fixa") + "</div>"
                : "<div class=\"a-pilula " + (status.Aberto ? "ok" : "erro") + "\" data-recurso=\"horarios\">" + I("f5de") + H(status.Rotulo) + "</div>");

            var noAr = d.NoAr;
            html.Append("<div class=\"a-identidade\">").Append(LogoComAnel(d.Logo, noAr, 64, "logo")).Append("<div class=\"a-flex\"><h3 class=\"a-nome\">").Append(H(e.NomeFantasia)).Append("</h3>");
            var linha = PreviaDados.Juntar(" · ", d.Categoria, e.Cidade);
            html.Append(string.IsNullOrWhiteSpace(d.Categoria) || string.IsNullOrWhiteSpace(e.Cidade)
                ? "<p class=\"a-meta a-falta-texto\" data-recurso=\"identidade\">" + (linha.Length > 0 ? H(linha) + " " : "") + Barra(45, "fixa") + "</p>"
                : "<p class=\"a-meta\" data-recurso=\"identidade\">" + H(linha) + "</p>");
            html.Append("</div></div>");

            var resumo = d.Resumo;
            var media = resumo == null ? null : resumo.MediaAvaliacoesWhera;
            var total = resumo == null ? 0 : resumo.TotalAvaliacoes;
            html.Append("<div class=\"a-linha-nota\" data-recurso=\"avaliacoes\">").Append(Estrelas(media ?? 0)).Append("<span class=\"a-meta\">")
                .Append(total > 0 && media != null ? Nota(media.Value) + " · " + Plural(total, "avaliação", "avaliações") : "Ainda sem avaliações").Append("</span></div>");
            html.Append(d.NotaGoogle != null
                ? "<div class=\"a-linha-nota\" data-recurso=\"google\">" + I("f3f5", "a-mudo") + "<span class=\"a-meta\">" + Nota(d.NotaGoogle.Value) + " (" + (e.Vitrine.TotalAvaliacoesGoogle ?? 0) + ")</span></div>"
                : "<div class=\"a-linha-nota a-falta-texto\" data-recurso=\"google\">" + I("f3f5") + Barra(22, "fixa") + "</div>");

            html.Append(string.IsNullOrWhiteSpace(e.Descricao)
                ? "<div class=\"a-descricao a-falta-texto\" data-recurso=\"descricao\">" + Barra(100) + Barra(92) + Barra(60) + "</div>"
                : "<p class=\"a-descricao\" data-recurso=\"descricao\">" + H(e.Descricao) + "</p>");

            var tags = d.Tags;
            html.Append("<div class=\"a-tags\" data-recurso=\"etiquetas\">");
            if (tags.Count > 0) foreach (var tag in tags) html.Append("<span class=\"a-tag\">").Append(H(tag)).Append("</span>");
            else html.Append("<span class=\"a-tag a-falta\"></span><span class=\"a-tag a-falta\"></span><span class=\"a-tag a-falta\"></span>");
            html.Append("</div>");

            var semPublico = resumo == null || resumo.Seguidores + resumo.Favoritados == 0;
            html.Append("<div class=\"a-relacao\">");
            html.Append("<div class=\"a-relacao-coluna\"><span class=\"a-botao cheio\">").Append(I("f103")).Append("Seguir</span>").Append(Publico(resumo == null ? "–" : resumo.Seguidores.ToString(), "seguidores", semPublico)).Append("</div>");
            html.Append("<div class=\"a-relacao-coluna\"><span class=\"a-botao contorno\">").Append(I("f377")).Append("Favoritar</span>").Append(Publico(resumo == null ? "–" : resumo.Favoritados.ToString(), "favoritos", semPublico)).Append("</div></div>");

            html.Append("<div class=\"a-rapidos\" data-recurso=\"contatos\">");
            foreach (var tipo in new[] { "WhatsApp", "Instagram", "Telefone", "Mapa" })
                html.Append("<div class=\"a-rapido").Append(d.TemCanal(tipo) ? "" : " a-falta").Append("\">").Append(I(d.IconeCanal(tipo))).Append("<span>").Append(H(d.RotuloCanal(tipo))).Append("</span></div>");
            html.Append("</div>");

            html.Append(Rail(d, noAr));
            html.Append(Galeria(d));
            html.Append(Cupons(d));
            html.Append(Catalogo(d));
            html.Append(d.TemCanal("Site") ? "<span class=\"a-botao cheio largo\" data-recurso=\"site\">" + I("f350") + "Visitar site da loja</span>" : "<span class=\"a-botao largo a-falta\" data-recurso=\"site\">" + I("f350") + Barra(40, "fixa") + "</span>");
            html.Append(e.LojaVirtual != null ? "<span class=\"a-botao contorno largo\" data-recurso=\"loja-virtual\">" + I("f15f") + "Ver loja virtual</span>" : "<span class=\"a-botao largo a-falta\" data-recurso=\"loja-virtual\">" + I("f15f") + Barra(35, "fixa") + "</span>");
            html.Append("<div class=\"a-espaco\"></div>");
            html.Append(Endereco(d));
            html.Append(Horarios(d, status));
            html.Append(Equipe(d));
            html.Append(Informacoes(d));
            html.Append(Avaliacoes(d));
            return html.Append("</div></div>").ToString();
        }

        private static string Publico(string valor, string rotulo, bool falta)
        {
            return "<div class=\"a-publico" + (falta ? " a-falta" : "") + "\" data-recurso=\"seguidores\"><b>" + valor + "</b> <span>" + rotulo + "</span></div>";
        }

        private static string LogoComAnel(string logo, List<ConteudoTenant> noAr, int tamanho, string recurso)
        {
            var imagem = string.IsNullOrWhiteSpace(logo)
                ? "<span class=\"a-logo a-falta\" style=\"width:" + tamanho + "px;height:" + tamanho + "px\" data-recurso=\"" + recurso + "\">" + I("f5a8") + "</span>"
                : "<span class=\"a-logo\" style=\"width:" + tamanho + "px;height:" + tamanho + "px\" data-recurso=\"" + recurso + "\">" + Img(logo, "a-cobre") + "</span>";
            if (noAr.Count == 0) return imagem;
            var relampago = noAr.Any(c => c.Relampago);
            return "<span class=\"a-anel" + (relampago ? " relampago" : "") + "\">" + imagem + (relampago ? "<span class=\"a-raio\">" + I("f316") + "</span>" : "") + "</span>";
        }

        private static string Rail(PreviaDados d, List<ConteudoTenant> noAr)
        {
            var html = new StringBuilder("<div data-recurso=\"promocoes\">");
            if (noAr.Count == 0) return html.Append("<h4 class=\"a-titulo a-falta-titulo\">Promoções</h4><div class=\"a-faixa\"><div class=\"a-bolha\"><span class=\"a-bolha-anel a-falta\">").Append(I("f4ea")).Append("</span>").Append(Barra(80, "centro")).Append("</div></div></div>").ToString();
            var primeiro = noAr[0];
            var imagem = primeiro.MidiaTipo == "Imagem" && !string.IsNullOrWhiteSpace(primeiro.MidiaUrl) ? primeiro.MidiaUrl : d.Imagens.Logo;
            var relampago = noAr.Any(c => c.Relampago);
            html.Append("<h4 class=\"a-titulo\">Promoções</h4><div class=\"a-faixa\"><div class=\"a-bolha\"><span class=\"a-bolha-anel").Append(relampago ? " relampago-loja" : "").Append("\">")
                .Append(string.IsNullOrWhiteSpace(imagem) ? I("f4ea") : Img(imagem, "a-cobre")).Append("</span>").Append(relampago ? "<span class=\"a-raio-bolha nota\">" + I("f316") + "</span>" : "").Append("<small>").Append(H(d.Empresa.NomeFantasia)).Append("</small></div></div></div>");
            return html.ToString();
        }

        private static string Galeria(PreviaDados d)
        {
            var fotos = d.Galeria;
            var html = new StringBuilder("<div data-recurso=\"galeria\" class=\"a-secao\">");
            if (fotos.Count == 0)
            {
                html.Append("<h4 class=\"a-titulo a-falta-titulo\">Fotos da loja</h4><div class=\"a-faixa\">");
                for (var i = 0; i < 3; i++) html.Append("<span class=\"a-foto a-falta\">").Append(I("f38f")).Append("</span>");
                return html.Append("</div></div>").ToString();
            }
            html.Append("<div class=\"a-titulo-linha\"><h4 class=\"a-titulo\">Fotos da loja</h4><span class=\"a-link\">Ver todas</span></div><div class=\"a-faixa\">");
            foreach (var foto in fotos) html.Append("<span class=\"a-foto\">").Append(Foto(foto, d.Recorte(foto))).Append("</span>");
            return html.Append("</div></div>").ToString();
        }

        private static string Cupons(PreviaDados d)
        {
            var cupons = d.CuponsDisponiveis;
            if (cupons.Count == 0) return "<div class=\"a-cupom a-falta\" data-recurso=\"cupons\"><span class=\"a-cupom-icone\">" + I("f5da") + "</span><div class=\"a-flex\">" + Barra(70) + Barra(50) + "</div></div>";
            var vip = cupons.Count(c => c.Publico == "Favoritos");
            return "<div class=\"a-cupom ativo\" data-recurso=\"cupons\"><span class=\"a-cupom-icone\">" + I("f5da") + "</span><div class=\"a-flex\"><strong>" + Plural(cupons.Count, "cupom disponível", "cupons disponíveis") + "</strong><p class=\"a-meta\">" + Plural(cupons.Count - vip, "cupom", "cupons") + " · " + vip + " VIP para quem favorita</p></div>" + I("f23b", "a-acao") + "</div>";
        }

        private static string Catalogo(PreviaDados d)
        {
            var itens = d.Catalogo;
            var html = new StringBuilder("<div data-recurso=\"catalogo\" class=\"a-secao\">");
            if (itens.Count == 0)
            {
                html.Append("<h4 class=\"a-titulo a-falta-titulo\">Produtos e serviços</h4><div class=\"a-faixa\">");
                for (var i = 0; i < 3; i++) html.Append("<div class=\"a-item a-falta\"><span class=\"a-item-foto\">").Append(I("f299")).Append("</span><div class=\"a-item-corpo\">").Append(Barra(85)).Append(Barra(45)).Append("</div></div>");
                return html.Append("</div></div>").ToString();
            }
            html.Append("<h4 class=\"a-titulo\">Produtos e serviços</h4><div class=\"a-faixa\">");
            foreach (var item in itens)
            {
                html.Append("<div class=\"a-item\"><span class=\"a-item-foto\">").Append(string.IsNullOrWhiteSpace(item.Imagem) ? I("f299") : Img(item.Imagem, "a-cobre")).Append("</span><div class=\"a-item-corpo\"><strong>").Append(H(item.Titulo ?? "Produto")).Append("</strong>");
                if (item.Preco != null) html.Append("<b>").Append(Dinheiro(item.Preco.Value)).Append("</b>");
                if (item.TemLink) html.Append("<span class=\"a-link\">Ver produto</span>");
                html.Append("</div></div>");
            }
            return html.Append("</div></div>").ToString();
        }

        private static string Endereco(PreviaDados d)
        {
            if (!d.EnderecoVisivel) return "<div class=\"a-secao\" data-recurso=\"endereco\"><h4 class=\"a-titulo a-falta-titulo\">Onde encontrar</h4><div class=\"a-cartao a-falta\"><div class=\"a-endereco\"><span class=\"a-redondo\">" + I("f3c5") + "</span><div class=\"a-flex\">" + Barra(80) + Barra(55) + "</div></div><span class=\"a-botao largo a-falta\">" + I("f470") + Barra(45, "fixa") + "</span></div></div>";
            return "<div class=\"a-secao\" data-recurso=\"endereco\"><h4 class=\"a-titulo\">Onde encontrar</h4><div class=\"a-cartao\"><div class=\"a-endereco\"><span class=\"a-redondo suave\">" + I("f3c5") + "</span><div class=\"a-flex\"><strong>" + H(d.EnderecoLinha1.Length > 0 ? d.EnderecoLinha1 : "Consulte a localização no mapa") + "</strong>" + (d.EnderecoLinha2.Length > 0 ? "<p class=\"a-nota\">" + H(d.EnderecoLinha2) + "</p>" : "") + "</div>" + (d.EnderecoLinha1.Length + d.EnderecoLinha2.Length > 0 ? I("f290", "a-mudo") : "") + "</div>" + (d.TemMapa ? "<span class=\"a-botao contorno largo\">" + I("f470") + "Abrir no Google Maps</span>" : "") + "</div></div>";
        }

        private static string Horarios(PreviaDados d, StatusHorario status)
        {
            var html = new StringBuilder("<div class=\"a-secao\" data-recurso=\"horarios\">");
            if (status == null)
            {
                html.Append("<h4 class=\"a-titulo a-falta-titulo\">Horários</h4><div class=\"a-cartao a-falta\">").Append(Barra(60));
                for (var i = 0; i < 7; i++) html.Append("<div class=\"a-dia\">").Append(Barra(34, "fixa")).Append(Barra(26, "fixa")).Append("</div>");
                return html.Append("</div></div>").ToString();
            }
            html.Append("<h4 class=\"a-titulo\">Horários</h4><div class=\"a-cartao\"><div class=\"a-status-linha ").Append(status.Aberto ? "ok" : "erro").Append("\"><i></i><strong>").Append(H(status.Rotulo)).Append("</strong>").Append(I("f241", "a-mudo")).Append("</div>");
            foreach (var dia in d.Semana) html.Append("<div class=\"a-dia").Append(dia.Hoje ? " hoje" : "").Append(dia.Texto == "Fechado" ? " fechado" : "").Append("\"><span>").Append(H(dia.Rotulo)).Append("</span><span>").Append(H(dia.Texto)).Append("</span></div>");
            return html.Append("</div><p class=\"a-nota\">Horários informados pela empresa.</p></div>").ToString();
        }

        private static string Equipe(PreviaDados d)
        {
            var equipe = d.Equipe;
            if (equipe.Count == 0) return "<div class=\"a-secao\" data-recurso=\"equipe\"><div class=\"a-cartao a-falta\"><div class=\"a-endereco\"><span class=\"a-redondo\">" + I("f49f") + "</span><div class=\"a-flex\"><strong>Falar com a equipe</strong>" + Barra(45) + "</div>" + I("f232") + "</div></div></div>";
            var html = new StringBuilder("<div class=\"a-secao a-equipe\" data-recurso=\"equipe\" aria-expanded=\"false\"><div class=\"a-cartao\"><div class=\"a-endereco\"><span class=\"a-redondo suave\">").Append(I("f49f")).Append("</span><div class=\"a-flex\"><strong>Falar com a equipe</strong><p class=\"a-nota\">").Append(equipe.Count == 1 ? "1 atendente disponível" : equipe.Count + " atendentes disponíveis").Append("</p></div>").Append(I("f232", "a-mudo a-seta")).Append("</div><div class=\"a-equipe-lista\">");
            foreach (var pessoa in equipe)
            {
                html.Append("<div class=\"a-pessoa\"><span class=\"a-avatar\">").Append(I("f4a5")).Append("</span><div class=\"a-flex\"><strong>").Append(H(pessoa.NomePublico)).Append("</strong><p class=\"a-nota\">").Append(H(string.IsNullOrWhiteSpace(pessoa.Funcao) ? "Atendimento" : pessoa.Funcao)).Append("</p></div>");
                if (!string.IsNullOrWhiteSpace(pessoa.LinkWhatsApp)) html.Append("<span class=\"a-redondo suave\">").Append(I("f425")).Append("</span>");
                if (!string.IsNullOrWhiteSpace(pessoa.Telefone)) html.Append("<span class=\"a-redondo suave\">").Append(I("f1d9")).Append("</span>");
                html.Append("</div>");
            }
            return html.Append("</div></div></div>").ToString();
        }

        private static string Informacoes(PreviaDados d)
        {
            return "<div class=\"a-informacoes" + (d.TemInformacoes ? "" : " a-falta") + "\" data-recurso=\"informacoes\">" + I("f399", "a-mudo") + "<span class=\"a-flex\">Informações de contato</span>" + I("f23b", "a-mudo") + "</div>";
        }

        private static string Avaliacoes(PreviaDados d)
        {
            var resumo = d.Resumo;
            var media = resumo == null ? null : resumo.MediaAvaliacoesWhera;
            var totalAvaliacoes = resumo == null ? 0 : resumo.TotalAvaliacoes;
            var comentarios = d.Comentarios == null || d.Comentarios.Itens == null ? new List<ComentarioEmpresa>() : d.Comentarios.Itens;
            var totalComentarios = d.Comentarios == null ? 0 : d.Comentarios.TotalItens;
            var html = new StringBuilder("<div class=\"a-secao\" data-recurso=\"avaliacoes\"><h4 class=\"a-titulo\">Avaliações</h4><div class=\"a-cartao\"><div class=\"a-placar\"><span class=\"a-nota-grande\">");
            html.Append(media != null ? Nota(media.Value) : "–").Append("</span><div class=\"a-flex\">").Append(Estrelas(media ?? 0)).Append("<p class=\"a-nota\">").Append(totalAvaliacoes > 0 ? Plural(totalAvaliacoes, "avaliação", "avaliações") + " no Elo" : "Ainda sem avaliações no Elo").Append("</p></div></div>");
            if (d.NotaGoogle != null) html.Append("<p class=\"a-nota a-nota-google\">Google: ").Append(Nota(d.NotaGoogle.Value)).Append(" (").Append(d.Empresa.Vitrine.TotalAvaliacoesGoogle ?? 0).Append(")</p>");
            html.Append("<div class=\"a-avaliar\"><strong>Toque para avaliar</strong>").Append(Estrelas(0, true)).Append("</div></div>");
            html.Append("<h5 class=\"a-subtitulo\">Comentários").Append(totalComentarios > 0 ? " (" + totalComentarios + ")" : "").Append("</h5>");
            if (comentarios.Count == 0) html.Append("<div class=\"a-cartao a-comentario a-falta\">").Append(Barra(40)).Append(Barra(95)).Append(Barra(70)).Append("</div>");
            foreach (var comentario in comentarios.Take(3))
                html.Append("<div class=\"a-cartao a-comentario\"><div class=\"a-comentario-topo\"><strong>").Append(H(comentario.UsuarioNome)).Append("</strong><span class=\"a-nota\">").Append(comentario.CriadoEm.ToString("dd 'de' MMM 'de' yyyy", Br)).Append("</span></div><p>").Append(H(comentario.Texto)).Append("</p></div>");
            html.Append("<span class=\"a-botao contorno largo\">").Append(I("f215")).Append(totalComentarios > 0 ? "Ver todos" : "Escrever comentário").Append("</span>");
            return html.Append("</div>").ToString();
        }

        // ---------- Tela do gerente ----------
        public static string Gerente(PreviaDados d, List<RecursoPrevia> recursos)
        {
            var e = d.Empresa;
            Func<string, bool> usa = chave => recursos.First(r => r.Chave == chave).Situacao != SituacaoRecurso.Faltando;
            var html = new StringBuilder("<div class=\"a-tela a-com-margem\"><div class=\"a-cabecalho\">").Append(I("f229")).Append("</div>");
            html.Append("<h3 class=\"a-titulo-grande\">").Append(H(e.NomeFantasia)).Append("</h3><p class=\"a-paragrafo\">").Append(e.Status == StatusEmpresa.Publicada ? "Publicada no Elo" : "Situação: " + H(e.Status) + ". O que você criar só aparece depois da publicação.").Append("</p>");
            html.Append(Chips(d));
            html.Append("<p class=\"a-grupo-titulo\">Criar</p><div class=\"a-grupo\">");
            html.Append(LinhaGestao("g-promocao", "f105", "Nova promoção", "Em formato de story, com a duração que você escolher", usa));
            html.Append(LinhaGestao("g-relampago", "f31a", "Promoção relâmpago", "Até 24 horas, com contagem regressiva", usa));
            html.Append(LinhaGestao("g-cupom", "f5db", "Novo cupom", "Para quem segue ou favorita", usa));
            html.Append("</div><p class=\"a-grupo-titulo\">Acompanhar</p><div class=\"a-grupo\">");
            html.Append(LinhaGestao("g-resultados", "f110", "Minhas promoções", null, usa));
            html.Append(LinhaGestao("g-fotos", "f392", "Fotos da loja", "Fachada e galeria", usa));
            html.Append(LinhaGestao("g-produtos", "f299", "Produtos no perfil", "Escolha quais aparecem", usa));
            html.Append(LinhaGestao("g-cupons", "f4ee", "Cupons da empresa", null, usa));
            html.Append(LinhaGestao("g-validar", "f55a", "Validar cupom", "Confira o código do cliente", usa));
            return html.Append("</div><div class=\"a-recado\">Cadastro de produtos, dados da empresa, horários e gerentes ficam no painel de gestão.</div></div>").ToString();
        }

        private static string LinhaGestao(string chave, string icone, string titulo, string subtitulo, Func<string, bool> usa)
        {
            return "<div class=\"a-linha" + (usa(chave) ? "" : " a-linha-falta") + "\" data-recurso=\"" + chave + "\"><span class=\"a-linha-icone\">" + I(icone) + "</span><div class=\"a-flex\"><strong>" + H(titulo) + "</strong>" + (subtitulo == null ? "" : "<p class=\"a-meta\">" + H(subtitulo) + "</p>") + "</div>" + I("f23b", "a-mudo") + "</div>";
        }

        private static string Chips(PreviaDados d)
        {
            if (d.Empresas.Count < 2) return "";
            var html = new StringBuilder("<div class=\"a-faixa a-chips\">");
            foreach (var empresa in d.Empresas) html.Append("<span class=\"a-chip").Append(empresa.Id == d.Empresa.Id ? " ativo" : "").Append("\">").Append(H(empresa.NomeFantasia)).Append("</span>");
            return html.Append("</div>").ToString();
        }

        // ---------- Perfil da empresa ----------
        public static string Perfil(PreviaDados d, bool dono)
        {
            var e = d.Empresa;
            var promocoes = d.Promocoes ?? new List<ConteudoTenant>();
            var stories = d.Stories ?? new List<ConteudoTenant>();
            var ativas = d.PromocoesAtivas;
            var destaque = promocoes.FirstOrDefault() ?? stories.FirstOrDefault();
            var html = new StringBuilder("<div class=\"a-tela a-com-margem\"><div class=\"a-cabecalho\">").Append(I("f4aa")).Append("<span class=\"a-flex\"></span>").Append(I("f47f")).Append(I("f56c")).Append("</div>");
            html.Append(Chips(d));
            html.Append("<div class=\"a-identidade\">").Append(LogoComAnel(d.Logo, ativas, 72, "p-logo")).Append("<div class=\"a-flex\"><span class=\"a-sobretitulo\">").Append(dono ? "Perfil do dono" : "Perfil de gerente").Append("</span><h3 class=\"a-nome menor\">").Append(H(e.NomeFantasia)).Append("</h3>");
            var temIdentidade = !string.IsNullOrWhiteSpace(d.Categoria) && !string.IsNullOrWhiteSpace(e.Cidade);
            html.Append(temIdentidade ? "<p class=\"a-meta\" data-recurso=\"p-identidade\">" + H(d.Categoria + " · " + e.Cidade) + "</p>" : "<p class=\"a-meta a-falta-texto\" data-recurso=\"p-identidade\">" + Barra(70, "fixa") + "</p>").Append("</div>");
            html.Append("</div>");
            var imagemDestaque = destaque != null && destaque.MidiaTipo == "Imagem" ? destaque.MidiaUrl : null;
            html.Append("<span class=\"a-papel\">").Append(I("f1b2")).Append(dono ? "Dono desta empresa" : "Gerente desta empresa").Append("</span>");
            if (e.Status != StatusEmpresa.Publicada) html.Append("<p class=\"a-situacao\">Situação: ").Append(H(e.Status)).Append(". O que você criar só aparece depois da publicação.</p>");
            html.Append("<div class=\"a-acoes-perfil\"><span class=\"a-acao-perfil\">").Append(I("f1b2")).Append("Gestão da empresa</span>");
            html.Append("<div class=\"a-oferta\" data-recurso=\"p-oferta\"><span class=\"a-oferta-imagem").Append(destaque == null ? " a-falta" : "").Append("\">").Append(string.IsNullOrWhiteSpace(imagemDestaque) ? I("f103") : Img(imagemDestaque, "a-cobre")).Append("</span><small>Oferta</small></div>");
            html.Append("<span class=\"a-acao-perfil\">").Append(I("f488")).Append("Ver perfil público</span></div>");

            var resumo = d.Resumo;
            var usaIndicadores = resumo != null && resumo.Seguidores + resumo.Favoritados + resumo.TotalAvaliacoes + resumo.TotalComentarios > 0;
            html.Append("<div class=\"a-indicadores").Append(usaIndicadores ? "" : " a-falta").Append("\" data-recurso=\"p-indicadores\">");
            html.Append(Indicador("f4a3", resumo == null ? "–" : resumo.Seguidores.ToString(), "Seguidores")).Append(Indicador("f377", resumo == null ? "–" : resumo.Favoritados.ToString(), "Favoritos")).Append(Indicador("f170", resumo == null ? "–" : (resumo.TotalAvaliacoes + resumo.TotalComentarios).ToString(), "Interações")).Append("</div>");

            html.Append("<div class=\"a-secao\" data-recurso=\"p-ativas\">");
            if (ativas.Count == 0) html.Append("<h4 class=\"a-titulo-medio a-falta-titulo\">Ofertas ativas</h4><div class=\"a-faixa\"><div class=\"a-bolha\"><span class=\"a-bolha-anel a-falta\">").Append(I("f4ea")).Append("</span>").Append(Barra(80, "centro")).Append("</div><div class=\"a-bolha\"><span class=\"a-bolha-anel a-falta\">").Append(I("f4ea")).Append("</span>").Append(Barra(80, "centro")).Append("</div></div>");
            else
            {
                html.Append("<h4 class=\"a-titulo-medio\">Ofertas ativas</h4><div class=\"a-faixa\">");
                foreach (var oferta in ativas)
                {
                    html.Append("<div class=\"a-bolha\"><span class=\"a-bolha-anel").Append(oferta.Relampago ? " relampago" : "").Append("\">");
                    if (oferta.MidiaTipo == "Imagem" && !string.IsNullOrWhiteSpace(oferta.MidiaUrl)) html.Append(Img(oferta.MidiaUrl, "a-cobre"));
                    else if (!string.IsNullOrWhiteSpace(oferta.MidiaCaminho)) html.Append(I("f4ea"));
                    else html.Append("<span class=\"a-bolha-texto\" style=\"").Append(Fundo(oferta.FundoTexto)).Append("\">").Append(H(oferta.Titulo)).Append("</span>");
                    html.Append("</span><small>").Append(H(oferta.Titulo)).Append("</small></div>");
                }
                html.Append("</div>");
            }
            html.Append("</div>");

            html.Append("<div class=\"a-abas\" role=\"tablist\" aria-label=\"Conteúdo do perfil\">");
            foreach (var aba in new[] { new[] { "todas", "Todas" }, new[] { "produtos", "Produtos" }, new[] { "ofertas", "Ofertas" } })
                html.Append("<button type=\"button\" role=\"tab\" class=\"a-aba").Append(aba[0] == "produtos" ? " ativa" : "").Append("\" aria-selected=\"").Append(aba[0] == "produtos" ? "true" : "false").Append("\" data-aba-app=\"").Append(aba[0]).Append("\">").Append(aba[1]).Append("</button>");
            html.Append("</div>");
            html.Append(PainelComentarios(d)).Append(PainelProdutos(d)).Append(PainelOfertas(promocoes));
            return html.Append("</div>").ToString();
        }

        private static string Indicador(string icone, string valor, string rotulo)
        {
            return "<div class=\"a-indicador\">" + I(icone) + "<b>" + valor + "</b><span>" + rotulo + "</span></div>";
        }

        private static string PainelComentarios(PreviaDados d)
        {
            var itens = d.Comentarios == null || d.Comentarios.Itens == null ? new List<ComentarioEmpresa>() : d.Comentarios.Itens;
            var html = new StringBuilder("<div data-painel-app=\"todas\" data-recurso=\"p-comentarios\" hidden>");
            if (itens.Count == 0) return html.Append(CartaoVazio(true)).Append(CartaoVazio(true)).Append("</div>").ToString();
            foreach (var item in itens)
                html.Append("<div class=\"a-registro\"><span class=\"a-inicial\">").Append(H(string.IsNullOrEmpty(item.UsuarioNome) ? "" : item.UsuarioNome.Substring(0, 1).ToUpper(Br))).Append("</span><div class=\"a-flex\"><strong>").Append(H(item.UsuarioNome)).Append("</strong><p>").Append(H(item.Texto)).Append("</p><span class=\"a-meta\">").Append(item.CriadoEm.ToString("dd 'de' MMM 'de' yyyy", Br)).Append("</span></div></div>");
            return html.Append("</div>").ToString();
        }

        private static string CartaoVazio(bool avatar)
        {
            return "<div class=\"a-registro a-falta\">" + (avatar ? "<span class=\"a-inicial\"></span>" : "") + "<div class=\"a-flex\">" + Barra(35) + Barra(85) + Barra(50) + "</div></div>";
        }

        private static string PainelProdutos(PreviaDados d)
        {
            var produtos = d.Produtos ?? new List<ProdutoTenant>();
            var html = new StringBuilder("<div data-painel-app=\"produtos\" data-recurso=\"p-produtos\">");
            if (produtos.Count == 0) return html.Append(CartaoVazio(false)).Append(CartaoVazio(false)).Append("</div>").ToString();
            foreach (var p in produtos)
            {
                var descricao = string.IsNullOrWhiteSpace(p.Descricao) ? "Sem descrição." : p.Descricao;
                if (p.Preco != null) descricao += " · " + Dinheiro(p.Preco.Value);
                html.Append("<div class=\"a-registro\"><div class=\"a-flex\"><span class=\"a-estado acao\">").Append(p.Publicado && p.Disponivel ? "No perfil" : p.Publicado ? "Indisponível" : "Fora do perfil").Append("</span><strong>").Append(H(p.Nome)).Append("</strong><p class=\"a-meta a-duas\">").Append(H(descricao)).Append("</p></div>").Append(I("f23b", "a-mudo")).Append("</div>");
            }
            return html.Append("</div>").ToString();
        }

        private static string PainelOfertas(List<ConteudoTenant> ofertas)
        {
            var html = new StringBuilder("<div data-painel-app=\"ofertas\" data-recurso=\"p-ofertas\" hidden>");
            if (ofertas.Count == 0) return html.Append(CartaoVazio(false)).Append(CartaoVazio(false)).Append("</div>").ToString();
            foreach (var o in ofertas)
            {
                var estado = PreviaDados.EstadoConteudo(o);
                var texto = !string.IsNullOrWhiteSpace(o.Descricao) ? o.Descricao : !string.IsNullOrWhiteSpace(o.Condicoes) ? o.Condicoes : "Oferta publicada pela empresa.";
                html.Append("<div class=\"a-registro\"><div class=\"a-flex\"><span class=\"a-estado ").Append(estado == "Ativa" ? "ok" : "").Append("\">").Append(estado).Append("</span><strong>").Append(H(string.IsNullOrWhiteSpace(o.Titulo) ? "Promoção" : o.Titulo)).Append("</strong><p class=\"a-meta a-duas\">").Append(H(texto)).Append("</p></div>").Append(I("f23b", "a-mudo")).Append("</div>");
            }
            return html.Append("</div>").ToString();
        }
    }
}
