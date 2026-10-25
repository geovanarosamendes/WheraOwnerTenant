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
            if (SessaoTenant.EhOwner && !string.IsNullOrWhiteSpace(SessaoTenant.EmpresaSelecionadaId))
                return PerfilEmpresa(api, SessaoTenant.EmpresaSelecionadaId);
            return api.Get<PerfilEmpresa>("api/perfis/minha-empresa");
        }

        public static PerfilEmpresa PerfilEmpresa(WheraApi api, string id)
        {
            if (SessaoTenant.DevPreviewAtiva) return PreviewEmpresa();
            return api.Get<PerfilEmpresa>("api/empresas/" + Uri.EscapeDataString(id) + "/perfil");
        }

        public static ResultadoEmpresas ListarEmpresas(WheraApi api, string busca)
        {
            if (SessaoTenant.DevPreviewAtiva) return new ResultadoEmpresas { Itens = new List<EmpresaResumoTenant> { new EmpresaResumoTenant { Id = "00000000-0000-0000-0000-000000000001", NomeFantasia = "Empresa de prévia local", Slug = "empresa-previa-local", Modalidade = "Fisica", Status = StatusEmpresa.Rascunho } }, TotalItens = 1 };
            return api.Get<ResultadoEmpresas>("api/empresas?busca=" + Uri.EscapeDataString(busca ?? "") + "&pagina=1&tamanhoPagina=100");
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

        public static ConteudoTenant SalvarConteudo(WheraApi api, string empresaId, string id, object request)
        {
            if (SessaoTenant.DevPreviewAtiva) return new ConteudoTenant { Id = id, EmpresaId = empresaId, Tipo = "Story", Titulo = "Conteúdo de prévia", Estado = "Rascunho", Versao = 1, CriadoEm = DateTime.UtcNow };
            return api.Put<ConteudoTenant>("api/empresas/" + Uri.EscapeDataString(empresaId) + "/conteudos/" + Uri.EscapeDataString(id), request);
        }

        public static MidiaConteudo EnviarMidia(WheraApi api, string empresaId, string nome, string tipo, byte[] conteudo)
        {
            if (SessaoTenant.DevPreviewAtiva) return new MidiaConteudo { Caminho = "preview-local/" + nome, Tipo = "Imagem" };
            return api.EnviarArquivo<MidiaConteudo>("api/empresas/" + Uri.EscapeDataString(empresaId) + "/conteudos/midias", "arquivo", nome, tipo, conteudo);
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
    }

    public class MidiaConteudo
    {
        public string Caminho { get; set; }
        public string Tipo { get; set; }
    }

    public class UsuarioTenant
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    public class ResultadoEmpresas
    {
        public List<EmpresaResumoTenant> Itens { get; set; }
        public int TotalItens { get; set; }
    }

    public class EmpresaResumoTenant
    {
        public string Id { get; set; }
        public string NomeFantasia { get; set; }
        public string Slug { get; set; }
        public string Modalidade { get; set; }
        public string Status { get; set; }
    }

    public class ImagemPerfilEnviada { public string Url { get; set; } }
}
