namespace MaternaApp.Views;

public partial class PerfilPage : ContentPage
{
    public PerfilPage()
    {
        InitializeComponent();
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
            await DisplayAlertAsync("Aviso", "Não foi possível carregar a imagem.", "OK");
        }
    }

    private async void OnInicioTapped(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new HomeGestantePage());
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
            app.Windows[0].Page = new NavigationPage(new Selecaoperfilpage());
        }
        else if (app != null)
        {
            app.OpenWindow(new Window(new NavigationPage(new Selecaoperfilpage())));
        }
    }
}