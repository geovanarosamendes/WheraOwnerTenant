using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_CuponsCriar : PaginaTenant
{
    private static readonly CultureInfo Brasil = new CultureInfo("pt-BR");

    /// <summary>Criado uma vez por formulário: repetir o envio não duplica o cupom na API.</summary>
    private string CupomId
    {
        get { var id = ViewState["CupomId"] as string; if (id == null) { id = Guid.NewGuid().ToString(); ViewState["CupomId"] = id; } return id; }
    }

    private string EmpresaId { get { return ViewState["EmpresaId"] as string; } set { ViewState["EmpresaId"] = value; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            EmpresaId = empresa.Id;
            pnlConteudo.Visible = true;
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    protected void btnCriar_Click(object sender, EventArgs e)
    {
        var titulo = txtTitulo.Text.Trim();
        var percentual = ddlTipo.SelectedValue == "Percentual";
        decimal valor;
        if (titulo.Length == 0) { Mostrar("Dê um título para o cupom."); return; }
        if (!LerValor(txtValor.Text.Trim(), out valor) || valor <= 0 || valor > 99999999m || (percentual && valor > 100))
        {
            Mostrar(percentual ? "Informe um desconto entre 0,01% e 100%, como 10 ou 12,50." : "Informe o valor do desconto em reais, como 5 ou 12,50.");
            return;
        }
        int? limite = null;
        var textoLimite = txtLimite.Text.Trim();
        if (textoLimite.Length > 0)
        {
            int pessoas;
            if (!Regex.IsMatch(textoLimite, "^[0-9]+$") || !int.TryParse(textoLimite, out pessoas) || pessoas < 1 || pessoas > 1000000) { Mostrar("Informe para quantas pessoas o cupom vale, de 1 a 1.000.000, ou deixe vazio."); return; }
            limite = pessoas;
        }
        int? prazo = ddlPrazo.SelectedValue.Length == 0 ? (int?)null : int.Parse(ddlPrazo.SelectedValue, CultureInfo.InvariantCulture);
        if (limite != null && prazo == null) { Mostrar("Com limite de pessoas, escolha quanto tempo a pessoa tem para usar o cupom depois de resgatar."); return; }
        try
        {
            var fim = DateTime.UtcNow.AddDays(int.Parse(ddlDias.SelectedValue, CultureInfo.InvariantCulture)).ToString("o", CultureInfo.InvariantCulture);
            TenantApi.CriarCupom(Api, EmpresaId, CupomId, new { Titulo = titulo, Condicoes = txtCondicoes.Text.Trim(), Publico = ddlPublico.SelectedValue, TipoDesconto = ddlTipo.SelectedValue, Valor = valor, FimEm = fim, LimiteResgates = limite, PrazoUsoMinutos = prazo });
        }
        catch (WheraApiException ex) { Erro(ex); return; }
        AvisarNaProximaTela("Cupom criado.");
        Response.Redirect("~/Adm/Cupons.aspx", true);
    }

    /// <summary>Aceita 10, 12,50 e 1.000,50. Recusa 10.5, que o formato brasileiro leria como 105.</summary>
    private static bool LerValor(string texto, out decimal valor)
    {
        valor = 0;
        return Regex.IsMatch(texto, @"^([0-9]+|[0-9]{1,3}(\.[0-9]{3})+)(,[0-9]{1,2})?$") && decimal.TryParse(texto, NumberStyles.Number, Brasil, out valor);
    }

    private void Erro(WheraApiException ex)
    {
        if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
        Mostrar(ex.StatusCode == 403 ? "Você não tem permissão para criar cupons nesta empresa." : ex.Message);
    }

    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
