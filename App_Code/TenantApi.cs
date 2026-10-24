using System;
using System.Collections.Generic;
using System.Globalization;

namespace WheraOwnerTenant
{
    /// <summary>Contratos do painel do tenant. Todas as operações continuam passando pela API Whera.</summary>
    public static class TenantApi
    {
        public static PerfilEmpresa MinhaEmpresa(WheraApi api)
        {
            return api.Get<PerfilEmpresa>("api/perfis/minha-empresa");
        }

        public static PerfilEmpresa AtualizarPerfil(WheraApi api, string id, object perfil)
        {
            return api.Put<PerfilEmpresa>("api/empresas/" + Uri.EscapeDataString(id) + "/perfil", perfil);
        }

        public static List<GerenteTenant> Gerentes(WheraApi api, string id)
        {
            return api.Get<List<GerenteTenant>>("api/empresas/" + Uri.EscapeDataString(id) + "/gerentes") ?? new List<GerenteTenant>();
        }

        public static GerenteTenant AdicionarGerente(WheraApi api, string id, string email)
        {
            return api.Post<GerenteTenant>("api/empresas/" + Uri.EscapeDataString(id) + "/gerentes", new { Email = email });
        }

        public static void RemoverGerente(WheraApi api, string id, string gerenteId)
        {
            api.Delete("api/empresas/" + Uri.EscapeDataString(id) + "/gerentes/" + Uri.EscapeDataString(gerenteId));
        }

        public static ResultadoConteudos ListarConteudos(WheraApi api, string id, string tipo)
        {
            return api.Get<ResultadoConteudos>("api/empresas/" + Uri.EscapeDataString(id) + "/conteudos?tipo=" + Uri.EscapeDataString(tipo));
        }

        public static ConteudoTenant SalvarConteudo(WheraApi api, string empresaId, string id, object request)
        {
            return api.Put<ConteudoTenant>("api/empresas/" + Uri.EscapeDataString(empresaId) + "/conteudos/" + Uri.EscapeDataString(id), request);
        }

        public static MidiaConteudo EnviarMidia(WheraApi api, string empresaId, string nome, string tipo, byte[] conteudo)
        {
            return api.EnviarArquivo<MidiaConteudo>("api/empresas/" + Uri.EscapeDataString(empresaId) + "/conteudos/midias", "arquivo", nome, tipo, conteudo);
        }

        public static UsuarioTenant MeuUsuario(WheraApi api)
        {
            return api.Get<UsuarioTenant>("api/usuarios/me");
        }

        public static UsuarioTenant AtualizarUsuario(WheraApi api, string nome, string email)
        {
            return api.Put<UsuarioTenant>("api/usuarios/me", new { Nome = nome, Email = email });
        }

        public static void AlterarSenha(WheraApi api, string senhaAtual, string novaSenha)
        {
            api.Patch<object>("api/usuarios/me/senha", new { SenhaAtual = senhaAtual, NovaSenha = novaSenha });
        }

        public static void AlterarSegundaSenha(WheraApi api, string atual, string nova)
        {
            api.Patch<object>("api/usuarios/me/segunda-senha", new { SegundaSenhaAtual = atual, NovaSegundaSenha = nova });
        }

        public static PreCadastroResposta CriarPreCadastro(WheraApi api, object request)
        {
            return api.Post<PreCadastroResposta>("api/pre-cadastros/empresas", request);
        }

        public static VinculoPreCadastroResposta VincularPreCadastro(WheraApi api, string empresaId, string codigo, object request)
        {
            return api.PostComHeader<VinculoPreCadastroResposta>("api/pre-cadastros/empresas/" + Uri.EscapeDataString(empresaId) + "/vincular", request, "X-Codigo-Continuacao", codigo);
        }

        public static ValidacaoPreCadastroResposta ValidarPreCadastro(WheraApi api, string empresaId, string codigo)
        {
            return api.PostComHeader<ValidacaoPreCadastroResposta>("api/pre-cadastros/empresas/" + Uri.EscapeDataString(empresaId) + "/validar", null, "X-Codigo-Continuacao", codigo);
        }

        public static List<AtendentePerfil> Atendentes(WheraApi api, string id) { return api.Get<List<AtendentePerfil>>(Rota(id, "atendentes")) ?? new List<AtendentePerfil>(); }
        public static AtendentePerfil AdicionarAtendente(WheraApi api, string id, object request) { return api.Post<AtendentePerfil>(Rota(id, "atendentes"), request); }
        public static AtendentePerfil AtualizarAtendente(WheraApi api, string empresaId, string atendenteId, object request) { return api.Put<AtendentePerfil>(Rota(empresaId, "atendentes") + "/" + Uri.EscapeDataString(atendenteId), request); }
        public static void RemoverAtendente(WheraApi api, string empresaId, string atendenteId) { api.Delete(Rota(empresaId, "atendentes") + "/" + Uri.EscapeDataString(atendenteId)); }
        public static List<AtendentePerfil> ReordenarAtendentes(WheraApi api, string id, List<string> ids) { return api.Put<List<AtendentePerfil>>(Rota(id, "atendentes") + "/ordem", new { Ids = ids }); }

        public static SemanaFuncionamento Semana(WheraApi api, string id) { return api.Get<SemanaFuncionamento>(Rota(id, "horarios")); }
        public static SemanaFuncionamento DefinirSemana(WheraApi api, string id, object request) { return api.Put<SemanaFuncionamento>(Rota(id, "horarios/semana"), request); }

        public static CanalPerfil AdicionarCanal(WheraApi api, string id, object request) { return api.Post<CanalPerfil>(Rota(id, "canais"), request); }
        public static CanalPerfil AtualizarCanal(WheraApi api, string empresaId, string canalId, object request) { return api.Put<CanalPerfil>(Rota(empresaId, "canais") + "/" + Uri.EscapeDataString(canalId), request); }
        public static void RemoverCanal(WheraApi api, string empresaId, string canalId) { api.Delete(Rota(empresaId, "canais") + "/" + Uri.EscapeDataString(canalId)); }
        public static List<CanalPerfil> Canais(WheraApi api, string id) { return api.Get<List<CanalPerfil>>(Rota(id, "canais")) ?? new List<CanalPerfil>(); }

        public static ImagemPerfilEnviada EnviarImagemPerfil(WheraApi api, string empresaId, string nome, string tipo, string tipoImagem, byte[] conteudo)
        {
            return api.EnviarArquivoComCampo<ImagemPerfilEnviada>(Rota(empresaId, "imagens"), "arquivo", nome, tipo, conteudo, "tipo", tipoImagem);
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

    public class PreCadastroResposta { public string EmpresaId { get; set; } public string Slug { get; set; } public string Status { get; set; } public string CodigoContinuacao { get; set; } public string CodigoExpiraEm { get; set; } }
    public class ValidacaoPreCadastroResposta { public string EmpresaId { get; set; } public string Slug { get; set; } public string Status { get; set; } public string CodigoExpiraEm { get; set; } public string IdentificadorTemporario { get; set; } public string IdentificadorExpiraEm { get; set; } }
    public class VinculoPreCadastroResposta { public string EmpresaId { get; set; } public string Slug { get; set; } public string Status { get; set; } public SessaoResposta Sessao { get; set; } }
    public class ImagemPerfilEnviada { public string Url { get; set; } }
}
