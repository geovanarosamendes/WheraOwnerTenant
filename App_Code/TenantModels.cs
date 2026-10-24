using System.Collections.Generic;

namespace WheraOwnerTenant
{
    /// <summary>Recorte do PerfilEmpresaResponse usado pelo contexto autorizado do tenant.</summary>
    public class PerfilEmpresa
    {
        public string Id { get; set; }
        public string NomeFantasia { get; set; }
        public string Slug { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public int Versao { get; set; }
        public EnderecoPerfil Endereco { get; set; }
        public CategoriaResumo Categoria { get; set; }
        public List<CanalPerfil> Canais { get; set; }
        public ProprietarioResumo Proprietario { get; set; }
        public List<HorarioPerfil> Horarios { get; set; }
        public List<AtendentePerfil> Atendentes { get; set; }
        public ImagensPerfil Imagens { get; set; }
        public List<ItemCatalogo> Catalogo { get; set; }

        public string Cidade
        {
            get { return Endereco == null ? null : Endereco.Cidade; }
        }
    }

    public class EnderecoPerfil { public string Logradouro { get; set; } public string Numero { get; set; } public string Complemento { get; set; } public string Bairro { get; set; } public string Cidade { get; set; } public string Estado { get; set; } public string Cep { get; set; } }
    public class CanalPerfil { public string Id { get; set; } public string Tipo { get; set; } public string Valor { get; set; } public string Link { get; set; } public string Rotulo { get; set; } public bool Principal { get; set; } }
    public class HorarioPerfil { public string Id { get; set; } public string DiaSemana { get; set; } public string Abertura { get; set; } public string Fechamento { get; set; } }
    public class AtendentePerfil { public string Id { get; set; } public string NomePublico { get; set; } public string Funcao { get; set; } public string Telefone { get; set; } public string WhatsApp { get; set; } public string Email { get; set; } public string LinkWhatsApp { get; set; } public int Ordem { get; set; } public bool Principal { get; set; } public bool Ativo { get; set; } }
    public class ImagensPerfil { public string Logo { get; set; } public string FotoCapa { get; set; } public string FotoFachada { get; set; } public List<string> Galeria { get; set; } }
    public class ItemCatalogo { public string Imagem { get; set; } public string Titulo { get; set; } public string Link { get; set; } }
    public class DiaFuncionamento { public string DiaSemana { get; set; } public string Nome { get; set; } public bool Fechado { get; set; } public List<IntervaloFuncionamento> Intervalos { get; set; } }
    public class IntervaloFuncionamento { public string Id { get; set; } public string Abertura { get; set; } public string Fechamento { get; set; } }
    public class SemanaFuncionamento { public List<DiaFuncionamento> Dias { get; set; } }
    public class CategoriaResumo { public string Id { get; set; } public string Nome { get; set; } }
    public class ProprietarioResumo { public string Id { get; set; } public string Nome { get; set; } public string Email { get; set; } }

    public static class StatusEmpresa
    {
        public const string Rascunho = "Rascunho";
        public const string AguardandoValidacao = "AguardandoValidacao";
        public const string Publicada = "Publicada";
        public const string Suspensa = "Suspensa";
        public const string Arquivada = "Arquivada";
        public static string Rotulo(string status)
        {
            if (status == Rascunho || status == AguardandoValidacao) return "Aguardando aprovação";
            if (status == Publicada) return "Publicada";
            if (status == Suspensa) return "Suspensa";
            if (status == Arquivada) return "Arquivada";
            return "Não informado";
        }
    }
}
