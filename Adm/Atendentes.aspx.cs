using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_Atendentes : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.Atendentes.Empresa";
    private const string ChaveLista = "WheraOwnerTenant.Atendentes.Lista";
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }
    private void Carregar() { try { var empresa = TenantApi.MinhaEmpresa(Api); if (empresa == null) { pnlSemEmpresa.Visible = true; return; } Session[ChaveEmpresa] = empresa; litEmpresa.Text = HttpUtility.HtmlEncode(empresa.NomeFantasia); pnlConteudo.Visible = true; Bind(TenantApi.Atendentes(Api, empresa.Id)); } catch (WheraApiException ex) { Erro(ex); } }
    private List<AtendentePerfil> Lista { get { return Session[ChaveLista] as List<AtendentePerfil> ?? new List<AtendentePerfil>(); } set { Session[ChaveLista] = value; } }
    private void Bind(List<AtendentePerfil> lista) { Lista = lista; rptAtendentes.DataSource = lista; rptAtendentes.DataBind(); pnlVazio.Visible = lista.Count == 0; }
    protected void btnSalvar_Click(object sender, EventArgs e) { var empresa = Session[ChaveEmpresa] as PerfilEmpresa; if (empresa == null) { Carregar(); return; } if (string.IsNullOrWhiteSpace(txtNome.Text)) { MostrarErro("Informe o nome público."); return; } try { var atual = Lista.FirstOrDefault(x => x.Id == hidId.Value); object req = new { NomePublico = txtNome.Text.Trim(), Funcao = txtFuncao.Text.Trim(), Telefone = txtTelefone.Text.Trim(), WhatsApp = txtWhatsApp.Text.Trim(), Email = txtEmail.Text.Trim(), Principal = atual != null && atual.Principal, Ativo = atual == null || atual.Ativo }; if (string.IsNullOrWhiteSpace(hidId.Value)) TenantApi.AdicionarAtendente(Api, empresa.Id, req); else TenantApi.AtualizarAtendente(Api, empresa.Id, hidId.Value, req); Mensagem("Atendente salvo."); Limpar(); Bind(TenantApi.Atendentes(Api, empresa.Id)); } catch (WheraApiException ex) { Erro(ex); } }
    protected void btnEditar_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e) { var item = Lista.FirstOrDefault(x => x.Id == Convert.ToString(e.CommandArgument)); if (item == null) return; hidId.Value = item.Id; txtNome.Text = item.NomePublico; txtFuncao.Text = item.Funcao; txtTelefone.Text = item.Telefone; txtWhatsApp.Text = item.WhatsApp; txtEmail.Text = item.Email; }
    protected void btnRemover_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e) { var empresa = Session[ChaveEmpresa] as PerfilEmpresa; try { TenantApi.RemoverAtendente(Api, empresa.Id, Convert.ToString(e.CommandArgument)); Mensagem("Atendente removido."); Bind(TenantApi.Atendentes(Api, empresa.Id)); } catch (WheraApiException ex) { Erro(ex); } }
    protected void btnOrdem_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e) { var empresa = Session[ChaveEmpresa] as PerfilEmpresa; var partes = Convert.ToString(e.CommandArgument).Split('|'); var lista = Lista; var i = lista.FindIndex(x => x.Id == partes[1]); int destino = partes[0] == "up" ? i - 1 : i + 1; if (i < 0 || destino < 0 || destino >= lista.Count) return; var item = lista[i]; lista.RemoveAt(i); lista.Insert(destino, item); try { Bind(TenantApi.ReordenarAtendentes(Api, empresa.Id, lista.Select(x => x.Id).ToList())); Mensagem("Ordem atualizada."); } catch (WheraApiException ex) { Erro(ex); } }
    protected void btnNovo_Click(object sender, EventArgs e) { Limpar(); }
    private void Limpar() { hidId.Value = ""; txtNome.Text = ""; txtFuncao.Text = ""; txtTelefone.Text = ""; txtWhatsApp.Text = ""; txtEmail.Text = ""; }
    protected string Meios(object telefone, object whatsapp) { return string.Join(" · ", new[] { Convert.ToString(telefone), Convert.ToString(whatsapp) }.Where(x => !string.IsNullOrWhiteSpace(x))); }
    private void Mensagem(string texto) { pnlSucesso.Visible = true; litSucesso.Text = HttpUtility.HtmlEncode(texto); }
    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } MostrarErro(ex.StatusCode == 403 ? "Você não tem permissão para administrar atendentes desta empresa." : ex.Message); }
    private void MostrarErro(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
