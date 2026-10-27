using System;

namespace MaternaApp.Views;

public partial class PainelProfissionalPage : ContentPage
{
    public PainelProfissionalPage()
    {
        InitializeComponent();
    }

    private void OnAbrirModalCadastroTapped(object sender, EventArgs e)
    {
        ModalCadastroGrid.IsVisible = true;
    }

    // Adicionado: manipulador para o evento Clicked do botão "+ Nova Gestante"
    private void OnAbrirModalCadastroClicked(object sender, EventArgs e)
    {
        ModalCadastroGrid.IsVisible = true;
    }

    private void OnFecharModalCadastroTapped(object sender, EventArgs e)
    {
        ModalCadastroGrid.IsVisible = false;
    }

    private void OnFecharModalCadastroClicked(object sender, EventArgs e)
    {
        ModalCadastroGrid.IsVisible = false;
    }

    private void OnFecharModalTrocarSenhaTapped(object sender, EventArgs e)
    {
        ModalTrocarSenhaGrid.IsVisible = false;
    }

    private void OnFecharModalTrocarSenhaClicked(object sender, EventArgs e)
    {
        ModalTrocarSenhaGrid.IsVisible = false;
    }

    private async void OnSalvarNovaGestanteClicked(object sender, EventArgs e)
    {
        // Ajustado para usar os nomes x:Name definidos no XAML
        var txtNomeGestante = this.FindByName<Microsoft.Maui.Controls.Entry>("txtNomeGestante");
        var txtCpfGestante = this.FindByName<Microsoft.Maui.Controls.Entry>("txtCpfGestante");
        var txtSenhaInicialGestante = this.FindByName<Microsoft.Maui.Controls.Entry>("txtSenhaInicialGestante");

        if (string.IsNullOrWhiteSpace(txtNomeGestante?.Text) || string.IsNullOrWhiteSpace(txtCpfGestante?.Text))
        {
            await DisplayAlert("Atenção", "Preencha pelo menos o Nome e o CPF da gestante.", "OK");
            return;
        }

        string nome = txtNomeGestante.Text;
        string cpf = txtCpfGestante.Text;
        string senha = txtSenhaInicialGestante?.Text ?? string.Empty;

        ModalCadastroGrid.IsVisible = false;

        await DisplayAlert("Sucesso", $"Gestante {nome} cadastrada com sucesso!\n\nLogin: {cpf}\nSenha: {senha}", "OK");

        txtNomeGestante.Text = string.Empty;
        txtCpfGestante.Text = string.Empty;
        if (txtSenhaInicialGestante != null) txtSenhaInicialGestante.Text = string.Empty;
    }

    // Adicionado: manipulador para o evento Clicked do botão "Alterar Senha"
    private async void OnTrocarSenhaClicked(object sender, EventArgs e)
    {
        // Reutiliza a mesma lógica que estava em OnTrocarSenhaTapped
        string novaSenha = await DisplayPromptAsync("Redefinir Senha", "Digite a nova senha para Maria Silva:", placeholder: "Nova senha");

        if (!string.IsNullOrWhiteSpace(novaSenha))
        {
            await DisplayAlert("Senha Alterada", "A senha da gestante foi atualizada com sucesso no sistema!", "OK");
        }
    }

    private async void OnTrocarSenhaTapped(object sender, EventArgs e)
    {
        string novaSenha = await DisplayPromptAsync("Redefinir Senha", "Digite a nova senha para Maria Silva:", placeholder: "Nova senha");

        if (!string.IsNullOrWhiteSpace(novaSenha))
        {
            await DisplayAlert("Senha Alterada", "A senha da gestante foi atualizada com sucesso no sistema!", "OK");
        }
    }

    private void OnToggleNovaSenhaClicked(object sender, EventArgs e)
    {
        if (txtNovaSenhaGestante != null)
        {
            txtNovaSenhaGestante.IsPassword = !txtNovaSenhaGestante.IsPassword;
        }

        if (btnToggleNovaSenha != null)
        {
            btnToggleNovaSenha.Source = txtNovaSenhaGestante.IsPassword ? "eye_closed.png" : "eye_open.png";
        }
    }

    private void OnToggleConfirmarSenhaClicked(object sender, EventArgs e)
    {
        if (txtConfirmarNovaSenhaGestante != null)
        {
            txtConfirmarNovaSenhaGestante.IsPassword = !txtConfirmarNovaSenhaGestante.IsPassword;
        }

        if (btnToggleConfirmarSenha != null)
        {
            btnToggleConfirmarSenha.Source = txtConfirmarNovaSenhaGestante.IsPassword ? "eye_closed.png" : "eye_open.png";
        }
    }

    private async void OnSalvarNovaSenhaClicked(object sender, EventArgs e)
    {
        var nova = txtNovaSenhaGestante?.Text ?? string.Empty;
        var confirmar = txtConfirmarNovaSenhaGestante?.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nova) || nova.Length < 6)
        {
            await DisplayAlert("Erro", "A nova senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        if (nova != confirmar)
        {
            await DisplayAlert("Erro", "As senhas não coincidem.", "OK");
            return;
        }

        ModalTrocarSenhaGrid.IsVisible = false;
        await DisplayAlert("Sucesso", "Senha atualizada com sucesso.", "OK");
    }
}
