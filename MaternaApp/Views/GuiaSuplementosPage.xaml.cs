
namespace MaternaApp.Views;

public partial class GuiaSuplementosPage : ContentPage
{
    public int GestanteId { get; }
    public string NomeGestante { get; }

    private readonly string[] _dias = ["Seg", "Ter", "Qua", "Qui", "Sex", "Sáb", "Hoje"];

    private readonly Dictionary<string, bool[]> _historico = new()
    {
        ["Ácido Fólico"] = [true, true, false, true, true, true, false],
        ["Sulfato Ferroso"] = [true, false, true, true, true, false, false],
        ["Cálcio"] = [true, true, true, false, true, true, false]
    };

    private readonly Dictionary<string, (Button Botao, string Cor)> _botoes = new();

    public GuiaSuplementosPage() : this(0, string.Empty)
    {
    }

    public GuiaSuplementosPage(int gestanteId, string nomeGestante)
    {
        InitializeComponent();

        GestanteId = gestanteId;
        NomeGestante = nomeGestante;

        _botoes["Ácido Fólico"] = (AcidoFolicoButton, "#20BF62");
        _botoes["Sulfato Ferroso"] = (FerroButton, "#EF4148");
        _botoes["Cálcio"] = (CalcioButton, "#347FF0");

        AtualizarBotoes();
        AtualizarHistorico();
    }

    private void MarcarSuplemento(string nome)
    {
        _historico[nome][6] = !_historico[nome][6];
        AtualizarBotoes();
        AtualizarHistorico();
    }

    private void AtualizarBotoes()
    {
        foreach (var item in _botoes)
        {
            bool tomado = _historico[item.Key][6];

            item.Value.Botao.Text = tomado ? "✓ Tomei hoje" : "Marcar como tomado";
            item.Value.Botao.BackgroundColor = tomado
                ? Color.FromArgb(item.Value.Cor)
                : Color.FromArgb("#AAB5BC");
        }
    }

    private void AtualizarHistorico()
    {
        while (HistoricoStack.Children.Count > 1)
            HistoricoStack.Children.RemoveAt(1);

        foreach (var item in _historico)
        {
            HistoricoStack.Children.Add(new Label
            {
                Text = item.Key,
                FontSize = 13,
                TextColor = Color.FromArgb("#77858F")
            });

            var linha = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new(), new(), new(), new(), new(), new(), new()
                },
                ColumnSpacing = 4
            };

            for (int i = 0; i < _dias.Length; i++)
            {
                bool tomado = item.Value[i];
                var cor = Color.FromArgb(_botoes[item.Key].Cor);

                linha.Add(new Border
                {
                    BackgroundColor = tomado ? cor.WithAlpha(0.13f) : Color.FromArgb("#F5F7F8"),
                    StrokeThickness = 0,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                    {
                        CornerRadius = 6
                    },
                    Padding = 5,
                    Content = new Label
                    {
                        Text = tomado ? "✓" : "-",
                        TextColor = tomado ? cor : Color.FromArgb("#C5CDD2"),
                        FontSize = 13,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }, i);
            }

            HistoricoStack.Children.Add(linha);

            var dias = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new(), new(), new(), new(), new(), new(), new()
                }
            };

            for (int i = 0; i < _dias.Length; i++)
            {
                dias.Add(new Label
                {
                    Text = _dias[i],
                    FontSize = 10,
                    TextColor = Color.FromArgb("#9AA7B3"),
                    HorizontalTextAlignment = TextAlignment.Center
                }, i);
            }

            HistoricoStack.Children.Add(dias);
        }
    }

    private void OnAcidoFolicoClicked(object sender, EventArgs e) => MarcarSuplemento("Ácido Fólico");

    private void OnFerroClicked(object sender, EventArgs e) => MarcarSuplemento("Sulfato Ferroso");

    private void OnCalcioClicked(object sender, EventArgs e) => MarcarSuplemento("Cálcio");

    private async void OnDiarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage());
    }

    private async void OnSemanarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaSemanarioPage(GestanteId, NomeGestante));
    }

    private async void OnPlanoPartoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaPartoPage(GestanteId, NomeGestante));
    }

    private async void OnInicioTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

    private async void OnAgendaMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AgendaPage(GestanteId));
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
