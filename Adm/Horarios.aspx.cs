using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WheraOwnerTenant;

/// <summary>
/// Na API cada dia é uma lista de períodos. Aqui, um período é um dia sem pausa; dois são o dia com almoço.
/// Dias com três ou mais períodos, cadastrados pelo site, são mantidos como estão ao salvar.
/// </summary>
public partial class Adm_Horarios : PaginaTenant
{
    private const string ChaveEmpresa = "WheraOwnerTenant.Horarios.Empresa";
    private const string AberturaPadrao = "08:00", FechamentoPadrao = "18:00", SaidaAlmocoPadrao = "12:00", VoltaAlmocoPadrao = "13:00";

    private static readonly string[][] Dias =
    {
        new[] { "Segunda", "Segunda-feira" }, new[] { "Terca", "Terça-feira" }, new[] { "Quarta", "Quarta-feira" },
        new[] { "Quinta", "Quinta-feira" }, new[] { "Sexta", "Sexta-feira" }, new[] { "Sabado", "Sábado" }, new[] { "Domingo", "Domingo" }
    };

    public class LinhaDia
    {
        public string Dia { get; set; }
        public string Nome { get; set; }
        public bool Aberto { get; set; }
        public string Abre { get; set; }
        public string Fecha { get; set; }
        public bool Almoco { get; set; }
        public string Saida { get; set; }
        public string Volta { get; set; }
        public string Fixos { get; set; }
    }

    protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) Carregar(); }

    private void Carregar()
    {
        try
        {
            var empresa = TenantApi.MinhaEmpresa(Api);
            if (empresa == null) { Mostrar("Nenhuma empresa vinculada."); return; }
            Session[ChaveEmpresa] = empresa;
            var semana = TenantApi.Semana(Api, empresa.Id);
            var dias = semana == null || semana.Dias == null ? new List<DiaFuncionamento>() : semana.Dias;
            var semCadastro = dias.All(d => d.Intervalos == null || d.Intervalos.Count == 0);
            rptDias.DataSource = Dias.Select(d => Montar(d[0], d[1], dias.FirstOrDefault(x => x.DiaSemana == d[0]), semCadastro)).ToList();
            rptDias.DataBind();
            pnlSugestao.Visible = semCadastro;
            litAviso.Text = semCadastro ? "Sugestão ainda não salva." : "Os horários aparecem no seu perfil.";
            pnlConteudo.Visible = true;
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    /// <summary>Sem nenhum horário cadastrado, sugere segunda a sexta das 8h às 18h. Nada é gravado até a pessoa salvar.</summary>
    private static LinhaDia Montar(string dia, string nome, DiaFuncionamento atual, bool semCadastro)
    {
        var linha = new LinhaDia { Dia = dia, Nome = nome, Abre = AberturaPadrao, Fecha = FechamentoPadrao, Saida = SaidaAlmocoPadrao, Volta = VoltaAlmocoPadrao, Fixos = "" };
        var periodos = atual == null || atual.Intervalos == null ? new List<IntervaloFuncionamento>() : atual.Intervalos;
        if (semCadastro) { linha.Aberto = dia != "Sabado" && dia != "Domingo"; return linha; }
        linha.Aberto = periodos.Count > 0;
        if (periodos.Count == 1) { linha.Abre = Hora(periodos[0].Abertura); linha.Fecha = Hora(periodos[0].Fechamento); }
        else if (periodos.Count == 2)
        {
            linha.Almoco = true;
            linha.Abre = Hora(periodos[0].Abertura); linha.Saida = Hora(periodos[0].Fechamento);
            linha.Volta = Hora(periodos[1].Abertura); linha.Fecha = Hora(periodos[1].Fechamento);
        }
        else if (periodos.Count > 2) linha.Fixos = string.Join(";", periodos.Select(p => Hora(p.Abertura) + "-" + Hora(p.Fechamento)));
        return linha;
    }

    private static string Hora(string valor) { return string.IsNullOrWhiteSpace(valor) ? "" : valor.Length >= 5 ? valor.Substring(0, 5) : valor; }

    protected void btnSalvar_Click(object sender, EventArgs e)
    {
        var empresa = Session[ChaveEmpresa] as PerfilEmpresa;
        if (empresa == null) { Carregar(); return; }
        var horarios = new List<object>();
        foreach (RepeaterItem item in rptDias.Items)
        {
            var dia = ((HiddenField)item.FindControl("hidDia")).Value;
            var nome = ((HiddenField)item.FindControl("hidNome")).Value;
            var fixos = ((HiddenField)item.FindControl("hidFixos")).Value;
            if (!((CheckBox)item.FindControl("chkAberto")).Checked) continue;
            if (fixos.Length > 0)
            {
                foreach (var periodo in fixos.Split(';')) { var p = periodo.Split('-'); horarios.Add(new { DiaSemana = dia, Abertura = p[0], Fechamento = p[1] }); }
                continue;
            }
            string abre = Texto(item, "txtAbre"), fecha = Texto(item, "txtFecha"), saida = Texto(item, "txtSaida"), volta = Texto(item, "txtVolta");
            var almoco = ((CheckBox)item.FindControl("chkAlmoco")).Checked;
            var problema = Validar(nome, abre, fecha, almoco, saida, volta);
            if (problema != null) { Mostrar(problema); return; }
            if (almoco)
            {
                horarios.Add(new { DiaSemana = dia, Abertura = abre, Fechamento = saida });
                horarios.Add(new { DiaSemana = dia, Abertura = volta, Fechamento = fecha });
            }
            else horarios.Add(new { DiaSemana = dia, Abertura = abre, Fechamento = fecha });
        }
        try
        {
            TenantApi.DefinirSemana(Api, empresa.Id, new { Horarios = horarios });
            pnlSugestao.Visible = false;
            litAviso.Text = "Os horários aparecem no seu perfil.";
            pnlSucesso.Visible = true;
            litSucesso.Text = "Horários salvos.";
        }
        catch (WheraApiException ex) { Erro(ex); }
    }

    private static string Texto(RepeaterItem item, string id) { return ((TextBox)item.FindControl(id)).Text.Trim(); }

    private static string Validar(string nome, string abre, string fecha, bool almoco, string saida, string volta)
    {
        int inicio, fim;
        if (!Minutos(abre, false, out inicio) || !Minutos(fecha, true, out fim)) return nome + ": informe o horário em que abre e em que fecha.";
        if (inicio >= fim) return nome + ": o horário de abrir precisa ser antes do de fechar. Para passar da meia-noite, feche às 00:00 e continue no dia seguinte.";
        if (!almoco) return null;
        int sai, retorno;
        if (!Minutos(saida, true, out sai) || !Minutos(volta, false, out retorno)) return nome + ": informe a saída e a volta do almoço.";
        if (sai <= inicio || retorno <= sai || retorno >= fim) return nome + ": o almoço precisa ficar entre a abertura e o fechamento, com a saída antes da volta.";
        return null;
    }

    /// <summary>Em horário de fechamento, 00:00 é meia-noite do fim do dia, como na API.</summary>
    private static bool Minutos(string valor, bool fechamento, out int minutos)
    {
        minutos = 0;
        DateTime hora;
        if (!DateTime.TryParseExact(valor, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out hora)) return false;
        minutos = hora.Hour * 60 + hora.Minute;
        if (fechamento && minutos == 0) minutos = 24 * 60;
        return true;
    }

    private void Erro(WheraApiException ex) { if (ex.StatusCode == 401) { TratarSessaoRecusada(ex); return; } Mostrar(ex.StatusCode == 403 ? "Você não tem permissão para editar os horários desta empresa." : ex.Message); }
    private void Mostrar(string texto) { pnlErro.Visible = true; litErro.Text = HttpUtility.HtmlEncode(texto); }
}
