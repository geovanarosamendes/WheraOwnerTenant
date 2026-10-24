using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

public partial class Adm_Catalogo : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.Catalogo.Empresa";
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }
    private void Carregar() { try { var empresa = TenantApi.MinhaEmpresa(Api); if (empresa == null) { Mostrar("Nenhuma empresa vinculada."); return; } Session[ChaveEmpresa] = empresa; pnlConteudo.Visible = true; Bind(empresa.Catalogo ?? new List<ItemCatalogo>()); } catch (WheraApiException ex) { Erro(ex); } }
    private void Bind(List<ItemCatalogo> itens) { rptCatalogo.DataSource = itens; rptCatalogo.DataBind(); }
    protected void btnAdicionar_Click(object sender, EventArgs e) { var empresa = Session[ChaveEmpresa] as PerfilEmpresa; if (!fuImagem.HasFile) { Mostrar("Escolha uma imagem."); return; } try { if ((empresa.Catalogo ?? new List<ItemCatalogo>()).Count >= 6) throw new WheraApiException(400, "O catálogo permite até seis itens.", null); using (var s = fuImagem.PostedFile.InputStream) using (var m = new MemoryStream()) { s.CopyTo(m); var enviada = TenantApi.EnviarImagemPerfil(Api, empresa.Id, Path.GetFileName(fuImagem.FileName), fuImagem.PostedFile.ContentType, "Catalogo", m.ToArray()); var itens = empresa.Catalogo ?? new List<ItemCatalogo>(); itens.Add(new ItemCatalogo { Imagem = enviada.Url, Titulo = "", Link = "" }); Salvar(empresa, itens); } Mensagem("Item adicionado."); Carregar(); } catch (WheraApiException ex) { Erro(ex); } }
    protected void btnSalvar_Click(object sender, EventArgs e) { var empresa = Session[ChaveEmpresa] as PerfilEmpresa; var itens = new List<ItemCatalogo>(); foreach (RepeaterItem row in rptCatalogo.Items) { var imagem = ((HiddenField)row.FindControl("hidImagem")).Value; var titulo = ((TextBox)row.FindControl("txtTitulo")).Text; var link = ((TextBox)row.FindControl("txtLink")).Text; itens.Add(new ItemCatalogo { Imagem = imagem, Titulo = titulo, Link = link }); } try { Salvar(empresa, itens); Mensagem("Catálogo atualizado."); Carregar(); } catch (WheraApiException ex) { Erro(ex); } }
    private void Salvar(PerfilEmpresa empresa, List<ItemCatalogo> itens) { TenantApi.AtualizarPerfil(Api, empresa.Id, new { NomeFantasia = empresa.NomeFantasia, Versao = empresa.Versao, Catalogo = itens }); }
    private void Mensagem(string texto) { pnlSucesso.Visible = true; litSucesso.Text = HttpUtility.HtmlEncode(texto); }
    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } Mostrar(ex.Message); }
    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
