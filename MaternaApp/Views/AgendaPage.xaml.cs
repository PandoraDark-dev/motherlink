namespace MaternaApp.Views;

public partial class AgendaPage : ContentPage
{
    public AgendaPage()
    {
        InitializeComponent();
    }

    private async void OnInicioTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}