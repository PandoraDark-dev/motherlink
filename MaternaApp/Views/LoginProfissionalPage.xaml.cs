
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class LoginProfissionalPage : ContentPage
{
    public LoginProfissionalPage()
    {
        InitializeComponent();
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnMostrarSenhaTapped(object sender, EventArgs e)
    {
        txtSenhaProfissional.IsPassword = !txtSenhaProfissional.IsPassword;
        lblOlho.Opacity = txtSenhaProfissional.IsPassword ? 1.0 : 0.5;
    }


    private async void OnEntrarProfissionalClicked(object sender, EventArgs e)
    {
        var identificacao = txtIdentificacao.Text?.Trim();
        var senha = txtSenhaProfissional.Text;

        if (string.IsNullOrWhiteSpace(identificacao) ||
            string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Erro", "Preencha todos os campos.", "OK");
            return;
        }

        try
        {
            var api = new ApiService();

            var resultado = identificacao.Contains('@')
                ? await api.LoginProfissional(string.Empty, identificacao, senha)
                : await api.LoginProfissional(identificacao, string.Empty, senha);

            if (resultado == null || !resultado.Sucesso)
            {
                await DisplayAlert("Erro", resultado?.Mensagem ?? "A API não retornou dados.", "OK");
                return;
            }

            if (resultado.TipoUsuario != "Profissional")
            {
                await DisplayAlert("Erro", "Este acesso não é de um profissional.", "OK");
                return;
            }

            Application.Current!.MainPage =
                new NavigationPage(new PainelProfissionalPage());
        }
        catch (TaskCanceledException)
        {
            await DisplayAlert("Erro de conexão", "A API demorou demais para responder. Verifique se está rodando na porta 5240.", "OK");
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("Erro de conexão", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

}