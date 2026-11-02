using System;
using System.Text;
using System.Web;
using WheraOwnerTenant;

/// <summary>Explica a validação feita no aplicativo. Nada aqui altera dados: as telas usam um exemplo.</summary>
public partial class Adm_CuponsValidar : PaginaTenant
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        var nome = "Sua empresa";
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa != null && !string.IsNullOrWhiteSpace(empresa.NomeFantasia)) nome = empresa.NomeFantasia;
        }
        catch (WheraApiException ex)
        {
            if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; }
            // O nome é só um detalhe do exemplo: a explicação continua útil sem ele.
        }
        var agora = PublicacoesTenant.HoraBrasil(DateTime.UtcNow);
        var passos = new[]
        {
            new { Quem = "Cliente", Titulo = "O cliente mostra o cupom", Texto = "Depois de resgatar, o cliente abre Meus cupons no aplicativo e mostra o QR Code e o código. O tempo para usar aparece logo abaixo.", Tela = PreviaValidacao.TelaCliente(nome) },
            new { Quem = "Loja", Titulo = "Abra Validar cupom", Texto = "No seu aplicativo, entre em Conta, toque no ícone de maleta para abrir o perfil da empresa, depois em Gestão da empresa e em Validar cupom.", Tela = PreviaValidacao.TelaGestao(nome) },
            new { Quem = "Loja", Titulo = "Leia o QR Code", Texto = "Aponte a câmera para o celular do cliente. No primeiro uso, o aplicativo pede permissão para usar a câmera.", Tela = PreviaValidacao.TelaCamera() },
            new { Quem = "Loja", Titulo = "Sem câmera? Digite o código", Texto = "Toque em Digitar o código, informe o código do cupom e toque em Conferir código.", Tela = PreviaValidacao.TelaDigitar() },
            new { Quem = "Loja", Titulo = "Confira os dados e valide", Texto = "Compare o nome e a foto com o cliente. Veja também o desconto, as condições e até quando o cupom pode ser usado. Até aqui nada foi registrado. Se estiver tudo certo, toque em Validar cupom, no fim da tela.", Tela = PreviaValidacao.TelaConferir(agora) },
            new { Quem = "Loja", Titulo = "Pronto, cupom validado", Texto = "Ao tocar em Validar cupom, o uso fica registrado e o mesmo código não vale outra vez. Aplique o desconto na venda e toque em Escanear outro cupom para o próximo atendimento.", Tela = PreviaValidacao.TelaValidado(agora) }
        };

        var telas = new StringBuilder();
        var lista = new StringBuilder();
        for (var i = 0; i < passos.Length; i++)
        {
            telas.Append("<div data-tela-passo=\"").Append(i).Append("\"").Append(i == 0 ? "" : " hidden").Append(">").Append(passos[i].Tela).Append("</div>");
            lista.Append("<li class=\"recurso passo-validacao").Append(i == 0 ? " selecionado\" aria-current=\"step" : "").Append("\" data-item-passo=\"").Append(i).Append("\"><div class=\"recurso-topo\"><button type=\"button\" class=\"recurso-nome\" data-ir-passo=\"").Append(i).Append("\">").Append(i + 1).Append(". ").Append(HttpUtility.HtmlEncode(passos[i].Titulo)).Append("</button><span class=\"selo ").Append(passos[i].Quem == "Cliente" ? "selo-aviso" : "selo-neutro").Append("\">").Append(passos[i].Quem).Append("</span></div><p>").Append(HttpUtility.HtmlEncode(passos[i].Texto)).Append("</p></li>");
        }
        litTelas.Text = telas.ToString();
        litPassos.Text = lista.ToString();
    }
}
