using System;
using WheraOwnerTenant;

public partial class Adm_Home : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var nome = SessaoTenant.Nome;
            ((Adm_MasterPage)Master).Usuario = string.IsNullOrWhiteSpace(nome) ? SessaoTenant.Email : nome;
        }
    }
}
