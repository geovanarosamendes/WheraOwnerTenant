using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace WheraOwnerTenant
{
    public enum SituacaoRecurso { EmUso, Faltando, NaoVerificado }

    public class RecursoPrevia
    {
        public string Chave { get; set; }
        public string Nome { get; set; }
        public string OQueE { get; set; }
        public string Beneficio { get; set; }
        public SituacaoRecurso Situacao { get; set; }
        /// <summary>Tela do painel onde o recurso é configurado. Sem ela, vale o texto de Origem.</summary>
        public string Url { get; set; }
        public string Acao { get; set; }
        public string Origem { get; set; }
    }

    public class ContatoApp
    {
        public string Tipo { get; set; }
        public string Rotulo { get; set; }
        public string Detalhe { get; set; }
        public string Icone { get; set; }
        public bool Principal { get; set; }
        public int Ordem { get; set; }
    }

    public class ItemCatalogoApp { public string Imagem { get; set; } public string Titulo { get; set; } public decimal? Preco { get; set; } public bool TemLink { get; set; } }
    public class DiaApp { public string Rotulo { get; set; } public bool Hoje { get; set; } public string Texto { get; set; } }
    public class StatusHorario { public bool Aberto { get; set; } public string Rotulo { get; set; } }

    /// <summary>
    /// Tudo o que as telas do aplicativo mostram sobre a empresa. Cada fonte que falha fica null: o recurso
    /// aparece como "não verificado", nunca como "faltando", para não afirmar algo que não foi conferido.
    /// As regras seguem o elo-mobile (features/companies/media, contacts, hours e app/empresa/[slug]).
    /// </summary>
    public class PreviaDados
    {
        public PerfilEmpresa Empresa { get; set; }
        public List<ProdutoTenant> Produtos { get; set; }
        public ResumoInteracoes Resumo { get; set; }
        public List<CupomEmpresa> Cupons { get; set; }
        public List<ConteudoTenant> Promocoes { get; set; }
        public List<ConteudoTenant> Stories { get; set; }
        public PaginaComentarios Comentarios { get; set; }
        public List<EmpresaAcessoTenant> Empresas { get; set; }
        public DateTime Agora { get; set; }

        public static PreviaDados Carregar(WheraApi api, PerfilEmpresa empresa)
        {
            var dados = new PreviaDados { Empresa = empresa, Agora = AgoraBrasil() };
            var id = empresa.Id;
            dados.Produtos = Tentar(() => TenantApi.Produtos(api, id));
            dados.Resumo = Tentar(() => TenantApi.Interacoes(api, id));
            dados.Cupons = Tentar(() => TenantApi.Cupons(api, id));
            dados.Promocoes = Tentar(() => TenantApi.ListarConteudos(api, id, "Promocao").Itens ?? new List<ConteudoTenant>());
            dados.Stories = Tentar(() => TenantApi.ListarConteudos(api, id, "Story").Itens ?? new List<ConteudoTenant>());
            dados.Comentarios = Tentar(() => TenantApi.Comentarios(api, id, 15));
            var acesso = Tentar(() => TenantApi.MeuAcesso(api));
            dados.Empresas = acesso == null || acesso.Empresas == null ? new List<EmpresaAcessoTenant>() : acesso.Empresas;
            return dados;
        }

        private static T Tentar<T>(Func<T> chamada) where T : class
        {
            try { return chamada(); }
            catch (WheraApiException ex)
            {
                if (ex.StatusCode == 401) throw;
                return null;
            }
        }

        private static DateTime AgoraBrasil()
        {
            try { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")); }
            catch (TimeZoneNotFoundException) { return DateTime.Now; }
        }

        public ImagensPerfil Imagens { get { return Empresa.Imagens ?? new ImagensPerfil(); } }
        public List<string> Galeria { get { return (Imagens.Galeria ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList(); } }

        public string Capa { get { return Primeira(Imagens.FotoCapa, Imagens.FotoFachada, Galeria.FirstOrDefault()); } }
        public string Logo { get { return Primeira(Imagens.Logo, Imagens.FotoFachada); } }

        private static string Primeira(params string[] valores) { return valores.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)); }

        public string Categoria { get { return Empresa.Categoria == null ? null : Empresa.Categoria.Nome; } }

        /// <summary>Como em LocationSection: a seção aparece com qualquer parte do endereço ou com um link de mapa.</summary>
        public bool TemMapa { get { return TemCanal("Mapa") || EnderecoLinha1.Length > 0 || (Empresa.Endereco != null && Tem(Empresa.Endereco.GooglePlaceId)); } }
        public bool EnderecoVisivel { get { return EnderecoLinha1.Length > 0 || EnderecoLinha2.Length > 0 || TemMapa; } }

        /// <summary>O corte salvo para a foto. Valores fora do intervalo são ignorados e a foto fica centralizada.</summary>
        public EnquadramentoImagem Recorte(string url)
        {
            if (Imagens.Enquadramentos == null || !Tem(url)) return null;
            var quadro = Imagens.Enquadramentos.FirstOrDefault(x => x.Url == url);
            return quadro != null && quadro.Largura > 0 && quadro.Largura <= 1 && quadro.Altura > 0 && quadro.Altura <= 1 && quadro.X >= 0 && quadro.Y >= 0 ? quadro : null;
        }

        /// <summary>Como no topo da loja: sem foto de capa, é a fachada que aparece, no corte escolhido.</summary>
        public EnquadramentoImagem RecorteCapa { get { return Tem(Imagens.FotoCapa) ? null : Recorte(Imagens.FotoFachada); } }

        /// <summary>O link "Informações de contato" aparece com um canal preenchido, mesmo sem link, ou com endereço.</summary>
        public bool TemInformacoes { get { return (Empresa.Canais ?? new List<CanalPerfil>()).Any(c => Tem(c.Valor) || Tem(c.Link)) || EnderecoLinha1.Length > 0 || EnderecoLinha2.Length > 0; } }
        public string EnderecoLinha1 { get { var e = Empresa.Endereco; return e == null ? "" : Juntar(", ", e.Logradouro, e.Numero); } }
        public string EnderecoLinha2 { get { var e = Empresa.Endereco; return e == null ? "" : Juntar(" · ", e.Bairro, e.Complemento, e.Cidade); } }
        public static string Juntar(string separador, params string[] partes) { return string.Join(separador, partes.Where(x => !string.IsNullOrWhiteSpace(x))); }

        public List<AtendentePerfil> Equipe { get { return (Empresa.Atendentes ?? new List<AtendentePerfil>()).Where(a => !string.IsNullOrWhiteSpace(a.NomePublico)).OrderBy(a => a.Ordem).ToList(); } }
        public List<string> Tags { get { return Empresa.Vitrine == null || Empresa.Vitrine.Tags == null ? new List<string>() : Empresa.Vitrine.Tags.Take(4).ToList(); } }
        public decimal? NotaGoogle { get { return Empresa.Vitrine == null ? null : Empresa.Vitrine.NotaGoogle; } }

        // ---------- Contatos ----------
        private static readonly Dictionary<string, string[]> MetaCanais = new Dictionary<string, string[]>
        {
            { "WhatsApp", new[] { "WhatsApp", "f425", "1" } }, { "Telefone", new[] { "Ligar", "f1d9", "2" } }, { "Mapa", new[] { "Como chegar", "f470", "3" } },
            { "Instagram", new[] { "Instagram", "f3f9", "4" } }, { "Site", new[] { "Site", "f350", "5" } }, { "LojaVirtual", new[] { "Loja virtual", "f15f", "6" } },
            { "Marketplace", new[] { "Marketplace", "f5a8", "7" } }, { "OutraRedeSocial", new[] { "Rede social", "f350", "8" } }, { "EmailComercial", new[] { "E-mail", "f434", "9" } }
        };

        /// <summary>Só entra canal com link que o app aceita abrir, na mesma ordem do app.</summary>
        public List<ContatoApp> Contatos
        {
            get
            {
                var canais = Empresa.Canais ?? new List<CanalPerfil>();
                return canais.Where(c => c.Link != null && Regex.IsMatch(c.Link.Trim(), @"^(https?://|tel:|mailto:)\S+$", RegexOptions.IgnoreCase))
                    .Select((c, i) =>
                    {
                        var meta = c.Tipo != null && MetaCanais.ContainsKey(c.Tipo) ? MetaCanais[c.Tipo] : new[] { "Contato", "f350", "99" };
                        return new { Canal = c, Meta = meta, Indice = i };
                    })
                    .OrderByDescending(x => x.Canal.Principal).ThenBy(x => int.Parse(x.Meta[2], CultureInfo.InvariantCulture)).ThenBy(x => x.Indice)
                    .Select(x => new ContatoApp { Tipo = x.Canal.Tipo, Rotulo = x.Meta[0], Icone = x.Meta[1], Detalhe = x.Canal.Valor, Principal = x.Canal.Principal })
                    .ToList();
            }
        }

        public bool TemCanal(string tipo) { return Contatos.Any(c => c.Tipo == tipo); }
        public string RotuloCanal(string tipo) { return MetaCanais[tipo][0]; }
        public string IconeCanal(string tipo) { return MetaCanais[tipo][1]; }

        // ---------- Catálogo ----------
        /// <summary>Como no app: os produtos do perfil; sem nenhum, as fotos em destaque.</summary>
        public List<ItemCatalogoApp> Catalogo
        {
            get
            {
                var publicados = (Produtos ?? new List<ProdutoTenant>()).Where(p => p.Publicado && p.Disponivel).ToList();
                if (publicados.Count > 0) return publicados.Select(p => new ItemCatalogoApp { Imagem = TenantApi.FotoPrincipal(p), Titulo = p.Nome, Preco = p.Preco }).ToList();
                return (Empresa.Catalogo ?? new List<ItemCatalogo>()).Select(i => new ItemCatalogoApp { Imagem = WheraApi.UrlMidia(i.Imagem), Titulo = i.Titulo, TemLink = !string.IsNullOrWhiteSpace(i.Link) }).ToList();
            }
        }

        // ---------- Promoções e cupons ----------
        private static DateTime? Data(string valor)
        {
            DateTimeOffset data;
            if (string.IsNullOrWhiteSpace(valor) || !DateTimeOffset.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out data)) return null;
            return data.UtcDateTime;
        }

        public static string EstadoConteudo(ConteudoTenant c)
        {
            if (c.Estado == "Rascunho") return "Rascunho";
            var fim = Data(c.FimEm);
            if (c.Estado != "Publicada" || fim == null || fim <= DateTime.UtcNow) return "Encerrada";
            var inicio = Data(c.InicioEm);
            return inicio != null && inicio > DateTime.UtcNow ? "Agendada" : "Ativa";
        }

        public List<ConteudoTenant> PromocoesAtivas { get { return (Promocoes ?? new List<ConteudoTenant>()).Where(c => EstadoConteudo(c) == "Ativa").ToList(); } }
        /// <summary>Promoções e stories no ar, os relâmpagos primeiro.</summary>
        public List<ConteudoTenant> NoAr { get { return PromocoesAtivas.Concat((Stories ?? new List<ConteudoTenant>()).Where(c => EstadoConteudo(c) == "Ativa")).OrderByDescending(c => c.Relampago).ToList(); } }
        public bool FontesConteudoOk { get { return Promocoes != null && Stories != null; } }

        public List<CupomEmpresa> CuponsDisponiveis
        {
            get
            {
                return (Cupons ?? new List<CupomEmpresa>()).Where(c =>
                {
                    var fim = Data(c.FimEm); var inicio = Data(c.InicioEm);
                    return c.Estado == "Ativo" && fim != null && fim > DateTime.UtcNow && (inicio == null || inicio <= DateTime.UtcNow)
                        && (c.LimiteResgates == null || c.TotalResgates < c.LimiteResgates);
                }).ToList();
            }
        }

        // ---------- Horários ----------
        private static readonly string[] DiasApi = { "Domingo", "Segunda", "Terca", "Quarta", "Quinta", "Sexta", "Sabado" };
        private static readonly string[] DiasRotulo = { "Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "Sábado" };

        private List<HorarioPerfil> Horarios { get { return (Empresa.Horarios ?? new List<HorarioPerfil>()).Where(h => Array.IndexOf(DiasApi, h.DiaSemana) >= 0 && !string.IsNullOrEmpty(h.Abertura) && !string.IsNullOrEmpty(h.Fechamento)).ToList(); } }
        private static int Minutos(string valor) { var p = valor.Split(':'); return int.Parse(p[0], CultureInfo.InvariantCulture) * 60 + int.Parse(p[1], CultureInfo.InvariantCulture); }
        private static string Curta(string valor) { return valor.Length >= 5 ? valor.Substring(0, 5) : valor; }
        private static bool PassaMeiaNoite(HorarioPerfil h) { return Minutos(h.Fechamento) <= Minutos(h.Abertura); }

        public StatusHorario Status
        {
            get
            {
                var horarios = Horarios;
                if (horarios.Count == 0) return null;
                var hoje = (int)Agora.DayOfWeek;
                var atual = Agora.Hour * 60 + Agora.Minute;
                var rodando = horarios.FirstOrDefault(h =>
                {
                    var dia = Array.IndexOf(DiasApi, h.DiaSemana);
                    if (dia == hoje) return atual >= Minutos(h.Abertura) && (PassaMeiaNoite(h) || atual < Minutos(h.Fechamento));
                    return dia == (hoje + 6) % 7 && PassaMeiaNoite(h) && atual < Minutos(h.Fechamento);
                });
                if (rodando != null) return new StatusHorario { Aberto = true, Rotulo = "Aberto agora · fecha às " + Curta(rodando.Fechamento) };
                for (var deslocamento = 0; deslocamento < 7; deslocamento++)
                {
                    var dia = (hoje + deslocamento) % 7;
                    var proximo = horarios.Where(h => Array.IndexOf(DiasApi, h.DiaSemana) == dia && (deslocamento > 0 || Minutos(h.Abertura) > atual)).OrderBy(h => Minutos(h.Abertura)).FirstOrDefault();
                    if (proximo == null) continue;
                    var quando = deslocamento == 0 ? "hoje" : deslocamento == 1 ? "amanhã" : DiasRotulo[dia].ToLowerInvariant();
                    return new StatusHorario { Aberto = false, Rotulo = "Fechado · abre " + quando + " às " + Curta(proximo.Abertura) };
                }
                return new StatusHorario { Aberto = false, Rotulo = "Fechado" };
            }
        }

        /// <summary>A semana a partir de hoje, como no app.</summary>
        public List<DiaApp> Semana
        {
            get
            {
                var horarios = Horarios;
                var hoje = (int)Agora.DayOfWeek;
                return Enumerable.Range(0, 7).Select(deslocamento =>
                {
                    var dia = (hoje + deslocamento) % 7;
                    var periodos = horarios.Where(h => Array.IndexOf(DiasApi, h.DiaSemana) == dia).OrderBy(h => Minutos(h.Abertura)).Select(h => Curta(h.Abertura) + " às " + Curta(h.Fechamento)).ToList();
                    return new DiaApp { Rotulo = DiasRotulo[dia], Hoje = deslocamento == 0, Texto = periodos.Count > 0 ? string.Join(", ", periodos) : "Fechado" };
                }).ToList();
            }
        }

        // ---------- Recursos de cada tela ----------
        private const string PeloApp = "Crie pelo aplicativo Elo, em Gestão da empresa.";
        private const string PelosClientes = "Vem dos clientes: peça uma avaliação depois do atendimento.";
        private const string PelaEquipe = "Definido pela equipe do Elo. Fale com a gente para ativar.";

        private static SituacaoRecurso Sit(bool emUso) { return emUso ? SituacaoRecurso.EmUso : SituacaoRecurso.Faltando; }
        private static SituacaoRecurso Sit(object fonte, bool emUso) { return fonte == null ? SituacaoRecurso.NaoVerificado : Sit(emUso); }
        private static bool Tem(string texto) { return !string.IsNullOrWhiteSpace(texto); }

        private static RecursoPrevia R(string chave, string nome, string oQueE, string beneficio, SituacaoRecurso situacao, string url, string acao, string origem = null)
        {
            return new RecursoPrevia { Chave = chave, Nome = nome, OQueE = oQueE, Beneficio = beneficio, Situacao = situacao, Url = url, Acao = acao, Origem = origem };
        }

        public List<RecursoPrevia> RecursosLoja()
        {
            const string imagens = "~/Adm/Imagens.aspx", dadosEmpresa = "~/Adm/MinhaEmpresa.aspx";
            return new List<RecursoPrevia>
            {
                R("capa", "Foto de capa", "A imagem grande no topo da página da loja.", "É a primeira impressão: sem ela, o app usa uma imagem genérica da categoria.", Sit(Tem(Imagens.FotoCapa) || Tem(Imagens.FotoFachada)), imagens, "Enviar capa"),
                R("logo", "Logo", "O círculo com a marca, ao lado do nome.", "Faz o cliente reconhecer sua empresa na busca e nas promoções.", Sit(Tem(Imagens.Logo)), imagens, "Enviar logo"),
                R("horarios", "Horários", "O selo “Aberto agora” e a semana completa.", "O cliente sabe se pode ir agora, sem precisar perguntar.", Sit(Status != null), "~/Adm/Horarios.aspx", "Informar horários"),
                R("identidade", "Categoria e cidade", "A linha abaixo do nome da empresa.", "Mostra o que você faz e onde, e ajuda a aparecer nas buscas certas.", Sit(Tem(Categoria) && Tem(Empresa.Cidade)), dadosEmpresa, "Editar dados"),
                R("descricao", "Descrição", "O texto que apresenta a empresa.", "Explica em poucas linhas por que escolher você.", Sit(Tem(Empresa.Descricao)), dadosEmpresa, "Escrever descrição"),
                R("contatos", "WhatsApp e redes", "Os atalhos de WhatsApp, Instagram, ligação e mapa, no topo da tela.", "Quanto menos toques até falar com você, mais conversas começam.", Sit(TemCanal("WhatsApp") || TemCanal("Instagram")), dadosEmpresa, "Adicionar contatos"),
                R("promocoes", "Promoções", "Os stories da loja e o anel colorido na logo.", "Dá motivo para visitar hoje e destaca sua loja no início do app.", Sit(FontesConteudoOk ? (object)true : null, NoAr.Count > 0), null, null, PeloApp),
                R("galeria", "Fotos da loja", "Uma faixa de fotos do ambiente e do que você faz.", "Mostra como é o lugar e passa confiança antes da visita.", Sit(Galeria.Count > 0), imagens, "Enviar fotos"),
                R("cupons", "Cupons", "O cartão com os descontos disponíveis.", "Transforma quem segue ou favorita em cliente na loja.", Sit(Cupons, CuponsDisponiveis.Count > 0), "~/Adm/CuponsCriar.aspx", "Criar cupom"),
                R("catalogo", "Produtos e serviços", "A faixa com foto, nome e preço do que você vende.", "O cliente descobre o que você tem antes de sair de casa.", Sit(Produtos, Catalogo.Count > 0), "~/Adm/Catalogo.aspx", "Montar catálogo"),
                R("site", "Site da loja", "O botão “Visitar site da loja”.", "Leva quem quer saber mais para a sua página.", Sit(TemCanal("Site")), dadosEmpresa, "Informar site"),
                R("loja-virtual", "Loja virtual", "O botão “Ver loja virtual”.", "Abre um caminho de venda online para quem não pode ir até você.", Sit(Empresa.LojaVirtual != null), null, null, PelaEquipe),
                R("endereco", "Endereço", "A seção “Onde encontrar”, com botão para o Google Maps.", "Leva o cliente até a porta da loja.", Sit(EnderecoVisivel), dadosEmpresa, "Informar endereço"),
                R("equipe", "Atendentes", "A seção “Falar com a equipe”.", "O cliente fala direto com a pessoa certa, e a equipe ganha rosto e nome.", Sit(Equipe.Count > 0), "~/Adm/Atendentes.aspx", "Adicionar atendente"),
                R("informacoes", "Informações de contato", "O link que abre todos os contatos e o endereço, com opção de copiar.", "O cliente encontra telefone, e-mail e endereço por escrito, prontos para copiar.", Sit(TemInformacoes), dadosEmpresa, "Adicionar contatos"),
                R("avaliacoes", "Avaliações e comentários", "A nota da loja e o que os clientes dizem.", "Boas opiniões convencem quem ainda não conhece você.", Sit(Resumo, Resumo != null && Resumo.TotalAvaliacoes + Resumo.TotalComentarios > 0), null, null, PelosClientes),
                R("seguidores", "Seguidores e favoritos", "Os números abaixo dos botões Seguir e Favoritar.", "Quem segue fica sabendo das suas promoções e pode receber cupons.", Sit(Resumo, Resumo != null && Resumo.Seguidores + Resumo.Favoritados > 0), null, null, "Vem dos clientes: divulgue seu perfil no WhatsApp e na loja."),
                R("etiquetas", "Etiquetas", "Palavras curtas abaixo da descrição, como “Entrega”.", "Destacam diferenciais em uma olhada.", Sit(Tags.Count > 0), null, null, PelaEquipe),
                R("google", "Nota do Google", "A nota e o total de avaliações no Google.", "Aproveita a reputação que você já construiu fora do Elo.", Sit(NotaGoogle != null), null, null, PelaEquipe)
            };
        }

        public List<RecursoPrevia> RecursosGerente()
        {
            var promocoes = Promocoes ?? new List<ConteudoTenant>();
            return new List<RecursoPrevia>
            {
                R("g-promocao", "Nova promoção", "Cria uma oferta em formato de story, com a duração que você escolher.", "Coloca sua loja entre as promoções do início do app.", Sit(Promocoes, promocoes.Any(p => !p.Relampago && p.Estado != "Rascunho")), null, null, PeloApp),
                R("g-relampago", "Promoção relâmpago", "Oferta de até 24 horas, com contagem regressiva.", "Cria urgência e ajuda a girar estoque em um dia fraco.", Sit(Promocoes, promocoes.Any(p => p.Relampago)), null, null, PeloApp),
                R("g-cupom", "Novo cupom", "Desconto para quem segue ou favorita a empresa.", "Faz o cliente seguir sua loja e voltar para usar o desconto.", Sit(Cupons, Cupons != null && Cupons.Count > 0), "~/Adm/CuponsCriar.aspx", "Criar cupom"),
                R("g-resultados", "Minhas promoções", "Visualizações, toques e situação de cada promoção.", "Mostra quais ofertas chamam atenção e merecem se repetir.", Sit(Promocoes, promocoes.Any(p => p.Estado != "Rascunho")), null, null, "Aparece depois da primeira promoção no ar."),
                R("g-fotos", "Fotos da loja", "Envia a fachada e as fotos da galeria, com o corte de cada uma.", "Mantém as fotos atualizadas pelo celular, de dentro da loja.", Sit(Tem(Imagens.FotoFachada) || Galeria.Count > 0), "~/Adm/Imagens.aspx", "Enviar fotos"),
                R("g-produtos", "Produtos no perfil", "Liga e desliga os produtos que aparecem para o cliente.", "Mantém o perfil atualizado de qualquer lugar, pelo celular.", Sit(Produtos, Produtos != null && Produtos.Any(p => p.Publicado)), "~/Adm/ProdutosGaleria.aspx", "Abrir galeria"),
                R("g-cupons", "Cupons da empresa", "Lista os cupons, as vagas ocupadas e o prazo de cada um.", "Mostra quantos clientes cada desconto trouxe.", Sit(Cupons, Cupons != null && Cupons.Count > 0), "~/Adm/CuponsCriar.aspx", "Criar cupom"),
                R("g-validar", "Validar cupom", "Confere e baixa o código que o cliente mostra no caixa.", "Evita uso repetido e registra o resgate.", Sit(Cupons, Cupons != null && Cupons.Any(c => c.TotalResgates > 0)), "~/Adm/CuponsValidar.aspx", "Ver como validar")
            };
        }

        public List<RecursoPrevia> RecursosPerfil()
        {
            var ofertas = Promocoes ?? new List<ConteudoTenant>();
            return new List<RecursoPrevia>
            {
                R("p-logo", "Logo", "O círculo com a marca da empresa.", "Identifica a empresa que você está administrando.", Sit(Tem(Imagens.Logo)), "~/Adm/Imagens.aspx", "Enviar logo"),
                R("p-identidade", "Categoria e cidade", "A linha abaixo do nome da empresa.", "Confirma que o cadastro está certo.", Sit(Tem(Categoria) && Tem(Empresa.Cidade)), "~/Adm/MinhaEmpresa.aspx", "Editar dados"),
                R("p-oferta", "Atalho de oferta", "O círculo “Oferta”, com a promoção mais recente.", "Publica ou abre uma oferta em um toque.", Sit(FontesConteudoOk ? (object)true : null, ofertas.Count > 0 || (Stories != null && Stories.Count > 0)), null, null, PeloApp),
                R("p-indicadores", "Seguidores, favoritos e interações", "Os três números do perfil.", "Mostra se a sua presença no Elo está crescendo.", Sit(Resumo, Resumo != null && Resumo.Seguidores + Resumo.Favoritados + Resumo.TotalAvaliacoes + Resumo.TotalComentarios > 0), null, null, "Vem dos clientes: divulgue seu perfil no WhatsApp e na loja."),
                R("p-ativas", "Ofertas ativas", "As promoções no ar agora, em círculos.", "Você confere em um relance o que o cliente está vendo.", Sit(Promocoes, PromocoesAtivas.Count > 0), null, null, PeloApp),
                R("p-comentarios", "Comentários", "A aba “Todas”, com o que os clientes escreveram.", "Permite acompanhar a opinião dos clientes e melhorar o atendimento.", Sit(Comentarios, Comentarios != null && Comentarios.Itens != null && Comentarios.Itens.Count > 0), null, null, PelosClientes),
                R("p-produtos", "Produtos", "A aba “Produtos”, com o que está no perfil e o que está fora.", "Mostra de relance o que o cliente encontra.", Sit(Produtos, Produtos != null && Produtos.Count > 0), "~/Adm/Produtos.aspx", "Cadastrar produto"),
                R("p-ofertas", "Histórico de ofertas", "A aba “Ofertas”, com a situação de cada promoção.", "Ajuda a repetir o que funcionou.", Sit(Promocoes, ofertas.Count > 0), null, null, PeloApp)
            };
        }
    }
}
