using System;
using System.Linq;
using System.Net.Http;
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class LoginGestantePage : ContentPage
{
    private bool _usandoCpf = true;

    public LoginGestantePage()
    {
        InitializeComponent();
        AtualizarTipoDocumento();
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync();
        else
            Application.Current!.MainPage = new NavigationPage(new Selecaoperfilpage());
    }

    private void OnSelectCpf(object sender, EventArgs e)
    {
        _usandoCpf = true;
        AtualizarTipoDocumento();
        txtDocumento.Text = string.Empty;
    }

    private void OnSelectSus(object sender, EventArgs e)
    {
        _usandoCpf = false;
        AtualizarTipoDocumento();
        txtDocumento.Text = string.Empty;
    }

    private void AtualizarTipoDocumento()
    {
        lblTipoDoc.Text = _usandoCpf ? "CPF" : "CARTÃO SUS";
        txtDocumento.Placeholder = _usandoCpf ? "Digite seu CPF" : "Digite seu Cartão SUS";

        btnCpf.BackgroundColor = _usandoCpf ? Colors.White : Colors.Transparent;
        btnCpf.TextColor = _usandoCpf ? Color.FromArgb("#168A75") : Color.FromArgb("#718582");
        btnCpf.FontAttributes = _usandoCpf ? FontAttributes.Bold : FontAttributes.None;

        btnSus.BackgroundColor = _usandoCpf ? Colors.Transparent : Colors.White;
        btnSus.TextColor = _usandoCpf ? Color.FromArgb("#718582") : Color.FromArgb("#168A75");
        btnSus.FontAttributes = _usandoCpf ? FontAttributes.None : FontAttributes.Bold;
    }

    private void OnMostrarSenhaTapped(object sender, EventArgs e)
    {
        txtSenha.IsPassword = !txtSenha.IsPassword;
        lblOlho.Opacity = txtSenha.IsPassword ? 1.0 : 0.5;
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        var documentoDigitado = txtDocumento.Text?.Trim();
        var senha = txtSenha.Text;

        if (string.IsNullOrWhiteSpace(documentoDigitado) ||
            string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Preencha todos os campos.", "OK");
            return;
        }

        var documento = new string(documentoDigitado.Where(char.IsDigit).ToArray());

        if (_usandoCpf && documento.Length != 11)
        {
            await DisplayAlert("CPF inválido", "Digite os 11 números do CPF.", "OK");
            return;
        }

        if (!_usandoCpf && documento.Length != 15)
        {
            await DisplayAlert("Cartão SUS inválido", "Confira o número do Cartão SUS.", "OK");
            return;
        }

        try
        {
            var api = new ApiService();
            var resultado = await api.Login(
                _usandoCpf ? documento : string.Empty,
                _usandoCpf ? string.Empty : documento,
                senha);

            if (resultado == null || !resultado.Sucesso)
            {
                await DisplayAlert("Não foi possível entrar",
                    resultado?.Mensagem ?? "A API não retornou dados.", "OK");
                return;
            }

            if (resultado.TipoUsuario != "Gestante")
            {
                await DisplayAlert("Acesso incorreto",
                    "Esta conta não pertence a uma gestante.", "OK");
                return;
            }

            Application.Current!.MainPage = new NavigationPage(new HomeGestantePage());
        }
        catch (TaskCanceledException)
        {
            await DisplayAlert("Erro de conexão",
                "A API demorou para responder. Verifique se o backend está rodando na porta 5240.", "OK");
        }
        catch (HttpRequestException)
        {
            await DisplayAlert("Erro de conexão",
                "Não foi possível conectar à API. Verifique se ela está em execução.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
}