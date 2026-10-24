using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

public partial class Adm_Imagens : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.Imagens.Empresa";
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }
    private void Carregar() { try { var empresa = TenantApi.MinhaEmpresa(Api); if (empresa == null) { Mostrar("Nenhuma empresa vinculada."); return; } Session[ChaveEmpresa] = empresa; pnlConteudo.Visible = true; imgLogo.ImageUrl = WheraApi.UrlMidia(empresa.Imagens == null ? "" : empresa.Imagens.Logo); imgCapa.ImageUrl = WheraApi.UrlMidia(empresa.Imagens == null ? "" : empresa.Imagens.FotoCapa); imgFachada.ImageUrl = WheraApi.UrlMidia(empresa.Imagens == null ? "" : empresa.Imagens.FotoFachada); var galeria = empresa.Imagens == null ? new List<string>() : empresa.Imagens.Galeria ?? new List<string>(); rptGaleria.DataSource = galeria.ConvertAll(WheraApi.UrlMidia); rptGaleria.DataBind(); } catch (WheraApiException ex) { Erro(ex); } }
    protected void btnSalvar_Click(object sender, EventArgs e) { var empresa = Session[ChaveEmpresa] as PerfilEmpresa; if (empresa == null) { Carregar(); return; } try { var imagens = empresa.Imagens ?? new ImagensPerfil(); string logo = Enviar(fuLogo, empresa.Id, "Logo", imagens.Logo); string capa = Enviar(fuCapa, empresa.Id, "FotoCapa", imagens.FotoCapa); string fachada = Enviar(fuFachada, empresa.Id, "FotoFachada", imagens.FotoFachada); var galeria = new List<string>(imagens.Galeria ?? new List<string>()); string nova = Enviar(fuGaleria, empresa.Id, "Galeria", null); if (!string.IsNullOrWhiteSpace(nova)) galeria.Add(nova); TenantApi.AtualizarPerfil(Api, empresa.Id, new { NomeFantasia = empresa.NomeFantasia, Versao = empresa.Versao, Imagens = new { Logo = logo, FotoCapa = capa, FotoFachada = fachada, Galeria = galeria } }); Mensagem("Imagens atualizadas."); Carregar(); } catch (WheraApiException ex) { Erro(ex); } }
    private string Enviar(FileUpload upload, string empresaId, string tipoImagem, string atual) { if (!upload.HasFile) return atual; using (var s = upload.PostedFile.InputStream) using (var m = new MemoryStream()) { s.CopyTo(m); return TenantApi.EnviarImagemPerfil(Api, empresaId, Path.GetFileName(upload.FileName), upload.PostedFile.ContentType, tipoImagem, m.ToArray()).Url; } }
    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } Mostrar(ex.Message); }
    private void Mensagem(string texto) { pnlSucesso.Visible = true; litSucesso.Text = HttpUtility.HtmlEncode(texto); }
    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
