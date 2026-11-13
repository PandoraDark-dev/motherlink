
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class GuiaSuplementosPage : ContentPage
{
    public int GestanteId { get; }
    public string NomeGestante { get; }

    private readonly ApiService _api = new();
    private List<SuplementoResponse> _suplementos = [];
    private List<RegistroSuplementoResponse> _registros = [];
    private bool _carregando;

    public GuiaSuplementosPage() : this(0, string.Empty)
    {
    }

    public GuiaSuplementosPage(int gestanteId, string nomeGestante)
    {
        InitializeComponent();

        GestanteId = gestanteId;
        NomeGestante = nomeGestante;

        DataHistoricoPicker.Date = DateTime.Today;
        DataHistoricoPicker.MaximumDate = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (GestanteId <= 0)
        {
            SuplementosStack.Children.Clear();
            SuplementosStack.Children.Add(new Label
            {
                Text = "Não foi possível identificar a gestante.",
                TextColor = Color.FromArgb("#81938F"),
                HorizontalTextAlignment = TextAlignment.Center
            });
            return;
        }

        await AtualizarTela();
    }

    private async Task AtualizarTela()
    {
        if (_carregando)
            return;

        _carregando = true;

        try
        {
            _suplementos = (await _api.ListarSuplementos(GestanteId)).ToList();

            _registros = (await _api.ListarHistoricoSuplementos(
                GestanteId, DateTime.Today)).ToList();

            ExibirSuplementos();

            await CarregarHistorico(DataHistoricoPicker.Date ?? DateTime.Today);
            await CarregarHistoricoSemanal();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            _carregando = false;
        }
    }

    private void ExibirSuplementos()
    {
        SuplementosStack.Children.Clear();

        if (_suplementos.Count == 0)
        {
            SuplementosStack.Children.Add(new Label
            {
                Text = "Nenhum suplemento foi cadastrado para você.",
                TextColor = Color.FromArgb("#81938F"),
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 12)
            });

            return;
        }

        foreach (var suplemento in _suplementos)
        {
            var conteudo = new VerticalStackLayout { Spacing = 10 };

            conteudo.Children.Add(new Label
            {
                Text = suplemento.Nome,
                FontSize = 17,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#263746")
            });

            conteudo.Children.Add(new Label
            {
                Text = $"Dosagem: {suplemento.Dosagem}",
                FontSize = 13,
                TextColor = Color.FromArgb("#77858F")
            });

            if (!string.IsNullOrWhiteSpace(suplemento.Orientacoes))
            {
                conteudo.Children.Add(new Label
                {
                    Text = suplemento.Orientacoes,
                    FontSize = 13,
                    TextColor = Color.FromArgb("#77858F")
                });
            }

            foreach (var horario in suplemento.Horarios ?? [])
            {
                bool tomado = _registros.Any(r =>
                    r.SuplementoId == suplemento.Id &&
                    r.Horario == horario &&
                    r.Tomado);

                var botao = new Button
                {
                    Text = tomado
                        ? $"✓ Tomado às {horario}"
                        : $"Marcar dose das {horario} como tomada",
                    BackgroundColor = Color.FromArgb(
                        tomado ? "#20A765" : "#D9534F"),
                    TextColor = Colors.White,
                    CornerRadius = 12,
                    HeightRequest = 46,
                    FontSize = 13
                };

                botao.Clicked += async (_, _) =>
                {
                    botao.IsEnabled = false;
                    await RegistrarDose(suplemento, horario, !tomado);
                    botao.IsEnabled = true;
                };

                conteudo.Children.Add(botao);
            }

            SuplementosStack.Children.Add(new Border
            {
                BackgroundColor = Colors.White,
                Stroke = Color.FromArgb("#E5EFEC"),
                Padding = 18,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = 20
                },
                Content = conteudo
            });
        }
    }

    private async Task RegistrarDose(
        SuplementoResponse suplemento,
        string horario,
        bool tomado)
    {
        try
        {
            await _api.RegistrarDose(
                suplemento.Id,
                new DoseSuplementoRequest
                {
                    Data = DateTime.Today,
                    Horario = horario,
                    Tomado = tomado
                });

            await AtualizarTela();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async Task CarregarHistorico(DateTime data)
    {
        try
        {
            var registros = (await _api.ListarHistoricoSuplementos(
                GestanteId, data)).ToList();

            HistoricoDosesStack.Children.Clear();

            if (_suplementos.Count == 0)
                return;

            foreach (var suplemento in _suplementos)
            {
                foreach (var horario in suplemento.Horarios ?? [])
                {
                    var registro = registros.FirstOrDefault(r =>
                        r.SuplementoId == suplemento.Id &&
                        r.Horario == horario);

                    bool tomado = registro?.Tomado == true;

                    var linha = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto)
                        },
                        ColumnSpacing = 8
                    };

                    linha.Add(new VerticalStackLayout
                    {
                        Spacing = 2,
                        Children =
                        {
                            new Label
                            {
                                Text = suplemento.Nome,
                                FontSize = 13,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Color.FromArgb("#263746")
                            },
                            new Label
                            {
                                Text = $"Dose das {horario}",
                                FontSize = 12,
                                TextColor = Color.FromArgb("#9AA7B3")
                            }
                        }
                    }, 0);

                    linha.Add(new Label
                    {
                        Text = tomado ? "Tomado" : "Não tomado",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb(
                            tomado ? "#20A765" : "#D9534F"),
                        VerticalOptions = LayoutOptions.Center
                    }, 1);

                    HistoricoDosesStack.Children.Add(linha);

                    if (registro != null)
                    {
                        HistoricoDosesStack.Children.Add(new Label
                        {
                            Text = $"Registrado às {registro.RegistradoEm:HH:mm}",
                            FontSize = 11,
                            TextColor = Color.FromArgb("#9AA7B3")
                        });
                    }

                    HistoricoDosesStack.Children.Add(new BoxView
                    {
                        HeightRequest = 1,
                        BackgroundColor = Color.FromArgb("#EEF2F1")
                    });
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async Task CarregarHistoricoSemanal()
    {
        try
        {
            HistoricoSemanalStack.Children.Clear();

            if (_suplementos.Count == 0)
            {
                HistoricoSemanalStack.Children.Add(new Label
                {
                    Text = "Nenhum suplemento cadastrado.",
                    TextColor = Color.FromArgb("#81938F")
                });
                return;
            }

            var hoje = DateTime.Today;
            var inicioSemana = hoje.AddDays(
                -(((int)hoje.DayOfWeek + 6) % 7));

            var dias = Enumerable.Range(0, 7)
                .Select(i => inicioSemana.AddDays(i))
                .ToList();

            var registrosPorDia =
                new Dictionary<DateTime, List<RegistroSuplementoResponse>>();

            foreach (var dia in dias)
            {
                registrosPorDia[dia] =
                    (await _api.ListarHistoricoSuplementos(
                        GestanteId, dia)).ToList();
            }

            var tabela = new Grid
            {
                ColumnSpacing = 5,
                RowSpacing = 8
            };

            tabela.ColumnDefinitions.Add(new ColumnDefinition(110));

            foreach (var dia in dias)
                tabela.ColumnDefinitions.Add(new ColumnDefinition(38));

            tabela.RowDefinitions.Add(new RowDefinition(35));

            tabela.Add(new Label
            {
                Text = "Suplemento",
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#526B66"),
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);

            var nomesDias = new[]
            {
                "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb", "Dom"
            };

            for (int i = 0; i < dias.Count; i++)
            {
                tabela.Add(new Label
                {
                    Text = nomesDias[i],
                    FontSize = 10,
                    TextColor = Color.FromArgb("#77858F"),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                }, i + 1, 0);
            }

            for (int i = 0; i < _suplementos.Count; i++)
            {
                var suplemento = _suplementos[i];

                tabela.RowDefinitions.Add(new RowDefinition(42));

                tabela.Add(new Label
                {
                    Text = suplemento.Nome,
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#263746"),
                    VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.TailTruncation
                }, 0, i + 1);

                for (int j = 0; j < dias.Count; j++)
                {
                    var dia = dias[j];
                    var registros = registrosPorDia[dia];

                    bool futuro = dia.Date > hoje;

                    bool tomouTodas =
                        suplemento.Horarios != null &&
                        suplemento.Horarios.Count > 0 &&
                        suplemento.Horarios.All(h => registros.Any(r =>
                            r.SuplementoId == suplemento.Id &&
                            r.Horario == h &&
                            r.Tomado));

                    var cor = futuro
                        ? "#F0F2F3"
                        : tomouTodas ? "#DDF5E6" : "#FBE1E1";

                    var texto = futuro
                        ? "–"
                        : tomouTodas ? "✓" : "✕";

                    var corTexto = futuro
                        ? "#AAB5BC"
                        : tomouTodas ? "#20A765" : "#D9534F";

                    tabela.Add(new Border
                    {
                        BackgroundColor = Color.FromArgb(cor),
                        StrokeThickness = 0,
                        Padding = 5,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                        {
                            CornerRadius = 6
                        },
                        Content = new Label
                        {
                            Text = texto,
                            FontSize = 13,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Color.FromArgb(corTexto),
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center
                        }
                    }, j + 1, i + 1);
                }
            }

            HistoricoSemanalStack.Children.Add(tabela);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void OnDataHistoricoChanged(
    object sender,
    DateChangedEventArgs e)
    {
        await CarregarHistorico(e.NewDate ?? DateTime.Today);
    }

    private async void OnDiarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage());
    }

    private async void OnSemanarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(
            new GuiaSemanarioPage(GestanteId, NomeGestante));
    }

    private async void OnPlanoPartoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(
            new GuiaPartoPage(GestanteId, NomeGestante));
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
        await DisplayAlertAsync(
            "Chat",
            "A tela de conversa ainda não foi implementada.",
            "OK");
    }

    private async void OnGuiaMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new GuiaDiarioPage());
    }

    private async void OnPerfilMenuTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(
            new PerfilPage(GestanteId, NomeGestante));
    }
}
