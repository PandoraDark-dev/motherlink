
namespace MaternaApp.Views;

public partial class PerfilPage : ContentPage
{
    private readonly int gestanteId;
    private readonly string nomeGestant;

    public PerfilPage(int gestanteId, string nomeGestant)
    {
        InitializeComponent();

        this.gestanteId = gestanteId;
        this.nomeGestant = nomeGestant;
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
        catch (Exception)
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
            new HomeGestantePage(gestanteId, nomeGestant));
    }

    private async void OnAgendaTapped(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgendaPage());
    }

    private void OnChatTapped(object? sender, EventArgs e)
    {
    }

    private void OnGuiaTapped(object? sender, EventArgs e)
    {
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
