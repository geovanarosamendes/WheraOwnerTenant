using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.IO;
using WheraOwnerTenant;

public partial class Adm_Conteudos : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.Conteudos.Empresa";
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); else AlternarTipo(); }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            Session[ChaveEmpresa] = empresa; pnlConteudo.Visible = true; AlternarTipo(); Bind();
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    private void Bind()
    {
        var empresa = Session[ChaveEmpresa] as PerfilEmpresa; if (empresa == null) return;
        var lista = TenantApi.ListarConteudos(Api, empresa.Id, ddlTipo.SelectedValue);
        rptConteudos.DataSource = lista == null ? new List<ConteudoTenant>() : lista.Itens ?? new List<ConteudoTenant>(); rptConteudos.DataBind();
        pnlVazio.Visible = lista == null || lista.Itens == null || lista.Itens.Count == 0;
    }

    protected void ddlTipo_SelectedIndexChanged(object sender, EventArgs e) { AlternarTipo(); Bind(); }
    private void AlternarTipo() { pnlStory.Visible = ddlTipo.SelectedValue == "Story"; pnlPromocao.Visible = !pnlStory.Visible; }

    protected void btnSalvar_Click(object sender, EventArgs e) { Salvar("Salvar"); }
    protected void btnPublicar_Click(object sender, EventArgs e) { Salvar("Publicar"); }

    private void Salvar(string acao)
    {
        var empresa = Session[ChaveEmpresa] as PerfilEmpresa; if (empresa == null) { Carregar(); return; }
        if (string.IsNullOrWhiteSpace(txtTitulo.Text)) { MostrarTexto("Informe um título."); return; }
        try
        {
            decimal preco; decimal? valor = decimal.TryParse(txtPreco.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out preco) ? (decimal?)preco : null;
            DateTime inicio; DateTime fim; DateTime? inicioUtc = DateTime.TryParse(txtInicio.Text, out inicio) ? (DateTime?)inicio.ToUniversalTime() : null; DateTime? fimUtc = DateTime.TryParse(txtFim.Text, out fim) ? (DateTime?)fim.ToUniversalTime() : null;
            string midiaCaminho = null, midiaTipo = null;
            if (fuMidia.HasFile)
            {
                using (var stream = fuMidia.PostedFile.InputStream)
                using (var memory = new MemoryStream()) { stream.CopyTo(memory); var enviada = TenantApi.EnviarMidia(Api, empresa.Id, Path.GetFileName(fuMidia.FileName), fuMidia.PostedFile.ContentType, memory.ToArray()); midiaCaminho = enviada.Caminho; midiaTipo = enviada.Tipo; }
            }
            if (acao == "Publicar") { MostrarTexto("A publicação depende da aprovação da plataforma. Salve o conteúdo como rascunho e aguarde a validação do Owner."); return; }
            var id = Guid.NewGuid().ToString();
            TenantApi.SalvarConteudo(Api, empresa.Id, id, new { Tipo = ddlTipo.SelectedValue, Acao = acao, Versao = 0, Titulo = txtTitulo.Text.Trim(), Descricao = txtDescricao.Text.Trim(), Condicoes = txtCondicoes.Text.Trim(), Preco = valor, MidiaCaminho = midiaCaminho, MidiaTipo = midiaTipo, Destino = txtDestino.Text.Trim(), DuracaoDias = int.Parse(ddlDuracao.SelectedValue), InicioEm = inicioUtc, FimEm = fimUtc });
            litSucesso.Text = acao == "Publicar" ? "Conteúdo publicado." : "Rascunho salvo."; pnlSucesso.Visible = true; Limpar(); Bind();
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    private void Limpar() { txtTitulo.Text = ""; txtDescricao.Text = ""; txtDestino.Text = ""; txtPreco.Text = ""; txtCondicoes.Text = ""; txtInicio.Text = ""; txtFim.Text = ""; }
    protected string Data(object valor) { DateTime data; return DateTime.TryParse(Convert.ToString(valor), out data) ? data.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : "-"; }
    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } MostrarTexto(ex.StatusCode == 403 ? "Você não tem permissão para administrar conteúdos desta empresa." : ex.Message); }
    private void MostrarTexto(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
