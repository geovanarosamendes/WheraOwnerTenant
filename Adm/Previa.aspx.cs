using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_Previa : PaginaTenant
{
    private static readonly string[][] Telas =
    {
        new[] { "loja", "Tela da loja", "O cliente" },
        new[] { "gerente", "Tela do gerente", "A equipe" },
        new[] { "perfil", "Perfil da empresa", "O dono ou gerente" }
    };

    protected int PercentualEmUso { get; private set; }

    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            var dados = PreviaDados.Carregar(Api, empresa);
            var tela = Telas.Any(t => t[0] == Request.QueryString["tela"]) ? Request.QueryString["tela"] : "loja";

            List<RecursoPrevia> recursos;
            if (tela == "gerente") { recursos = dados.RecursosGerente(); litTela.Text = PreviaTelas.Gerente(dados, recursos); }
            else if (tela == "perfil") { recursos = dados.RecursosPerfil(); litTela.Text = PreviaTelas.Perfil(dados, EhDono(dados)); }
            else { recursos = dados.RecursosLoja(); litTela.Text = PreviaTelas.Loja(dados); }

            litAbas.Text = Abas(tela);
            litStatus.Text = StatusEmpresa.Selo(empresa.Status);
            MontarResumo(tela, empresa, recursos);
            pnlPrevia.Visible = true;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(ex.StatusCode == 403 ? "Você não tem permissão para visualizar esta empresa." : ex.Message);
        }
    }

    private static bool EhDono(PreviaDados dados)
    {
        var vinculo = dados.Empresas.FirstOrDefault(x => x.Id == dados.Empresa.Id);
        return vinculo == null ? SessaoTenant.EhTenant : vinculo.Vinculo == "Tenant";
    }

    private string Abas(string atual)
    {
        var html = new StringBuilder();
        foreach (var tela in Telas)
            html.Append("<a class=\"aba").Append(tela[0] == atual ? " ativa\" aria-current=\"page" : "").Append("\" href=\"").Append(ResolveUrl("~/Adm/Previa.aspx?tela=" + tela[0])).Append("\">").Append(tela[1]).Append("</a>");
        return html.ToString();
    }

    private void MontarResumo(string tela, PerfilEmpresa empresa, List<RecursoPrevia> recursos)
    {
        var emUso = recursos.Where(r => r.Situacao == SituacaoRecurso.EmUso).ToList();
        var restantes = recursos.Where(r => r.Situacao != SituacaoRecurso.EmUso).OrderBy(r => r.Situacao == SituacaoRecurso.NaoVerificado).ThenBy(r => r.Url == null).ToList();
        PercentualEmUso = recursos.Count == 0 ? 0 : emUso.Count * 100 / recursos.Count;
        litContagem.Text = emUso.Count + " de " + recursos.Count + " recursos em uso";
        var quem = Telas.First(t => t[0] == tela)[2];
        litLegenda.Text = (tela == "loja"
            ? "Pontilhado é o que ainda não está em uso. Ele só existe nesta simulação: no app, o cliente simplesmente não vê esse espaço."
            : "Pontilhado é o que ainda não está em uso. " + quem + " vê esta tela no app, já com esse item disponível para usar.")
            + (empresa.Status != StatusEmpresa.Publicada ? " Os clientes só veem a empresa depois da publicação." : "");
        pnlParcial.Visible = recursos.Any(r => r.Situacao == SituacaoRecurso.NaoVerificado);

        pnlFaltando.Visible = restantes.Count > 0;
        litTituloFaltando.Text = "Ainda não usados (" + restantes.Count + ")";
        litFaltando.Text = Lista(restantes);
        pnlEmUso.Visible = emUso.Count > 0;
        litTituloEmUso.Text = "Em uso (" + emUso.Count + ")";
        litEmUso.Text = Lista(emUso);
    }

    private string Lista(IEnumerable<RecursoPrevia> recursos)
    {
        var html = new StringBuilder("<ul class=\"recursos\">");
        foreach (var r in recursos)
        {
            var selo = r.Situacao == SituacaoRecurso.EmUso ? "<span class=\"selo selo-sucesso\">Em uso</span>"
                : r.Situacao == SituacaoRecurso.Faltando ? "<span class=\"selo selo-neutro\">Não usado</span>"
                : "<span class=\"selo selo-aviso\">Não verificado</span>";
            html.Append("<li class=\"recurso\" data-recurso-item=\"").Append(r.Chave).Append("\"><div class=\"recurso-topo\"><button type=\"button\" class=\"recurso-nome\" data-ver-recurso=\"").Append(r.Chave).Append("\" title=\"Mostrar no celular\">").Append(HttpUtility.HtmlEncode(r.Nome)).Append("</button>").Append(selo).Append("</div>");
            html.Append("<p>").Append(HttpUtility.HtmlEncode(r.OQueE)).Append("</p>");
            html.Append("<p class=\"recurso-beneficio\">").Append(HttpUtility.HtmlEncode(r.Beneficio)).Append("</p>");
            if (r.Situacao != SituacaoRecurso.EmUso)
            {
                if (r.Url != null) html.Append("<a class=\"recurso-acao\" href=\"").Append(ResolveUrl(r.Url)).Append("\">").Append(HttpUtility.HtmlEncode(r.Acao)).Append("</a>");
                else html.Append("<p class=\"recurso-origem\">").Append(HttpUtility.HtmlEncode(r.Origem)).Append("</p>");
            }
            html.Append("</li>");
        }
        return html.Append("</ul>").ToString();
    }
}
