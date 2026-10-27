namespace MaternaApp.Views;

public partial class Selecaoperfilpage : ContentPage
{
    public Selecaoperfilpage()
    {
        InitializeComponent();
    }

    private async void OnGestanteTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginGestantePage());
    }

    private async void OnProfissionalTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginProfissionalPage());
    }
}