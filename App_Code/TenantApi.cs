using System;
using System.Collections.Generic;
using System.Globalization;

namespace WheraOwnerTenant
{
    /// <summary>Contratos do painel do tenant. Em prévia local, as telas usam somente dados de desenvolvimento e não fazem chamadas externas.</summary>
    public static class TenantApi
    {
        public static PerfilEmpresa MinhaEmpresa(WheraApi api)
        {
            if (SessaoTenant.DevPreviewAtiva) return PreviewEmpresa();
            if (!string.IsNullOrWhiteSpace(SessaoTenant.EmpresaSelecionadaId))
            {
                try { return PerfilEmpresa(api, SessaoTenant.EmpresaSelecionadaId); }
                catch (WheraApiException ex)
                {
                    // Perdeu o acesso à empresa escolhida: volta para a empresa padrão da conta.
                    if (ex.StatusCode != 403 && ex.StatusCode != 404) throw;
                    SessaoTenant.SelecionarEmpresa(null);
                }
            }
            return api.Get<PerfilEmpresa>("api/perfis/minha-empresa");
        }

        public static PerfilEmpresa PerfilEmpresa(WheraApi api, string id)
        {
            if (SessaoTenant.DevPreviewAtiva) return PreviewEmpresa();
            return api.Get<PerfilEmpresa>("api/empresas/" + Uri.EscapeDataString(id) + "/perfil");
        }

        /// <summary>Empresas às quais a conta tem acesso. Para a sessão Owner, a API devolve todas.</summary>
        public static AcessoTenant MeuAcesso(WheraApi api)
        {
            if (SessaoTenant.DevPreviewAtiva) return new AcessoTenant { Empresas = new List<EmpresaAcessoTenant>() };
            return api.Get<AcessoTenant>("api/usuarios/me/acesso");
        }

        public static PerfilEmpresa AtualizarPerfil(WheraApi api, string id, object perfil)
        {
            if (SessaoTenant.DevPreviewAtiva) return PreviewEmpresa();
            return api.Put<PerfilEmpresa>("api/empresas/" + Uri.EscapeDataString(id) + "/perfil", perfil);
        }

        public static List<GerenteTenant> Gerentes(WheraApi api, string id)
        {
            if (SessaoTenant.DevPreviewAtiva) return new List<GerenteTenant>();
            return api.Get<List<GerenteTenant>>("api/empresas/" + Uri.EscapeDataString(id) + "/gerentes") ?? new List<GerenteTenant>();
        }

        public static GerenteTenant AdicionarGerente(WheraApi api, string id, string email)
        {
            if (SessaoTenant.DevPreviewAtiva) return new GerenteTenant { Id = Guid.NewGuid().ToString(), EmpresaId = id, Nome = "Gerente de prévia", Email = email, Papel = "Gerente", Ativo = true };
            return api.Post<GerenteTenant>("api/empresas/" + Uri.EscapeDataString(id) + "/gerentes", new { Email = email });
        }

        public static void RemoverGerente(WheraApi api, string id, string gerenteId)
        {
            if (SessaoTenant.DevPreviewAtiva) return;
            api.Delete("api/empresas/" + Uri.EscapeDataString(id) + "/gerentes/" + Uri.EscapeDataString(gerenteId));
        }

        public static ResultadoConteudos ListarConteudos(WheraApi api, string id, string tipo)
        {
            if (SessaoTenant.DevPreviewAtiva) return new ResultadoConteudos { Itens = new List<ConteudoTenant>(), HoraServidor = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture) };
            return api.Get<ResultadoConteudos>("api/empresas/" + Uri.EscapeDataString(id) + "/conteudos?tipo=" + Uri.EscapeDataString(tipo));
        }

        public static UsuarioTenant MeuUsuario(WheraApi api)
        {
            if (SessaoTenant.DevPreviewAtiva) return new UsuarioTenant { Id = "preview-owner", Nome = "Usuário de prévia", Email = "preview@local" };
            return api.Get<UsuarioTenant>("api/usuarios/me");
        }

        public static UsuarioTenant AtualizarUsuario(WheraApi api, string nome, string email)
        {
            if (SessaoTenant.DevPreviewAtiva) return new UsuarioTenant { Id = "preview-owner", Nome = nome, Email = email };
            return api.Put<UsuarioTenant>("api/usuarios/me", new { Nome = nome, Email = email });
        }

        public static void AlterarSenha(WheraApi api, string senhaAtual, string novaSenha)
        {
            if (SessaoTenant.DevPreviewAtiva) return;
            api.Patch<object>("api/usuarios/me/senha", new { SenhaAtual = senhaAtual, NovaSenha = novaSenha });
        }

        public static void AlterarSegundaSenha(WheraApi api, string atual, string nova)
        {
            if (SessaoTenant.DevPreviewAtiva) return;
            api.Patch<object>("api/usuarios/me/segunda-senha", new { SegundaSenhaAtual = atual, NovaSegundaSenha = nova });
        }

        public static List<AtendentePerfil> Atendentes(WheraApi api, string id) { if (SessaoTenant.DevPreviewAtiva) return new List<AtendentePerfil>(); return api.Get<List<AtendentePerfil>>(Rota(id, "atendentes")) ?? new List<AtendentePerfil>(); }
        public static AtendentePerfil AdicionarAtendente(WheraApi api, string id, object request) { if (SessaoTenant.DevPreviewAtiva) return new AtendentePerfil { Id = Guid.NewGuid().ToString(), NomePublico = "Atendente de prévia", Ativo = true }; return api.Post<AtendentePerfil>(Rota(id, "atendentes"), request); }
        public static AtendentePerfil AtualizarAtendente(WheraApi api, string empresaId, string atendenteId, object request) { if (SessaoTenant.DevPreviewAtiva) return new AtendentePerfil { Id = atendenteId, NomePublico = "Atendente de prévia", Ativo = true }; return api.Put<AtendentePerfil>(Rota(empresaId, "atendentes") + "/" + Uri.EscapeDataString(atendenteId), request); }
        public static void RemoverAtendente(WheraApi api, string empresaId, string atendenteId) { if (SessaoTenant.DevPreviewAtiva) return; api.Delete(Rota(empresaId, "atendentes") + "/" + Uri.EscapeDataString(atendenteId)); }
        public static List<AtendentePerfil> ReordenarAtendentes(WheraApi api, string id, List<string> ids) { if (SessaoTenant.DevPreviewAtiva) return new List<AtendentePerfil>(); return api.Put<List<AtendentePerfil>>(Rota(id, "atendentes") + "/ordem", new { Ids = ids }); }

        public static SemanaFuncionamento Semana(WheraApi api, string id) { if (SessaoTenant.DevPreviewAtiva) return PreviewSemana(); return api.Get<SemanaFuncionamento>(Rota(id, "horarios")); }
        public static SemanaFuncionamento DefinirSemana(WheraApi api, string id, object request) { if (SessaoTenant.DevPreviewAtiva) return PreviewSemana(); return api.Put<SemanaFuncionamento>(Rota(id, "horarios/semana"), request); }

        public static CanalPerfil AdicionarCanal(WheraApi api, string id, object request) { return api.Post<CanalPerfil>(Rota(id, "canais"), request); }
        public static CanalPerfil AtualizarCanal(WheraApi api, string empresaId, string canalId, object request) { return api.Put<CanalPerfil>(Rota(empresaId, "canais") + "/" + Uri.EscapeDataString(canalId), request); }
        public static void RemoverCanal(WheraApi api, string empresaId, string canalId) { api.Delete(Rota(empresaId, "canais") + "/" + Uri.EscapeDataString(canalId)); }
        public static List<CanalPerfil> Canais(WheraApi api, string id) { if (SessaoTenant.DevPreviewAtiva) return new List<CanalPerfil>(); return api.Get<List<CanalPerfil>>(Rota(id, "canais")) ?? new List<CanalPerfil>(); }

        public static ImagemPerfilEnviada EnviarImagemPerfil(WheraApi api, string empresaId, string nome, string tipo, string tipoImagem, byte[] conteudo)
        {
            if (SessaoTenant.DevPreviewAtiva) return new ImagemPerfilEnviada { Url = null };
            return api.EnviarArquivoComCampo<ImagemPerfilEnviada>(Rota(empresaId, "imagens"), "arquivo", nome, tipo, conteudo, "tipo", tipoImagem);
        }

        public static ResultadoImportacaoProdutos ImportarProdutos(WheraApi api, string id, string nome, string tipo, byte[] conteudo) { if (SessaoTenant.DevPreviewAtiva) return new ResultadoImportacaoProdutos { Ocorrencias = new List<OcorrenciaImportacaoProduto>() }; return api.EnviarArquivo<ResultadoImportacaoProdutos>(Rota(id, "produtos/importacao"), "arquivo", nome, tipo, conteudo); }

        /// <summary>A imagem principal do produto, ou a primeira, com o endereço completo. Null quando não há foto.</summary>
        public static string FotoPrincipal(ProdutoTenant produto)
        {
            if (produto.Imagens == null || produto.Imagens.Count == 0) return null;
            var imagem = produto.Imagens.Find(i => i.Principal) ?? produto.Imagens[0];
            return WheraApi.UrlMidia(imagem.Url);
        }

        public static List<ProdutoTenant> Produtos(WheraApi api, string id) { if (SessaoTenant.DevPreviewAtiva) return new List<ProdutoTenant>(); return api.Get<List<ProdutoTenant>>(Rota(id, "produtos")) ?? new List<ProdutoTenant>(); }
        public static ProdutoTenant CriarProduto(WheraApi api, string id, object request) { if (SessaoTenant.DevPreviewAtiva) return new ProdutoTenant { Id = Guid.NewGuid().ToString(), Nome = "Produto de prévia" }; return api.Post<ProdutoTenant>(Rota(id, "produtos"), request); }
        public static ProdutoTenant AtualizarProduto(WheraApi api, string empresaId, string produtoId, object request) { if (SessaoTenant.DevPreviewAtiva) return new ProdutoTenant { Id = produtoId, Nome = "Produto de prévia" }; return api.Put<ProdutoTenant>(Rota(empresaId, "produtos") + "/" + Uri.EscapeDataString(produtoId), request); }
        public static void ExcluirProduto(WheraApi api, string empresaId, string produtoId) { if (SessaoTenant.DevPreviewAtiva) return; api.Delete(Rota(empresaId, "produtos") + "/" + Uri.EscapeDataString(produtoId)); }
        public static void EnviarImagemProduto(WheraApi api, string empresaId, string produtoId, string nome, string tipo, byte[] conteudo) { if (SessaoTenant.DevPreviewAtiva) return; api.EnviarArquivo<ProdutoImagemTenant>(Rota(empresaId, "produtos") + "/" + Uri.EscapeDataString(produtoId) + "/imagens", "arquivo", nome, tipo, conteudo); }

        public static ResumoInteracoes Interacoes(WheraApi api, string id) { if (SessaoTenant.DevPreviewAtiva) return new ResumoInteracoes(); return api.Get<ResumoInteracoes>(Rota(id, "interacoes")); }
        public static PaginaComentarios Comentarios(WheraApi api, string id, int quantidade) { if (SessaoTenant.DevPreviewAtiva) return new PaginaComentarios { Itens = new List<ComentarioEmpresa>() }; return api.Get<PaginaComentarios>(Rota(id, "comentarios") + "?pagina=1&tamanhoPagina=" + quantidade); }
        public static List<CupomEmpresa> Cupons(WheraApi api, string id) { if (SessaoTenant.DevPreviewAtiva) return new List<CupomEmpresa>(); return api.Get<List<CupomEmpresa>>(Rota(id, "cupons")) ?? new List<CupomEmpresa>(); }

        /// <summary>Resgates da empresa, do mais recente para o mais antigo. cupomId e situacao vazios não filtram.</summary>
        public static PaginaResgatesCupom ResgatesCupons(WheraApi api, string id, string cupomId, string situacao, int pagina, int tamanho)
        {
            if (SessaoTenant.DevPreviewAtiva) return PreviewResgates(situacao);
            var rota = Rota(id, "cupons/resgates") + "?pagina=" + pagina + "&tamanhoPagina=" + tamanho;
            if (!string.IsNullOrEmpty(cupomId)) rota += "&cupomId=" + Uri.EscapeDataString(cupomId);
            if (!string.IsNullOrEmpty(situacao)) rota += "&situacao=" + Uri.EscapeDataString(situacao);
            return api.Get<PaginaResgatesCupom>(rota) ?? new PaginaResgatesCupom { Itens = new List<ResgateCupom>() };
        }

        private static PaginaResgatesCupom PreviewResgates(string situacao)
        {
            var agora = DateTime.UtcNow;
            Func<string, string, string, DateTime, DateTime?, DateTime, ResgateCupom> item = (nome, codigo, estado, resgatado, usado, expira) => new ResgateCupom
            {
                Id = Guid.NewGuid().ToString(), CupomId = "preview", CupomTitulo = "Cupom de prévia", Publico = "Seguidores", TipoDesconto = "Percentual", Valor = 10, Codigo = codigo, NomeCliente = nome, Situacao = estado,
                ResgatadoEm = resgatado.ToString("o", CultureInfo.InvariantCulture), UtilizadoEm = usado.HasValue ? usado.Value.ToString("o", CultureInfo.InvariantCulture) : null, ExpiraEm = expira.ToString("o", CultureInfo.InvariantCulture)
            };
            var todos = new List<ResgateCupom>
            {
                item("Cliente de prévia 1", "PREVIA01", "Pendente", agora.AddMinutes(-20), null, agora.AddMinutes(100)),
                item("Cliente de prévia 2", "PREVIA02", "Utilizado", agora.AddHours(-5), agora.AddHours(-4), agora.AddHours(-3)),
                item("Cliente de prévia 3", "PREVIA03", "Expirado", agora.AddDays(-1), null, agora.AddHours(-22))
            };
            var itens = string.IsNullOrEmpty(situacao) ? todos : todos.FindAll(r => r.Situacao == situacao);
            return new PaginaResgatesCupom { Itens = itens, Pagina = 1, TotalItens = itens.Count, TotalPaginas = itens.Count == 0 ? 0 : 1 };
        }

        public static CupomEmpresa CriarCupom(WheraApi api, string empresaId, string cupomId, object request) { if (SessaoTenant.DevPreviewAtiva) return new CupomEmpresa { Id = cupomId }; return api.Put<CupomEmpresa>(Rota(empresaId, "cupons") + "/" + Uri.EscapeDataString(cupomId), request); }

        private static PerfilEmpresa PreviewEmpresa()
        {
            return new PerfilEmpresa { Id = "00000000-0000-0000-0000-000000000001", NomeFantasia = "Empresa de prévia local", Slug = "empresa-previa-local", Descricao = "Dados exibidos somente no modo de desenvolvimento local.", Status = StatusEmpresa.Rascunho, Versao = 1, Endereco = new EnderecoPerfil { Cidade = "Ambiente local", Estado = "--" }, Canais = new List<CanalPerfil>(), Horarios = new List<HorarioPerfil>(), Atendentes = new List<AtendentePerfil>(), Imagens = new ImagensPerfil { Galeria = new List<string>() }, Catalogo = new List<ItemCatalogo>() };
        }

        private static SemanaFuncionamento PreviewSemana()
        {
            return new SemanaFuncionamento { Dias = new List<DiaFuncionamento> { new DiaFuncionamento { DiaSemana = "Segunda", Nome = "Segunda-feira", Fechado = true, Intervalos = new List<IntervaloFuncionamento>() }, new DiaFuncionamento { DiaSemana = "Terca", Nome = "Terça-feira", Fechado = true, Intervalos = new List<IntervaloFuncionamento>() }, new DiaFuncionamento { DiaSemana = "Quarta", Nome = "Quarta-feira", Fechado = true, Intervalos = new List<IntervaloFuncionamento>() }, new DiaFuncionamento { DiaSemana = "Quinta", Nome = "Quinta-feira", Fechado = true, Intervalos = new List<IntervaloFuncionamento>() }, new DiaFuncionamento { DiaSemana = "Sexta", Nome = "Sexta-feira", Fechado = true, Intervalos = new List<IntervaloFuncionamento>() }, new DiaFuncionamento { DiaSemana = "Sabado", Nome = "Sábado", Fechado = true, Intervalos = new List<IntervaloFuncionamento>() }, new DiaFuncionamento { DiaSemana = "Domingo", Nome = "Domingo", Fechado = true, Intervalos = new List<IntervaloFuncionamento>() } } };
        }

        private static string Rota(string empresaId, string recurso) { return "api/empresas/" + Uri.EscapeDataString(empresaId) + "/" + recurso; }
    }

    public class ProdutoTenant
    {
        public string Sku { get; set; }
        public int? Estoque { get; set; }
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal? Preco { get; set; }
        public bool Disponivel { get; set; }
        public bool Publicado { get; set; }
        public List<ProdutoImagemTenant> Imagens { get; set; }
    }

    public class ResultadoImportacaoProdutos
    {
        public int TotalLinhas { get; set; }
        public int Importados { get; set; }
        public int Ignorados { get; set; }
        public int ComErro { get; set; }
        public List<OcorrenciaImportacaoProduto> Ocorrencias { get; set; }
    }

    public class OcorrenciaImportacaoProduto
    {
        public int Linha { get; set; }
        public string Nome { get; set; }
        public string Situacao { get; set; }
        public string Mensagem { get; set; }
    }

    public class ProdutoImagemTenant
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public bool Principal { get; set; }
    }

    public class GerenteTenant
    {
        public string Id { get; set; }
        public string EmpresaId { get; set; }
        public string UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Papel { get; set; }
        public bool Ativo { get; set; }
    }

    public class ResultadoConteudos
    {
        public List<ConteudoTenant> Itens { get; set; }
        public string HoraServidor { get; set; }
    }

    public class ConteudoTenant
    {
        public string Id { get; set; }
        public string EmpresaId { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Condicoes { get; set; }
        public decimal? Preco { get; set; }
        public string MidiaCaminho { get; set; }
        public string MidiaTipo { get; set; }
        public string MidiaUrl { get; set; }
        public string Destino { get; set; }
        public int? DuracaoDias { get; set; }
        public string InicioEm { get; set; }
        public string FimEm { get; set; }
        public string Estado { get; set; }
        public int Versao { get; set; }
        public DateTime? CriadoEm { get; set; }
        public bool Relampago { get; set; }
        public string FundoTexto { get; set; }
        public int? TotalVisualizacoes { get; set; }
        public int? TotalToques { get; set; }
    }

    public class UsuarioTenant
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    public class AcessoTenant
    {
        public List<EmpresaAcessoTenant> Empresas { get; set; }
    }

    public class EmpresaAcessoTenant
    {
        public string Id { get; set; }
        public string NomeFantasia { get; set; }
        public string Status { get; set; }
        public string Vinculo { get; set; }
    }

    public class ImagemPerfilEnviada { public string Url { get; set; } }
}
