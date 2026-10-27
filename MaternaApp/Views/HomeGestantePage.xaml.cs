namespace MaternaApp.Views;

public partial class HomeGestantePage : ContentPage
{
    public HomeGestantePage()
    {
        InitializeComponent();
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
        await Navigation.PushAsync(new PerfilPage());
    }
}