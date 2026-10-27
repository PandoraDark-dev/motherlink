using MaternaApp.Views;

namespace MaternaApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new Selecaoperfilpage());
    }
}