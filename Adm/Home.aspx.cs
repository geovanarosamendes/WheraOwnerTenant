using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

public partial class Adm_Home : PaginaTenant
{
    // No ViewState: o anel é desenhado também nos postbacks que não recarregam a empresa.
    protected int Feitos { get { return ViewState["Feitos"] as int? ?? 0; } private set { ViewState["Feitos"] = value; } }
    protected int Total { get { return ViewState["Total"] as int? ?? 1; } private set { ViewState["Total"] = value; } }
    protected int Percentual { get { return Feitos * 100 / Total; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var aviso = SessaoTenant.RetirarAviso();
            if (!string.IsNullOrWhiteSpace(aviso)) { litAviso.Text = HttpUtility.HtmlEncode(aviso); pnlAviso.Visible = true; }
            Carregar();
        }
    }

    protected void btnAtualizar_Click(object sender, EventArgs e) { Carregar(); }

    protected void ddlEmpresas_SelectedIndexChanged(object sender, EventArgs e)
    {
        var escolhida = ddlEmpresas.SelectedValue;
        if (string.IsNullOrWhiteSpace(escolhida)) return;
        try
        {
            // A API confere o vínculo: uma empresa sem acesso responde 403 e não entra na sessão.
            TenantApi.PerfilEmpresa(Api, escolhida);
            SessaoTenant.SelecionarEmpresa(escolhida);
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 403 || ex.StatusCode == 404) MostrarErro("Você não tem acesso a esta empresa.");
            else TratarErro(ex);
        }
        Carregar();
    }

    /// <summary>Só aparece para quem tem acesso a mais de uma empresa.</summary>
    private void MontarTrocaEmpresa(string empresaAtualId)
    {
        pnlTrocaEmpresa.Visible = false;
        AcessoTenant acesso;
        try { acesso = TenantApi.MeuAcesso(Api); }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) TratarSessaoRecusada(ex);
            return; // Sem a lista, o painel segue na empresa atual.
        }
        var empresas = acesso == null || acesso.Empresas == null ? new List<EmpresaAcessoTenant>() : acesso.Empresas;
        if (empresas.Count < 2 && (empresaAtualId != null || empresas.Count == 0)) return;
        ddlEmpresas.Items.Clear();
        if (empresaAtualId == null) ddlEmpresas.Items.Add(new ListItem("Escolha uma empresa", ""));
        foreach (var empresa in empresas)
            ddlEmpresas.Items.Add(new ListItem(empresa.NomeFantasia + " (" + StatusEmpresa.Rotulo(empresa.Status) + ")", empresa.Id));
        var atual = ddlEmpresas.Items.FindByValue(empresaAtualId ?? "");
        if (atual != null) ddlEmpresas.SelectedValue = atual.Value;
        pnlTrocaEmpresa.Visible = true;
    }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            MontarTrocaEmpresa(empresa == null ? null : empresa.Id);
            pnlSemEmpresa.Visible = empresa == null;
            pnlDashboard.Visible = empresa != null;
            if (empresa == null) return;
            pnlDashboard.Visible = true;
            var cidade = empresa.Cidade ?? "Cidade não informada";
            var categoria = empresa.Categoria == null ? null : empresa.Categoria.Nome;
            litEmpresa.Text = HttpUtility.HtmlEncode(empresa.NomeFantasia);
            litCidade.Text = HttpUtility.HtmlEncode(cidade);
            litStatus.Text = StatusEmpresa.Selo(empresa.Status);
            litCardNome.Text = HttpUtility.HtmlEncode(empresa.NomeFantasia);
            litCardLocal.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(categoria) ? cidade : categoria + ", " + cidade);
            litInicial.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(empresa.NomeFantasia) ? "" : empresa.NomeFantasia.Trim().Substring(0, 1).ToUpperInvariant());
            var imagens = empresa.Imagens ?? new ImagensPerfil();
            DefinirImagem(imgLogo, imagens.Logo);
            DefinirImagem(imgCapa, string.IsNullOrWhiteSpace(imagens.FotoFachada) ? imagens.FotoCapa : imagens.FotoFachada);
            MontarPassos(empresa, imagens);
            int conteudos = 0;
            try { conteudos = (TenantApi.ListarConteudos(Api, empresa.Id, "Story").Itens ?? new List<ConteudoTenant>()).Count + (TenantApi.ListarConteudos(Api, empresa.Id, "Promocao").Itens ?? new List<ConteudoTenant>()).Count; } catch (WheraApiException) { }
            litConteudos.Text = conteudos.ToString();
        }
        catch (WheraApiException ex) { TratarErro(ex); }
    }

    private static void DefinirImagem(Image imagem, string url)
    {
        imagem.Visible = !string.IsNullOrWhiteSpace(url);
        imagem.ImageUrl = WheraApi.UrlMidia(url ?? "");
    }

    private void MontarPassos(PerfilEmpresa empresa, ImagensPerfil imagens)
    {
        var endereco = empresa.Endereco;
        var passos = new[]
        {
            new { Titulo = "Apresente sua empresa", Detalhe = "Nome e descrição do que você oferece", Url = "~/Adm/MinhaEmpresa.aspx", Feito = !string.IsNullOrWhiteSpace(empresa.NomeFantasia) && !string.IsNullOrWhiteSpace(empresa.Descricao) },
            new { Titulo = "Facilite o contato", Detalhe = "WhatsApp, site ou Instagram", Url = "~/Adm/MinhaEmpresa.aspx", Feito = empresa.Canais != null && empresa.Canais.Count > 0 },
            new { Titulo = "Mostre onde encontrar você", Detalhe = "Endereço completo da empresa", Url = "~/Adm/MinhaEmpresa.aspx", Feito = endereco != null && !string.IsNullOrWhiteSpace(endereco.Logradouro) && !string.IsNullOrWhiteSpace(endereco.Cidade) },
            new { Titulo = "Informe seus horários", Detalhe = "Dias e horários de atendimento", Url = "~/Adm/Horarios.aspx", Feito = empresa.Horarios != null && empresa.Horarios.Count > 0 },
            new { Titulo = "Adicione logo e fachada", Detalhe = "As imagens que aparecem no card", Url = "~/Adm/Imagens.aspx", Feito = !string.IsNullOrWhiteSpace(imagens.Logo) && !string.IsNullOrWhiteSpace(imagens.FotoFachada) }
        };
        Total = passos.Length;
        Feitos = passos.Count(p => p.Feito);
        rptPassos.DataSource = passos.OrderBy(p => p.Feito).ToList();
        rptPassos.DataBind();
    }

    private void TratarErro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } MostrarErro(ex.Message); }
    private void MostrarErro(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
