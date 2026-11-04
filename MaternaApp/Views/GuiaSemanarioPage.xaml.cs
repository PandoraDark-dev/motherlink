
namespace MaternaApp.Views;

public partial class GuiaSemanarioPage : ContentPage
{
    public int GestanteId { get; }
    public string NomeGestante { get; }

    private readonly int _semanaGestacionalAtual = 24;
    private Button? _botaoSemanaSelecionado;

    public GuiaSemanarioPage() : this(0, string.Empty)
    {
    }

    public GuiaSemanarioPage(int gestanteId, string nomeGestante)
    {
        InitializeComponent();

        GestanteId = gestanteId;
        NomeGestante = nomeGestante;

        AtualizarSemana(_semanaGestacionalAtual);

        var botaoAtual = FindByName($"BotaoSemana{_semanaGestacionalAtual}") as Button;

        if (botaoAtual != null)
            SelecionarBotaoSemana(botaoAtual);
    }

    private void OnSemanaClicked(object sender, EventArgs e)
    {
        if (sender is Button botao && int.TryParse(botao.Text, out int semana))
        {
            SelecionarBotaoSemana(botao);
            AtualizarSemana(semana);
        }
    }

    private void OnVoceEstaAquiClicked(object sender, EventArgs e)
    {
        var botaoAtual = FindByName($"BotaoSemana{_semanaGestacionalAtual}") as Button;

        if (botaoAtual != null)
            SelecionarBotaoSemana(botaoAtual);

        AtualizarSemana(_semanaGestacionalAtual);
    }

    private void SelecionarBotaoSemana(Button botao)
    {
        if (_botaoSemanaSelecionado != null)
        {
            _botaoSemanaSelecionado.BackgroundColor = Colors.White;
            _botaoSemanaSelecionado.TextColor = Color.FromArgb("#1A9E85");
        }

        botao.BackgroundColor = Color.FromArgb("#1A9E85");
        botao.TextColor = Colors.White;
        _botaoSemanaSelecionado = botao;
    }

    private void AtualizarSemana(int semana)
    {
        SemanaAtualLabel.Text = $"{semana}ª";
        SemanaTituloLabel.Text = $"Semana {semana}";
        ProgressoTextoLabel.Text = $"{semana} de 40 semanas";
        ProgressoGestacao.Progress = semana / 40.0;

        var dados = semana switch
        {
            6 => ("Tamanho de uma ervilha", "🫛", "Menos de 1 g", "✦ O coração começa a se desenvolver", "✦ Início da formação dos braços e pernas", "Cansaço, enjoo"),
            8 => ("Tamanho de uma framboesa", "🫐", "Aproximadamente 1 g", "✦ Os principais órgãos estão em formação", "✦ Braços e pernas continuam a se desenvolver", "Enjoo, sensibilidade nas mamas"),
            10 => ("Tamanho de um morango", "🍓", "Aproximadamente 4 g", "✦ Os órgãos continuam em desenvolvimento", "✦ Dedos das mãos e dos pés se formam", "Enjoo, cansaço"),
            12 => ("Tamanho de um limão", "🍋", "Aproximadamente 14 g", "✦ Os reflexos começam a aparecer", "✦ O bebê movimenta braços e pernas", "Azia, cansaço"),
            16 => ("Tamanho de um abacate", "🥑", "Aproximadamente 100 g", "✦ O esqueleto continua a se desenvolver", "✦ Os movimentos ficam mais coordenados", "Dor nas costas, azia"),
            20 => ("Tamanho de uma banana", "🍌", "Aproximadamente 300 g", "✦ O bebê reage a sons", "✦ Os movimentos podem começar a ser percebidos", "Cansaço, dor nas costas"),
            24 => ("Tamanho de uma espiga de milho", "🌽", "Aproximadamente 600 g", "✦ Pulmões em desenvolvimento", "✦ Reage à luz e a sons", "Cansaço, inchaço nos pés"),
            28 => ("Tamanho de uma berinjela", "🍆", "Aproximadamente 1 kg", "✦ O cérebro cresce rapidamente", "✦ Os olhos podem abrir e fechar", "Azia, dor nas costas"),
            32 => ("Tamanho de uma abóbora pequena", "🎃", "Aproximadamente 1,7 kg", "✦ O bebê acumula gordura corporal", "✦ Os ossos continuam a amadurecer", "Cansaço, inchaço"),
            36 => ("Tamanho de um mamão", "🥭", "Aproximadamente 2,6 kg", "✦ O bebê continua ganhando peso", "✦ Os órgãos continuam amadurecendo", "Pressão pélvica, cansaço"),
            40 => ("Tamanho de uma abóbora", "🎃", "Aproximadamente 3,4 kg", "✦ O bebê está no período previsto para nascer", "✦ O desenvolvimento varia de bebê para bebê", "Desconforto pélvico, cansaço"),
            _ => ("O bebê continua se desenvolvendo", "👶", "Varia conforme a gestação", "✦ O desenvolvimento muda a cada semana", "✦ Acompanhe as orientações do pré-natal", "Os sintomas variam")
        };

        TamanhoLabel.Text = dados.Item1;
        BebeImagemLabel.Text = dados.Item2;
        PesoLabel.Text = dados.Item3;
        Desenvolvimento1Label.Text = dados.Item4;
        Desenvolvimento2Label.Text = dados.Item5;

        SintomasFlex.Children.Clear();

        foreach (string sintoma in dados.Item6.Split(", "))
        {
            SintomasFlex.Children.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#FFF2D9"),
                StrokeThickness = 0,
                Padding = new Thickness(12, 7),
                Margin = new Thickness(0, 0, 8, 6),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = 18
                },
                Content = new Label
                {
                    Text = sintoma,
                    FontSize = 12,
                    TextColor = Color.FromArgb("#B58A3C")
                }
            });
        }
    }

    private async void OnDiarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage());
    }

    private async void OnSuplementosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaSuplementosPage(GestanteId, NomeGestante));
    }

    private async void OnPlanoPartoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaPartoPage());
    }

    private async void OnInicioTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

    private async void OnAgendaMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AgendaPage());
    }

    private async void OnChatMenuTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Chat", "A tela de conversa ainda não foi implementada.", "OK");
    }

    private async void OnGuiaMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage());
    }

    private async void OnPerfilMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PerfilPage(GestanteId, NomeGestante));
    }
}
