using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WheraOwnerTenant;

public partial class Adm_MinhaEmpresa : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.MinhaEmpresa";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) Carregar();
    }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { pnlSemEmpresa.Visible = true; return; }
            Session[ChaveEmpresa] = empresa;
            Preencher(empresa);
            pnlFormulario.Visible = true;
        }
        catch (WheraApiException ex) { MostrarErro(ex); }
    }

    private void Preencher(PerfilEmpresa empresa)
    {
        litNomeCabecalho.Text = HttpUtility.HtmlEncode(empresa.NomeFantasia);
        litStatus.Text = HttpUtility.HtmlEncode(StatusEmpresa.Rotulo(empresa.Status));
        litVersao.Text = empresa.Versao.ToString();
        txtNome.Text = empresa.NomeFantasia ?? "";
        txtSlug.Text = empresa.Slug ?? "";
        txtDescricao.Text = empresa.Descricao ?? "";
        txtSite.Text = Canal(empresa, "Site");
        txtWhatsApp.Text = Canal(empresa, "WhatsApp");
        txtInstagram.Text = Canal(empresa, "Instagram");
        var endereco = empresa.Endereco ?? new EnderecoPerfil();
        txtCep.Text = endereco.Cep ?? "";
        txtLogradouro.Text = endereco.Logradouro ?? "";
        txtNumero.Text = endereco.Numero ?? "";
        txtBairro.Text = endereco.Bairro ?? "";
        txtCidade.Text = endereco.Cidade ?? "";
        txtEstado.Text = endereco.Estado ?? "";
    }

    private static string Canal(PerfilEmpresa empresa, string tipo)
    {
        var canal = empresa.Canais == null ? null : empresa.Canais.FirstOrDefault(x => string.Equals(x.Tipo, tipo, StringComparison.OrdinalIgnoreCase));
        return canal == null ? "" : (canal.Valor ?? canal.Link ?? "");
    }

    protected void btnSalvar_Click(object sender, EventArgs e)
    {
        var empresa = Session[ChaveEmpresa] as PerfilEmpresa;
        if (empresa == null) { Carregar(); return; }
        if (string.IsNullOrWhiteSpace(txtNome.Text)) { MostrarErroTexto("Informe o nome da empresa."); return; }
        try
        {
            var endereco = new { Logradouro = txtLogradouro.Text.Trim(), Numero = txtNumero.Text.Trim(), Complemento = "", Bairro = txtBairro.Text.Trim(), Cidade = txtCidade.Text.Trim(), Estado = txtEstado.Text.Trim(), Cep = txtCep.Text.Trim() };
            var atualizado = TenantApi.AtualizarPerfil(Api, empresa.Id, new
            {
                NomeFantasia = txtNome.Text.Trim(), Slug = (string)null, Descricao = txtDescricao.Text.Trim(), Modalidade = (string)null,
                Versao = empresa.Versao, CategoriaId = (string)null, SubcategoriaIds = (string[])null,
                Site = txtSite.Text.Trim(), WhatsApp = txtWhatsApp.Text.Trim(), Instagram = txtInstagram.Text.Trim(), GoogleMaps = (string)null,
                Endereco = endereco, Horarios = (object)null, Atendentes = (object)null, Imagens = (object)null, Curadoria = (object)null, Catalogo = (object)null
            });
            Session[ChaveEmpresa] = atualizado;
            Preencher(atualizado);
            pnlSucesso.Visible = true; litSucesso.Text = "Perfil atualizado com sucesso.";
        }
        catch (WheraApiException ex) { MostrarErro(ex); }
    }

    private void MostrarErro(WheraApiException ex)
    {
        if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
        MostrarErroTexto(ex.StatusCode == 403 ? "Você não tem permissão para editar esta empresa." : ex.Message);
    }

    private void MostrarErroTexto(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
