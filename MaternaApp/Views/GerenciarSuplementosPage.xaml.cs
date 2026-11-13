
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class GerenciarSuplementosPage : ContentPage
{
    private readonly ApiService _api = new();
    private readonly int _gestanteId;
    private readonly int _profissionalId;
    private int? _suplementoEditandoId;

    public GerenciarSuplementosPage(
        int gestanteId,
        string nomeGestante,
        int profissionalId)
    {
        InitializeComponent();

        _gestanteId = gestanteId;
        _profissionalId = profissionalId;
        lblNomeGestante.Text = nomeGestante;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarSuplementos();
    }

    private async Task CarregarSuplementos()
    {
        try
        {
            ListaSuplementosProfissional.ItemsSource =
                await _api.ListarSuplementos(_gestanteId);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void OnSalvarSuplementoClicked(object sender, EventArgs e)
    {
        var nome = txtNomeSuplemento.Text?.Trim() ?? string.Empty;
        var dosagem = txtDosagemSuplemento.Text?.Trim() ?? string.Empty;
        var horarios = (txtHorariosSuplemento.Text ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
        var orientacoes = txtOrientacoesSuplemento.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(dosagem) ||
            horarios.Count == 0)
        {
            await DisplayAlert(
                "Atenção",
                "Informe o nome, a dosagem e pelo menos um horário.",
                "OK");
            return;
        }

        try
        {
            var dados = new SuplementoRequest
            {
                GestanteId = _gestanteId,
                ProfissionalId = _profissionalId,
                Nome = nome,
                Dosagem = dosagem,
                Horarios = horarios,
                Orientacoes = orientacoes
            };

            if (_suplementoEditandoId.HasValue)
                await _api.AtualizarSuplemento(_suplementoEditandoId.Value, dados);
            else
                await _api.CadastrarSuplemento(dados);

            LimparFormulario();
            await CarregarSuplementos();

            await DisplayAlert(
                "Sucesso",
                "Suplemento salvo com sucesso.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private void OnEditarSuplementoClicked(object sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: SuplementoResponse suplemento })
            return;

        _suplementoEditandoId = suplemento.Id;

        txtNomeSuplemento.Text = suplemento.Nome;
        txtDosagemSuplemento.Text = suplemento.Dosagem;
        txtHorariosSuplemento.Text = string.Join(", ", suplemento.Horarios);
        txtOrientacoesSuplemento.Text = suplemento.Orientacoes;

        lblTituloFormulario.Text = "Editar suplemento";
        btnSalvarSuplemento.Text = "Salvar alterações";
        btnCancelarEdicao.IsVisible = true;

        ScrollToTop();
    }

    private async void OnExcluirSuplementoClicked(object sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: SuplementoResponse suplemento })
            return;

        bool confirmar = await DisplayAlert(
            "Excluir suplemento",
            $"Deseja excluir {suplemento.Nome}?",
            "Sim",
            "Cancelar");

        if (!confirmar)
            return;

        try
        {
            await _api.ExcluirSuplemento(suplemento.Id);

            if (_suplementoEditandoId == suplemento.Id)
                LimparFormulario();

            await CarregarSuplementos();

            await DisplayAlert("Sucesso", "Suplemento excluído.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private void OnCancelarEdicaoClicked(object sender, EventArgs e)
    {
        LimparFormulario();
    }

    private void LimparFormulario()
    {
        _suplementoEditandoId = null;

        txtNomeSuplemento.Text = string.Empty;
        txtDosagemSuplemento.Text = string.Empty;
        txtHorariosSuplemento.Text = string.Empty;
        txtOrientacoesSuplemento.Text = string.Empty;

        lblTituloFormulario.Text = "Cadastrar suplemento";
        btnSalvarSuplemento.Text = "Cadastrar suplemento";
        btnCancelarEdicao.IsVisible = false;
    }

    private void ScrollToTop()
    {
        txtNomeSuplemento.Focus();
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
