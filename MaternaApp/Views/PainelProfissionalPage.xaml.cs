
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class PainelProfissionalPage : ContentPage
{
    private readonly ApiService _api = new();
    private readonly int _profissionalId;
    private List<GestanteResponse> _gestantes = new();
    private GestanteResponse? _gestanteSelecionada;

    public PainelProfissionalPage(int profissionalId)
    {
        InitializeComponent();

        _profissionalId = profissionalId;
        dpDataConsulta.MinimumDate = DateTime.Today;
        dpDataConsulta.Date = DateTime.Today;
        tpHorarioConsulta.Time = new TimeSpan(9, 0, 0);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarGestantes();
    }

    private async Task CarregarGestantes()
    {
        try
        {
            _gestantes = await _api.ListarGestantes();
            AtualizarLista(_gestantes);

            lblTotalGestantes.Text = _gestantes.Count.ToString();
            lblEmAcompanhamento.Text = _gestantes.Count.ToString();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar as gestantes.\n{ex.Message}", "OK");
        }
    }

    private void AtualizarLista(List<GestanteResponse> gestantes)
    {
        ListaGestantes.ItemsSource = gestantes;
    }

    private void OnBuscarGestanteTextChanged(object sender, TextChangedEventArgs e)
    {
        var busca = e.NewTextValue?.Trim() ?? string.Empty;

        AtualizarLista(_gestantes
            .Where(g => g.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase) ||
                        g.CPF.Contains(busca, StringComparison.OrdinalIgnoreCase))
            .ToList());
    }

    private void OnGestanteSelecionadaChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not GestanteResponse gestante)
            return;

        _gestanteSelecionada = gestante;
        btnAgendarConsulta.IsEnabled = true;

        lblNomeGestanteExemplo.Text = gestante.Nome;
        lblCpfGestante.Text = gestante.CPF;
        lblSusGestante.Text = string.IsNullOrWhiteSpace(gestante.CartaoSUS)
            ? "Não informado"
            : gestante.CartaoSUS;
        lblDumGestante.Text = gestante.DUM.ToString("dd/MM/yyyy");
        lblDppGestante.Text = gestante.DPP.ToString("dd/MM/yyyy");
        lblStatusGestante.Text = "Ativa";

        ListaGestantes.SelectedItem = null;
    }

    private void OnAbrirModalCadastroClicked(object sender, EventArgs e)
    {
        dpDUMGestante.Date = DateTime.Today;
        dpDPPGestante.Date = DateTime.Today.AddMonths(7);
        ModalCadastroGrid.IsVisible = true;
    }

    private void OnFecharModalCadastroClicked(object sender, EventArgs e)
    {
        ModalCadastroGrid.IsVisible = false;
    }

    private async void OnSalvarNovaGestanteClicked(object sender, EventArgs e)
    {
        var nome = txtNomeGestante.Text?.Trim() ?? string.Empty;
        var cpf = new string((txtCpfGestante.Text ?? string.Empty)
            .Where(char.IsDigit).ToArray());
        var cartaoSUS = txtCartaoSUSGestante.Text?.Trim() ?? string.Empty;
        var senha = txtSenhaInicialGestante.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nome) || cpf.Length != 11 ||
            string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Informe nome, CPF válido e senha.", "OK");
            return;
        }

        if (dpDPPGestante.Date.GetValueOrDefault() <
            dpDUMGestante.Date.GetValueOrDefault())
        {
            await DisplayAlert("Atenção", "A DPP não pode ser anterior à DUM.", "OK");
            return;
        }

        try
        {
            var novaGestante = new GestanteRequest
            {
                Nome = nome,
                CPF = cpf,
                CartaoSUS = cartaoSUS,
                Senha = senha,
                DUM = dpDUMGestante.Date.GetValueOrDefault(),
                DPP = dpDPPGestante.Date.GetValueOrDefault()
            };

            await _api.CadastrarGestante(novaGestante);

            ModalCadastroGrid.IsVisible = false;

            txtNomeGestante.Text = string.Empty;
            txtCpfGestante.Text = string.Empty;
            txtCartaoSUSGestante.Text = string.Empty;
            txtSenhaInicialGestante.Text = string.Empty;

            await CarregarGestantes();

            await DisplayAlert(
                "Cadastro realizado",
                $"Gestante {nome} cadastrada com sucesso!\n\nLogin: {cpf}\nSenha: {senha}",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro no cadastro", ex.Message, "OK");
        }
    }

    private void OnAbrirModalConsultaClicked(object sender, EventArgs e)
    {
        if (_gestanteSelecionada == null)
            return;

        lblGestanteConsulta.Text = _gestanteSelecionada.Nome;
        dpDataConsulta.Date = DateTime.Today;
        tpHorarioConsulta.Time = new TimeSpan(9, 0, 0);
        pkTipoConsulta.SelectedIndex = -1;
        txtObservacoesConsulta.Text = string.Empty;

        ModalConsultaGrid.IsVisible = true;
    }

    private void OnFecharModalConsultaClicked(object sender, EventArgs e)
    {
        ModalConsultaGrid.IsVisible = false;
    }

    private async void OnSalvarConsultaClicked(object sender, EventArgs e)
    {
        if (_gestanteSelecionada == null)
        {
            await DisplayAlert("Atenção", "Selecione uma gestante.", "OK");
            return;
        }

        if (pkTipoConsulta.SelectedIndex < 0)
        {
            await DisplayAlert("Atenção", "Selecione o tipo de consulta.", "OK");
            return;
        }

        var dataConsulta = dpDataConsulta.Date.GetValueOrDefault().Date
                           + tpHorarioConsulta.Time;

        if (dataConsulta <= DateTime.Now)
        {
            await DisplayAlert("Atenção", "Escolha uma data e horário futuros.", "OK");
            return;
        }

        try
        {
            var consulta = new ConsultaRequest
            {
                GestanteId = _gestanteSelecionada.Id,
                ProfissionalId = _profissionalId,
                Data = dataConsulta.GetValueOrDefault(),
                Tipo = pkTipoConsulta.Items[pkTipoConsulta.SelectedIndex],
                Observacoes = txtObservacoesConsulta.Text?.Trim() ?? string.Empty
            };

            await _api.CadastrarConsulta(consulta);

            ModalConsultaGrid.IsVisible = false;

            await DisplayAlert(
                "Consulta agendada",
                $"Consulta de {_gestanteSelecionada.Nome} agendada para {dataConsulta:dd/MM/yyyy às HH:mm}.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao agendar", ex.Message, "OK");
        }
    }

    private async void OnSairClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Sair",
            "Deseja realmente sair da conta?",
            "Sim",
            "Cancelar");

        if (confirmar)
        {
            Application.Current!.MainPage =
                new NavigationPage(new Selecaoperfilpage());
        }
    }
}
