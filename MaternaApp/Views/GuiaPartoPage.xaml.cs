
using System.Text;
using System.Text.Json;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;

namespace MaternaApp.Views;

public partial class GuiaPartoPage : ContentPage
{
    public int GestanteId { get; }
    public string NomeGestante { get; }

    private readonly Dictionary<string, string> _preferencias = new();

    public GuiaPartoPage() : this(0, string.Empty)
    {
    }

    public GuiaPartoPage(int gestanteId, string nomeGestante)
    {
        InitializeComponent();

        GestanteId = gestanteId;
        NomeGestante = nomeGestante;

        CarregarPlano();
        AtualizarProgresso();
    }

    private void OnOpcaoChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || sender is not RadioButton radio)
            return;

        _preferencias[radio.GroupName] = radio.Value?.ToString() ?? string.Empty;
        AtualizarProgresso();
    }

    private void AtualizarProgresso()
    {
        ProgressoLabel.Text = $"{_preferencias.Count}/6";
    }

    private string ChavePlano => $"PlanoParto_{GestanteId}";

    private void CarregarPlano()
    {
        var json = Preferences.Get(ChavePlano, string.Empty);

        if (string.IsNullOrWhiteSpace(json))
            return;

        var dados = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        if (dados == null)
            return;

        foreach (var item in dados)
            _preferencias[item.Key] = item.Value;

        foreach (var radio in ObterRadios())
        {
            if (_preferencias.TryGetValue(radio.GroupName, out var valor) &&
                radio.Value?.ToString() == valor)
            {
                radio.IsChecked = true;
            }
        }
    }

    private IEnumerable<RadioButton> ObterRadios()
    {
        return PercorrerRadios(this);
    }

    private static IEnumerable<RadioButton> PercorrerRadios(IView view)
    {
        if (view is RadioButton radio)
            yield return radio;

        if (view is not IVisualTreeElement elemento)
            yield break;

        foreach (var filho in elemento.GetVisualChildren())
        {
            if (filho is IView filhoView)
            {
                foreach (var resultado in PercorrerRadios(filhoView))
                    yield return resultado;
            }
        }
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        Preferences.Set(ChavePlano, JsonSerializer.Serialize(_preferencias));

        await DisplayAlertAsync(
            "Plano salvo",
            "Suas preferências foram salvas neste dispositivo.",
            "OK");
    }

    private async void OnCompartilharClicked(object sender, EventArgs e)
    {
        if (_preferencias.Count == 0)
        {
            await DisplayAlertAsync("Atenção", "Preencha pelo menos uma preferência antes de compartilhar.", "OK");
            return;
        }

        var texto = new StringBuilder();
        texto.AppendLine("PLANO DE PARTO");
        texto.AppendLine($"Gestante: {NomeGestante}");
        texto.AppendLine();

        var nomes = new Dictionary<string, string>
        {
            ["Acompanhante"] = "Acompanhante",
            ["TipoParto"] = "Tipo de parto preferido",
            ["AlivioDor"] = "Alívio da dor",
            ["Cordao"] = "Corte do cordão umbilical",
            ["PelePele"] = "Contato pele a pele",
            ["Amamentacao"] = "Amamentação"
        };

        foreach (var item in nomes)
        {
            texto.AppendLine($"{item.Value}: {_preferencias.GetValueOrDefault(item.Key, "Não informado")}");
        }

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Compartilhar Plano de Parto",
            Text = texto.ToString()
        });
    }

    private async void OnDiarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage());
    }

    private async void OnSemanarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaSemanarioPage(GestanteId, NomeGestante));
    }

    private async void OnSuplementosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaSuplementosPage(GestanteId, NomeGestante));
    }

    private async void OnInicioTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

    private async void OnAgendaMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AgendaPage());
    }

    private async void OnChatMenuTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Chat", "A tela de conversa ainda não foi implementada.", "OK");
    }

    private async void OnGuiaMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage());
    }

    private async void OnPerfilMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PerfilPage(GestanteId, NomeGestante));
    }
}
