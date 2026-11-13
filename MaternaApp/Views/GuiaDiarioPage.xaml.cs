
namespace MaternaApp.Views;

public partial class GuiaDiarioPage : ContentPage
{
    private int _horasSono = 4;
    private int _movimentos;

    public int GestanteId { get; }
    public string NomeGestante { get; }

    public GuiaDiarioPage() : this(0, string.Empty)
    {
    }

    public GuiaDiarioPage(int gestanteId, string nomeGestante)
    {
        InitializeComponent();

        GestanteId = gestanteId;
        NomeGestante = nomeGestante;

        AtualizarSono();
    }

    private void OnHumorChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || sender is not CheckBox selecionado)
            return;

        var opcoes = new[]
        {
            HumorFeliz,
            HumorTranquila,
            HumorNeutra,
            HumorCansada,
            HumorTriste
        };

        foreach (var opcao in opcoes)
        {
            if (opcao != selecionado)
                opcao.IsChecked = false;
        }
    }

    private void OnSonoClicked(object sender, EventArgs e)
    {
        if (sender is not Button botao ||
            !int.TryParse(botao.Text, out int horas))
            return;

        _horasSono = horas;
        AtualizarSono();
    }

    private void AtualizarSono()
    {
        var botoes = new[]
        {
            Sono0, Sono2, Sono4, Sono6, Sono8, Sono10
        };

        foreach (var botao in botoes)
        {
            bool selecionado = botao.Text == _horasSono.ToString();

            botao.BackgroundColor = selecionado
                ? Color.FromArgb("#2F9E8F")
                : Color.FromArgb("#F0F3F7");

            botao.TextColor = selecionado
                ? Colors.White
                : Color.FromArgb("#65717B");
        }
    }

    private void OnMovimentoMenosClicked(object sender, EventArgs e)
    {
        if (_movimentos > 0)
            _movimentos--;

        MovimentosLabel.Text = _movimentos.ToString();
    }

    private void OnMovimentoMaisClicked(object sender, EventArgs e)
    {
        _movimentos++;
        MovimentosLabel.Text = _movimentos.ToString();
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        string? humor = null;

        if (HumorFeliz.IsChecked)
            humor = "Feliz";
        else if (HumorTranquila.IsChecked)
            humor = "Tranquila";
        else if (HumorNeutra.IsChecked)
            humor = "Neutra";
        else if (HumorCansada.IsChecked)
            humor = "Cansada";
        else if (HumorTriste.IsChecked)
            humor = "Triste";

        if (humor is null)
        {
            await DisplayAlertAsync("Atenção", "Selecione como você está se sentindo hoje.", "OK");
            return;
        }

        string sintomas = string.Join(", ",
            new[]
            {
                SintomaEnjoo.IsChecked ? "Enjoo" : null,
                SintomaCansaco.IsChecked ? "Cansaço" : null,
                SintomaCabeca.IsChecked ? "Dor de cabeça" : null,
                SintomaInchaco.IsChecked ? "Inchaço nos pés" : null,
                SintomaAzia.IsChecked ? "Azia" : null
            }.Where(s => s is not null));

        if (string.IsNullOrWhiteSpace(sintomas))
            sintomas = "Nenhum sintoma selecionado";

        string observacoes = ObservacoesEditor.Text?.Trim() ?? "";

        await DisplayAlertAsync(
            "Registro preenchido",
            $"Humor: {humor}\n" +
            $"Sono: {_horasSono} horas\n" +
            $"Sintomas: {sintomas}\n" +
            $"Movimentos registrados: {_movimentos}\n" +
            $"Observações: {(string.IsNullOrWhiteSpace(observacoes) ? "Nenhuma" : observacoes)}\n\n" +
            "Os dados ainda não foram enviados para a API.",
            "OK");
    }

    private async void OnDiarioClicked(object sender, EventArgs e)
    {
        await DisplayAlertAsync("Guia Pré-Natal", "Você já está no Diário.", "OK");
    }

    private async void OnSemanarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaSemanarioPage(GestanteId, NomeGestante));
    }

    private async void OnSuplementosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaSuplementosPage(GestanteId, NomeGestante));
    }

    private async void OnPlanoPartoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaPartoPage(GestanteId, NomeGestante));
    }
}
