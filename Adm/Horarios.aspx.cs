using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

public partial class Adm_Horarios : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.Horarios.Empresa";
    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }
    private void Carregar() { try { var empresa = TenantApi.MinhaEmpresa(Api); if (empresa == null) { Mostrar("Nenhuma empresa vinculada."); return; } Session[ChaveEmpresa] = empresa; Preencher(TenantApi.Semana(Api, empresa.Id)); pnlConteudo.Visible = true; } catch (WheraApiException ex) { Erro(ex); } }
    private void Preencher(SemanaFuncionamento semana) { if (semana == null || semana.Dias == null) return; foreach (var dia in semana.Dias) { var slots = dia.Intervalos ?? new List<IntervaloFuncionamento>(); Definir(dia.DiaSemana, slots, 0); Definir(dia.DiaSemana, slots, 1); } }
    private void Definir(string dia, List<IntervaloFuncionamento> slots, int indice) { var item = slots.Count > indice ? slots[indice] : null; var partes = CamposReal(dia, indice); partes[0].Text = item == null ? "" : Hora(item.Abertura); partes[1].Text = item == null ? "" : Hora(item.Fechamento); }
    private static string Hora(string valor) { return string.IsNullOrWhiteSpace(valor) ? "" : valor.Length >= 5 ? valor.Substring(0, 5) : valor; }
    private TextBox[] Campos(string dia, int indice) { string sufixo = indice == 0 ? "" : "2"; switch (dia) { case "Domingo": return new[] { domA, domF }; case "Segunda": return new[] { segA, segF }; case "Terca": return new[] { terA, terF }; case "Quarta": return new[] { quaA, quaF }; case "Quinta": return new[] { quiA, quiF }; case "Sexta": return new[] { sexA, sexF }; default: return new[] { sabA, sabF }; } }
    private TextBox[] CamposReal(string dia, int indice) { if (indice == 0) return Campos(dia, 0); switch (dia) { case "Domingo": return new[] { domA2, domF2 }; case "Segunda": return new[] { segA2, segF2 }; case "Terca": return new[] { terA2, terF2 }; case "Quarta": return new[] { quaA2, quaF2 }; case "Quinta": return new[] { quiA2, quiF2 }; case "Sexta": return new[] { sexA2, sexF2 }; default: return new[] { sabA2, sabF2 }; } }
    protected void btnSalvar_Click(object sender, EventArgs e) { var empresa = Session[ChaveEmpresa] as PerfilEmpresa; var lista = new List<object>(); try { foreach (var dia in new[] { "Domingo", "Segunda", "Terca", "Quarta", "Quinta", "Sexta", "Sabado" }) for (int i = 0; i < 2; i++) { var c = CamposReal(dia, i); bool a = !string.IsNullOrWhiteSpace(c[0].Text), f = !string.IsNullOrWhiteSpace(c[1].Text); if (a != f) throw new Exception("Preencha abertura e fechamento juntos em todos os intervalos."); if (a) lista.Add(new { DiaSemana = dia, Abertura = c[0].Text, Fechamento = c[1].Text }); } TenantApi.DefinirSemana(Api, empresa.Id, new { Horarios = lista }); pnlSucesso.Visible = true; litSucesso.Text = "Horários atualizados."; } catch (WheraApiException ex) { Erro(ex); } catch (Exception ex) { Mostrar(ex.Message); } }
    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } Mostrar(ex.StatusCode == 403 ? "Você não tem permissão para editar os horários desta empresa." : ex.Message); }
    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
