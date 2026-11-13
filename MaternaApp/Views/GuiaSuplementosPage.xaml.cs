
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class GuiaSuplementosPage : ContentPage
{
    public int GestanteId { get; }
    public string NomeGestante { get; }

    private readonly ApiService _api = new();
    private List<SuplementoResponse> _suplementos = [];
    private List<RegistroSuplementoResponse> _registros = [];

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

        await CarregarSuplementos();
        await CarregarHistorico(DateTime.Today);
    }

    private async Task CarregarSuplementos()
    {
        try
        {
            _suplementos = (await _api.ListarSuplementos(GestanteId)).ToList();
            _registros = (await _api.ListarHistoricoSuplementos(GestanteId, DateTime.Today)).ToList();

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
                CriarCartaoSuplemento(suplemento);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private void CriarCartaoSuplemento(SuplementoResponse suplemento)
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
            var registro = _registros.FirstOrDefault(r =>
                r.SuplementoId == suplemento.Id &&
                r.Horario == horario);

            bool tomado = registro?.Tomado == true;

            var botao = new Button
            {
                Text = tomado ? $"✓ Tomado às {horario}" : $"Marcar dose das {horario} como tomada",
                BackgroundColor = Color.FromArgb(tomado ? "#20A765" : "#D9534F"),
                TextColor = Colors.White,
                CornerRadius = 12,
                HeightRequest = 46,
                FontSize = 13
            };

            botao.Clicked += async (_, _) =>
                await RegistrarDose(suplemento, horario, !tomado);

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

    private async Task RegistrarDose(SuplementoResponse suplemento, string horario, bool tomado)
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

            await CarregarSuplementos();
            await CarregarHistorico(DataHistoricoPicker.Date ?? DateTime.Today);
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
            var registros = (await _api.ListarHistoricoSuplementos(GestanteId, data)).ToList();

            HistoricoDosesStack.Children.Clear();

            if (_suplementos.Count == 0)
            {
                HistoricoDosesStack.Children.Add(new Label
                {
                    Text = "Não há suplementos cadastrados.",
                    TextColor = Color.FromArgb("#81938F")
                });
                return;
            }

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
                        TextColor = Color.FromArgb(tomado ? "#20A765" : "#D9534F"),
                        VerticalOptions = LayoutOptions.Center
                    }, 1);

                    HistoricoDosesStack.Children.Add(linha);

                    if (registro != null)
                    {
                        HistoricoDosesStack.Children.Add(new Label
                        {
                            Text = $"Registro: {registro.RegistradoEm:HH:mm}",
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

    private async void OnDataHistoricoChanged(object sender, DateChangedEventArgs e)
    {
        await CarregarHistorico(DataHistoricoPicker.Date ?? DateTime.Today);
    }

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
