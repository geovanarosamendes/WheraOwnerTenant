using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WheraOwnerTenant;

/// <summary>Histórico somente leitura: o que está no ar e o que já passou.</summary>
public partial class Adm_Conteudos : PaginaTenant
{
    private DateTime agora;

    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }
    protected void ddlTipo_SelectedIndexChanged(object sender, EventArgs e) { Carregar(); }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            var dados = PublicacoesTenant.Carregar(Api, empresa.Id);
            agora = dados.AgoraUtc;
            var ativas = dados.Itens.Where(x => x.Situacao == Publicacao.Ativa).OrderBy(x => x.FimUtc).ToList();
            var programadas = dados.Itens.Where(x => x.Situacao == Publicacao.Programada).OrderBy(x => x.InicioUtc).ToList();
            var passadas = dados.Itens.Where(x => x.Situacao == Publicacao.Encerrada || x.Situacao == Publicacao.Suspensa).ToList();
            var publicadas = dados.Itens.Where(x => x.FoiPublicada).ToList();

            litAtivas.Text = ativas.Count.ToString();
            litProgramadas.Text = programadas.Count.ToString();
            litPassadas.Text = passadas.Count.ToString();
            litStories.Text = publicadas.Count(x => x.Tipo == "Story").ToString();
            litPromocoes.Text = publicadas.Count(x => x.Tipo == "Promocao").ToString();

            rptAtivas.DataSource = ativas; rptAtivas.DataBind(); rptAtivas.Visible = ativas.Count > 0; pnlSemAtivas.Visible = ativas.Count == 0;
            rptProgramadas.DataSource = programadas; rptProgramadas.DataBind(); pnlProgramadas.Visible = programadas.Count > 0;
            var filtradas = string.IsNullOrEmpty(ddlTipo.SelectedValue) ? passadas : passadas.Where(x => x.Tipo == ddlTipo.SelectedValue).ToList();
            rptPassadas.DataSource = filtradas; rptPassadas.DataBind(); rptPassadas.Visible = filtradas.Count > 0; pnlSemPassadas.Visible = filtradas.Count == 0;
            pnlConteudo.Visible = true;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            pnlErro.Visible = true;
            litErro.Text = HttpUtility.HtmlEncode(ex.StatusCode == 403 ? "Você não tem permissão para ver as publicações desta empresa." : ex.Message);
        }
    }

    protected string Linha(object item) { return PublicacoesTenant.LinhaHtml((Publicacao)item, agora); }
}
