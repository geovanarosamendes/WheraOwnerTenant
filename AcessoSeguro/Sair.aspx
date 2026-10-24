<%@ Page Language="C#" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        WheraOwnerTenant.SessaoTenant.Encerrar();
        Response.Redirect("~/AcessoSeguro/Login.aspx", true);
    }
</script>
