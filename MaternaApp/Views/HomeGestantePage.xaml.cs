
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class HomeGestantePage : ContentPage
{
    private readonly ApiService _api = new();

    public int GestanteId { get; }
    public string NomeGestante { get; private set; }

    public HomeGestantePage(int gestanteId, string nomeGestante)
    {
        InitializeComponent();

        GestanteId = gestanteId;
        NomeGestante = nomeGestante;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarDadosGestante();
    }

    private async Task CarregarDadosGestante()
    {
        try
        {
            var gestante = await _api.BuscarGestantePorId(GestanteId);

            if (gestante == null)
            {
                await DisplayAlert("Erro", "Gestante não encontrada.", "OK");
                return;
            }

            NomeGestante = gestante.Nome;
            lblNomeGestante.Text = $"Olá, {NomeGestante}! 💛";

            var hoje = DateTime.Today;
            var diasGestacao = Math.Max(0, (hoje - gestante.DUM.Date).Days);
            var semanas = diasGestacao / 7;
            var dias = diasGestacao % 7;
            var percentual = Math.Clamp(diasGestacao / 280.0 * 100, 0, 100);
            var diasRestantes = (gestante.DPP.Date - hoje).Days;

            lblSemanaGestacao.Text = $"{semanas} semanas e {dias} dias";
            lblPercentual.Text = $"{percentual:F0}%";

            lblDiasRestantes.Text = diasRestantes > 0
                ? $"{diasRestantes} dias restantes"
                : diasRestantes == 0
                    ? "DPP prevista para hoje"
                    : $"DPP ultrapassada há {Math.Abs(diasRestantes)} dias";

            lblDum.Text = gestante.DUM.ToString("dd/MM/yyyy");
            lblDpp.Text = gestante.DPP.ToString("dd/MM/yyyy");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar seus dados.\n{ex.Message}", "OK");
        }
    }

    private async void OnAgendaTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AgendaPage(GestanteId));
    }

    private async void OnChatTapped(object sender, EventArgs e)
    {
        try
        {
            var consultas = await _api.ListarConsultasGestante(GestanteId);

            var profissionalId = consultas
                .OrderByDescending(c => c.Data)
                .Select(c => c.ProfissionalId)
                .FirstOrDefault();

            if (profissionalId <= 0)
            {
                await DisplayAlert("Chat", "Nenhum profissional está vinculado às consultas desta gestante.", "OK");
                return;
            }

            var profissionais = await _api.ListarProfissionais();
            var profissional = profissionais.FirstOrDefault(p => p.Id == profissionalId);

            if (profissional == null)
            {
                await DisplayAlert("Erro", "Profissional não encontrado.", "OK");
                return;
            }

            await Navigation.PushAsync(new ChatPage(
                GestanteId,
                profissionalId,
                GestanteId,
                "gestante",
                profissional.Nome));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void OnGuiaTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage(GestanteId, NomeGestante));
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PerfilPage(GestanteId, NomeGestante));
    }
}
