using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

public partial class Adm_MasterPage : MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        litUsuario.Text = HttpUtility.HtmlEncode(WheraOwnerTenant.SessaoTenant.Nome ?? WheraOwnerTenant.SessaoTenant.Email ?? "");
    }

    protected string Ativo(params string[] telas)
    {
        string atual = Path.GetFileName(Request.Path);
        return telas.Any(t => string.Equals(t, atual, StringComparison.OrdinalIgnoreCase)) ? "ativo" : "";
    }
}
