
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class EsusPage : ContentPage
{
    private readonly ApiService _api = new();
    private List<GestanteResponse> _gestantes = new();

    public EsusPage()
    {
        InitializeComponent();
        CarregarGestantes();
    }

    private async void CarregarGestantes()
    {
        try
        {
            _gestantes = await _api.ListarGestantes();
            pkGestante.ItemsSource = _gestantes;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar as gestantes.\n{ex.Message}", "OK");
        }
    }

    private async void OnGestanteSelecionada(object sender, EventArgs e)
    {
        if (pkGestante.SelectedItem is not GestanteResponse gestante)
            return;

        lblNome.Text = gestante.Nome;
        lblCpf.Text = gestante.CPF;
        lblSus.Text = string.IsNullOrWhiteSpace(gestante.CartaoSUS)
            ? "Não informado"
            : gestante.CartaoSUS;
        lblDum.Text = gestante.DUM.ToString("dd/MM/yyyy");
        lblDpp.Text = gestante.DPP.ToString("dd/MM/yyyy");

        try
        {
            var consultas = await _api.ListarConsultasGestante(gestante.Id);

            ListaConsultas.ItemsSource = consultas
                .OrderByDescending(c => c.Data)
                .ToList();
        }
        catch (Exception ex)
        {
            ListaConsultas.ItemsSource = null;
            await DisplayAlert("Erro", $"Não foi possível carregar o histórico.\n{ex.Message}", "OK");
        }
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
