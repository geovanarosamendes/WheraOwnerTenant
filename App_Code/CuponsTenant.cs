using System;
using System.Globalization;

namespace WheraOwnerTenant
{
    /// <summary>Textos e situação dos cupons, comuns às telas do submenu Cupons.</summary>
    public static class CuponsTenant
    {
        public const string Vip = "Favoritos";
        public const string Disponivel = "Disponível";
        private static readonly CultureInfo Brasil = new CultureInfo("pt-BR");

        public static string Situacao(CupomEmpresa c, DateTime agora)
        {
            var inicio = PublicacoesTenant.ParaUtc(c.InicioEm);
            var fim = PublicacoesTenant.ParaUtc(c.FimEm);
            if (c.Estado != "Ativo" || fim == null || fim <= agora) return "Encerrado";
            if (c.LimiteResgates.HasValue && c.TotalResgates >= c.LimiteResgates.Value) return "Esgotado";
            return inicio != null && inicio > agora ? "Programado" : Disponivel;
        }

        public static int Ordem(string situacao) { return situacao == Disponivel ? 0 : situacao == "Programado" ? 1 : situacao == "Esgotado" ? 2 : 3; }

        /// <summary>Já em HTML: o rótulo vem de uma lista fixa, não de texto do usuário.</summary>
        public static string Selo(string situacao)
        {
            var tom = situacao == Disponivel ? "selo-sucesso" : situacao == "Encerrado" ? "selo-neutro" : "selo-aviso";
            return "<span class=\"selo " + tom + "\">" + situacao + "</span>";
        }

        public static string Desconto(CupomEmpresa c)
        {
            return c.TipoDesconto == "Percentual" ? c.Valor.ToString("0.##", Brasil) + "% OFF" : c.Valor.ToString("C", Brasil) + " OFF";
        }

        public static string Publico(CupomEmpresa c) { return c.Publico == Vip ? "VIP, quem favorita" : "Comum, quem segue"; }

        public static string PrazoUso(CupomEmpresa c)
        {
            if (!c.PrazoUsoMinutos.HasValue) return "";
            var minutos = c.PrazoUsoMinutos.Value;
            var texto = minutos < 60 ? minutos + " minutos" : minutos % 1440 == 0 && minutos > 1440 ? minutos / 1440 + " dias" : minutos == 60 ? "1 hora" : Math.Round(minutos / 60m) + " horas";
            return texto + " para usar depois de resgatar";
        }

        public static string Validade(CupomEmpresa c)
        {
            return PublicacoesTenant.DataHora(PublicacoesTenant.ParaUtc(c.InicioEm)) + " até " + PublicacoesTenant.DataHora(PublicacoesTenant.ParaUtc(c.FimEm));
        }
    }
}
