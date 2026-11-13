
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class PainelEnfermeiraChefePage : ContentPage
{
    private readonly ApiService _api = new();
    private readonly int _profissionalId;

    public PainelEnfermeiraChefePage(int profissionalId)
    {
        InitializeComponent();
        _profissionalId = profissionalId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarProfissionais();
    }

    private async Task CarregarProfissionais()
    {
        try
        {
            var profissionais = await _api.ListarProfissionais();

            ListaProfissionais.ItemsSource = profissionais;
            lblTotalProfissionais.Text = profissionais.Count.ToString();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private void OnAbrirCadastroClicked(object sender, EventArgs e)
    {
        ModalCadastroGrid.IsVisible = true;
    }

    private void OnFecharCadastroClicked(object sender, EventArgs e)
    {
        ModalCadastroGrid.IsVisible = false;
    }

    private async void OnSalvarProfissionalClicked(object sender, EventArgs e)
    {
        var nome = txtNome.Text?.Trim();
        var cpf = txtCPF.Text?.Trim();
        var registro = txtRegistro.Text?.Trim();
        var especialidade = txtEspecialidade.Text?.Trim();
        var email = txtEmail.Text?.Trim();
        var senha = txtSenha.Text;

        if (string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(cpf) ||
            string.IsNullOrWhiteSpace(registro) ||
            string.IsNullOrWhiteSpace(especialidade) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Preencha todos os campos.", "OK");
            return;
        }

        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
        {
            await DisplayAlert("Atenção", "Informe um CPF válido com 11 dígitos.", "OK");
            return;
        }

        try
        {
            await _api.CadastrarProfissional(new ProfissionalRequest
            {
                Nome = nome,
                CPF = cpf,
                Registro = registro,
                Especialidade = especialidade,
                Email = email,
                Senha = senha
            });

            await DisplayAlert("Sucesso", "Profissional cadastrado.", "OK");

            LimparCampos();
            ModalCadastroGrid.IsVisible = false;

            await CarregarProfissionais();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private void LimparCampos()
    {
        txtNome.Text = string.Empty;
        txtCPF.Text = string.Empty;
        txtRegistro.Text = string.Empty;
        txtEspecialidade.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtSenha.Text = string.Empty;
    }

    private async void OnSairClicked(object sender, EventArgs e)
    {
        Application.Current!.MainPage =
            new NavigationPage(new Selecaoperfilpage());
    }
}
