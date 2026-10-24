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
}
