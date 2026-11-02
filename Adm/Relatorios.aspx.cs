using System;
using WheraOwnerTenant;

/// <summary>Endereço antigo do menu: o único relatório disponível hoje é o de publicações.</summary>
public partial class Adm_Relatorios : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e) { Response.Redirect("~/Adm/PublicacoesDashboard.aspx", true); }
}
