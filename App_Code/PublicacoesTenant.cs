using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace WheraOwnerTenant
{
    // Compativel com C# 5. Consulta apenas: stories e promoções são criados no aplicativo Elo.

    /// <summary>Publicação pronta para as telas, com a situação calculada na hora do servidor da API.</summary>
    public class Publicacao
    {
        public const string Ativa = "Ativa";
        public const string Programada = "Programada";
        public const string Encerrada = "Encerrada";
        public const string Rascunho = "Rascunho";
        public const string Suspensa = "Suspensa";

        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string Situacao { get; set; }
        public DateTime? InicioUtc { get; set; }
        public DateTime? FimUtc { get; set; }
        public DateTime? CriadoUtc { get; set; }
        public decimal? Preco { get; set; }
        public bool Relampago { get; set; }
        public string MidiaUrl { get; set; }
        public string MidiaTipo { get; set; }

        public string TipoRotulo { get { return Tipo == "Promocao" ? "Promoção" : "Story"; } }
        /// <summary>Data que situa a publicação no tempo: o início ou, sem início, a criação.</summary>
        public DateTime? ReferenciaUtc { get { return InicioUtc ?? CriadoUtc; } }
        public bool FoiPublicada { get { return Situacao != Rascunho; } }
    }

    public class ResumoPublicacoes
    {
        public DateTime AgoraUtc { get; set; }
        public List<Publicacao> Itens { get; set; }
        public int Contar(string situacao) { return Itens.Count(x => x.Situacao == situacao); }
    }

    public static class PublicacoesTenant
    {
        private static readonly CultureInfo Brasil = new CultureInfo("pt-BR");
        private static readonly TimeZoneInfo FusoBrasil = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        public static ResumoPublicacoes Carregar(WheraApi api, string empresaId)
        {
            var stories = TenantApi.ListarConteudos(api, empresaId, "Story");
            var promocoes = TenantApi.ListarConteudos(api, empresaId, "Promocao");
            var agora = ParaUtc(stories == null ? null : stories.HoraServidor) ?? DateTime.UtcNow;
            var itens = new List<ConteudoTenant>();
            if (stories != null && stories.Itens != null) itens.AddRange(stories.Itens);
            if (promocoes != null && promocoes.Itens != null) itens.AddRange(promocoes.Itens);
            return new ResumoPublicacoes
            {
                AgoraUtc = agora,
                Itens = itens.Select(x => Montar(x, agora)).OrderByDescending(x => x.ReferenciaUtc).ToList()
            };
        }

        private static Publicacao Montar(ConteudoTenant c, DateTime agora)
        {
            var inicio = ParaUtc(c.InicioEm);
            var fim = ParaUtc(c.FimEm);
            return new Publicacao
            {
                Id = c.Id, Titulo = c.Titulo, Tipo = c.Tipo, InicioUtc = inicio, FimUtc = fim,
                CriadoUtc = c.CriadoEm.HasValue ? (DateTime?)c.CriadoEm.Value.ToUniversalTime() : null,
                Preco = c.Preco, Relampago = c.Relampago, MidiaUrl = WheraApi.UrlMidia(c.MidiaUrl), MidiaTipo = c.MidiaTipo,
                Situacao = CalcularSituacao(c.Estado, inicio, fim, agora)
            };
        }

        private static string CalcularSituacao(string estado, DateTime? inicio, DateTime? fim, DateTime agora)
        {
            if (estado == "Rascunho") return Publicacao.Rascunho;
            if (estado == "Suspensa" || estado == "Moderada") return Publicacao.Suspensa;
            if (estado != "Publicada") return Publicacao.Encerrada;
            if (fim.HasValue && fim.Value <= agora) return Publicacao.Encerrada;
            return inicio.HasValue && inicio.Value > agora ? Publicacao.Programada : Publicacao.Ativa;
        }

        public static DateTime? ParaUtc(string valor)
        {
            DateTime data;
            return DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out data) ? (DateTime?)data : null;
        }

        public static DateTime HoraBrasil(DateTime utc) { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), FusoBrasil); }

        public static string DataHora(DateTime? utc) { return utc.HasValue ? HoraBrasil(utc.Value).ToString("dd/MM/yyyy HH:mm", Brasil) : ""; }

        public static string Mes(DateTime brasil) { var m = brasil.ToString("MMM", Brasil).TrimEnd('.'); return char.ToUpper(m[0]) + m.Substring(1); }

        public static string PrecoTexto(decimal? preco) { return preco.HasValue ? preco.Value.ToString("C", Brasil) : ""; }

        public static string Periodo(Publicacao p)
        {
            if (p.InicioUtc.HasValue && p.FimUtc.HasValue) return DataHora(p.InicioUtc) + " até " + DataHora(p.FimUtc);
            if (p.InicioUtc.HasValue) return "Desde " + DataHora(p.InicioUtc);
            return p.CriadoUtc.HasValue ? "Criada em " + DataHora(p.CriadoUtc) : "Sem data";
        }

        /// <summary>Texto curto do prazo: "Termina em 2 dias" ou "Começa em 5 horas".</summary>
        public static string Prazo(Publicacao p, DateTime agora)
        {
            if (p.Situacao == Publicacao.Ativa && p.FimUtc.HasValue) return "Termina em " + Duracao(p.FimUtc.Value - agora);
            if (p.Situacao == Publicacao.Programada && p.InicioUtc.HasValue) return "Começa em " + Duracao(p.InicioUtc.Value - agora);
            return "";
        }

        private static string Duracao(TimeSpan t)
        {
            if (t.TotalDays >= 2) return (int)t.TotalDays + " dias";
            if (t.TotalHours >= 24) return "1 dia";
            if (t.TotalHours >= 2) return (int)t.TotalHours + " horas";
            if (t.TotalMinutes >= 60) return "1 hora";
            return Math.Max(1, (int)t.TotalMinutes) + " min";
        }

        public static string Selo(string situacao)
        {
            string tom = situacao == Publicacao.Ativa ? "selo-sucesso" : situacao == Publicacao.Programada ? "selo-aviso" : situacao == Publicacao.Suspensa ? "selo-erro" : "selo-neutro";
            return "<span class=\"selo " + tom + "\">" + HttpUtility.HtmlEncode(situacao) + "</span>";
        }

        /// <summary>Linha somente leitura das listas de publicações. Todo texto vindo da API é codificado.</summary>
        public static string LinhaHtml(Publicacao p, DateTime agora)
        {
            var html = new StringBuilder("<li class=\"linha-item\">");
            if (p.MidiaTipo == "Imagem" && !string.IsNullOrWhiteSpace(p.MidiaUrl))
                html.Append("<img class=\"miniatura\" loading=\"lazy\" alt=\"\" src=\"").Append(HttpUtility.HtmlAttributeEncode(p.MidiaUrl)).Append("\" />");
            else
                html.Append("<span class=\"miniatura miniatura-vazia\">").Append(p.MidiaTipo == "Video" ? "Vídeo" : "Sem imagem").Append("</span>");
            var prazo = Prazo(p, agora);
            html.Append("<div class=\"linha-principal\"><strong>").Append(HttpUtility.HtmlEncode(p.Titulo)).Append("</strong><small>")
                .Append(p.TipoRotulo).Append(p.Relampago ? " relâmpago" : "").Append(p.Preco.HasValue ? ", " + PrecoTexto(p.Preco) : "").Append("</small><small>")
                .Append(HttpUtility.HtmlEncode(Periodo(p))).Append("</small>");
            if (prazo.Length > 0) html.Append("<small class=\"linha-prazo\">").Append(prazo).Append("</small>");
            html.Append("</div>").Append(Selo(p.Situacao)).Append("</li>");
            return html.ToString();
        }
    }
}
