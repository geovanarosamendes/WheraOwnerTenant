using System.Web;
using System.Web.UI;

public partial class Adm_MasterPage : MasterPage
{
    public string Usuario
    {
        set { litUsuario.Text = HttpUtility.HtmlEncode(value ?? ""); }
    }
}
