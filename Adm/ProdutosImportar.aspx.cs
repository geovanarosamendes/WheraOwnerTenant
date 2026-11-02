using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using WheraOwnerTenant;

/// <summary>Contrato em docs/API_IMPORTACAO_PRODUTOS.md. Se a API em uso ainda não tiver o endpoint, o envio responde 404.</summary>
public partial class Adm_ProdutosImportar : PaginaTenant
{
    private const int TamanhoMaximo = 5 * 1024 * 1024;

    protected void btnImportar_Click(object sender, EventArgs e)
    {
        if (!fuPlanilha.HasFile) { Mostrar("Escolha uma planilha."); return; }
        var extensao = Path.GetExtension(fuPlanilha.FileName).ToLowerInvariant();
        if (extensao != ".xlsx" && extensao != ".csv") { Mostrar("Envie uma planilha .xlsx ou .csv."); return; }
        if (fuPlanilha.PostedFile.ContentLength > TamanhoMaximo) { Mostrar("A planilha pode ter no máximo 5 MB."); return; }
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { Mostrar("Nenhuma empresa vinculada."); return; }
            var tipo = extensao == ".csv" ? "text/csv" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var resultado = TenantApi.ImportarProdutos(Api, empresa.Id, Path.GetFileName(fuPlanilha.FileName), tipo, fuPlanilha.FileBytes);
            litTotal.Text = resultado.TotalLinhas.ToString();
            litImportados.Text = resultado.Importados.ToString();
            litIgnorados.Text = resultado.Ignorados.ToString();
            litErros.Text = resultado.ComErro.ToString();
            rptOcorrencias.DataSource = resultado.Ocorrencias ?? new List<OcorrenciaImportacaoProduto>();
            rptOcorrencias.DataBind();
            pnlResultado.Visible = true;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            Mostrar(ex.StatusCode == 404 ? "A importação por planilha ainda não está disponível. Nenhum produto foi cadastrado."
                : ex.StatusCode == 403 ? "Você não tem permissão para gerenciar os produtos desta empresa."
                : ex.Message);
        }
    }

    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
