using System;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_Configuracoes : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }
    private void Carregar() { try { var usuario = TenantApi.MeuUsuario(Api); txtNome.Text = usuario == null ? SessaoTenant.Nome : usuario.Nome; txtEmail.Text = usuario == null ? SessaoTenant.Email : usuario.Email; } catch (WheraApiException ex) { Erro(ex); } }
    protected void btnSalvarConta_Click(object sender, EventArgs e) { try { TenantApi.AtualizarUsuario(Api, txtNome.Text.Trim(), txtEmail.Text.Trim()); Mensagem("Dados da conta atualizados."); } catch (WheraApiException ex) { Erro(ex); } }
    protected void btnSenha_Click(object sender, EventArgs e) { if (string.IsNullOrWhiteSpace(txtSenhaAtual.Text) || string.IsNullOrWhiteSpace(txtNovaSenha.Text)) { Mostrar("Informe a senha atual e a nova senha."); return; } try { TenantApi.AlterarSenha(Api, txtSenhaAtual.Text, txtNovaSenha.Text); txtSenhaAtual.Text = ""; txtNovaSenha.Text = ""; Mensagem("Senha alterada com sucesso."); } catch (WheraApiException ex) { Erro(ex); } }
    private void Mensagem(string texto) { pnlSucesso.Visible = true; litSucesso.Text = HttpUtility.HtmlEncode(texto); pnlErro.Visible = false; }
    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } Mostrar(ex.Message); }
    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
