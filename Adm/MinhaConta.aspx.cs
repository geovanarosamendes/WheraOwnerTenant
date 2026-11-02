using System;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_MinhaConta : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }
    private void Carregar() { try { var usuario = TenantApi.MeuUsuario(Api); txtNome.Text = usuario == null ? SessaoTenant.Nome : usuario.Nome; txtEmail.Text = usuario == null ? SessaoTenant.Email : usuario.Email; } catch (WheraApiException ex) { Erro(ex); } }
    protected void btnSalvarConta_Click(object sender, EventArgs e) { try { TenantApi.AtualizarUsuario(Api, txtNome.Text.Trim(), txtEmail.Text.Trim()); pnlSucesso.Visible = true; litSucesso.Text = "Dados da conta atualizados."; } catch (WheraApiException ex) { Erro(ex); } }
    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(ex.Message); }
}
