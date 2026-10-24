<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        // A API Whera em produção é HTTPS; o .NET 4.8 sozinho pode negociar TLS antigo.
        System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
    }

</script>
