namespace MaternaApp.Views;

public partial class HomeGestantePage : ContentPage
{
    public int GestanteId { get; }
    public string NomeGestante { get; }

    public HomeGestantePage(int gestanteId, string nomeGestante)
    {
        InitializeComponent();

        GestanteId = gestanteId;
        NomeGestante = nomeGestante;
    }

    private async void OnAgendaTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgendaPage());
    }

    private async void OnChatTapped(object sender, EventArgs e)
    {
        // await Navigation.PushAsync(new ChatPage());
    }

    private async void OnGuiaTapped(object sender, EventArgs e)
    {
        // await Navigation.PushAsync(new GuiaPage());
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NavigationPage(new PerfilPage(GestanteId, NomeGestante)));
    }
}
