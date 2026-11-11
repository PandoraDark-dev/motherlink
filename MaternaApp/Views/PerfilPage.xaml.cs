
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class PerfilPage : ContentPage
{
    private readonly ApiService _api = new();
    private readonly int gestanteId;
    private readonly string nomeGestante;

    public PerfilPage(int gestanteId, string nomeGestante)
    {
        InitializeComponent();

        this.gestanteId = gestanteId;
        this.nomeGestante = nomeGestante;

        lblNomeCabecalho.Text = nomeGestante;
        lblNome.Text = nomeGestante;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarPerfil();
    }

    private async Task CarregarPerfil()
    {
        try
        {
            var gestante = await _api.BuscarGestantePorId(gestanteId);

            if (gestante == null)
            {
                await DisplayAlert("Erro", "Gestante não encontrada.", "OK");
                return;
            }

            lblNomeCabecalho.Text = gestante.Nome;
            lblNome.Text = gestante.Nome;
            lblCpf.Text = gestante.CPF;
            lblCartaoSUS.Text = gestante.CartaoSUS;
            lblDum.Text = gestante.DUM.ToString("dd/MM/yyyy");
            lblDpp.Text = gestante.DPP.ToString("dd/MM/yyyy");

            var diasGestacao = Math.Max(0, (DateTime.Today - gestante.DUM.Date).Days);
            var semanas = diasGestacao / 7;

            lblSemanaCabecalho.Text = $"Gestante · {semanas}ª semana";
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erro",
                $"Não foi possível carregar seu perfil.\n{ex.Message}",
                "OK");
        }
    }

    private async void OnAlterarFotoTapped(object? sender, EventArgs e)
    {
        try
        {
            var fotos = await MediaPicker.PickPhotosAsync();
            var foto = fotos.Count > 0 ? fotos[0] : null;

            if (foto != null)
            {
                var stream = await foto.OpenReadAsync();
                FotoPerfilImage.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch
        {
            await DisplayAlertAsync(
                "Aviso",
                "Não foi possível carregar a imagem.",
                "OK");
        }
    }

    private void OnInicioTapped(object? sender, EventArgs e)
    {
        Application.Current!.MainPage = new NavigationPage(
            new HomeGestantePage(gestanteId, nomeGestante));
    }

    private async void OnAgendaTapped(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgendaPage(gestanteId));
    }

    private void OnChatTapped(object? sender, EventArgs e)
    {
    }

    private async void OnGuiaTapped(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage());
    }

    private void OnSairTapped(object? sender, EventArgs e)
    {
        var app = Application.Current;

        if (app?.Windows.Count > 0)
        {
            app.Windows[0].Page =
                new NavigationPage(new Selecaoperfilpage());
        }
        else if (app != null)
        {
            app.OpenWindow(
                new Window(new NavigationPage(new Selecaoperfilpage())));
        }
    }
}
