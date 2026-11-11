
using MaternaApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace MaternaApp.Views;

public partial class AgendaPage : ContentPage
{
    private readonly ApiService _api = new();

    public int GestanteId { get; }

    public AgendaPage(int gestanteId)
    {
        InitializeComponent();
        GestanteId = gestanteId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAgenda();
    }

    private async Task CarregarAgenda()
    {
        try
        {
            var hoje = DateTime.Today;

            lblAno.Text = hoje.Year.ToString();
            lblMes.Text = hoje.ToString("MMMM");

            CalendarioGrid.Children.Clear();
            CalendarioGrid.RowDefinitions.Clear();
            ConsultasContainer.Children.Clear();
            lblSemConsultas.IsVisible = false;

            var consultas = await _api.ListarConsultasGestante(GestanteId);

            var consultasFuturas = consultas
                .Where(c => c.Data.Date >= hoje)
                .OrderBy(c => c.Data)
                .ToList();

            CriarCalendario(consultas);

            foreach (var consulta in consultasFuturas)
                ConsultasContainer.Children.Add(CriarCartaoConsulta(consulta));

            lblSemConsultas.IsVisible = consultasFuturas.Count == 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar a agenda.\n{ex.Message}", "OK");
        }
    }

    private void CriarCalendario(List<ConsultaResponse> consultas)
    {
        var hoje = DateTime.Today;
        var primeiroDia = new DateTime(hoje.Year, hoje.Month, 1);
        var diasNoMes = DateTime.DaysInMonth(hoje.Year, hoje.Month);
        var inicio = (int)primeiroDia.DayOfWeek;
        var totalCelulas = (int)Math.Ceiling((inicio + diasNoMes) / 7.0) * 7;

        for (int i = 0; i < totalCelulas / 7; i++)
            CalendarioGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        var diasComConsulta = consultas
            .Where(c => c.Data.Year == hoje.Year && c.Data.Month == hoje.Month)
            .Select(c => c.Data.Day)
            .ToHashSet();

        for (int i = 0; i < totalCelulas; i++)
        {
            var dia = i - inicio + 1;

            if (dia < 1 || dia > diasNoMes)
                continue;

            var temConsulta = diasComConsulta.Contains(dia);

            var label = new Label
            {
                Text = dia.ToString(),
                FontSize = 12,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = temConsulta ? Colors.White : Color.FromArgb("#1A9E85"),
                FontAttributes = temConsulta ? FontAttributes.Bold : FontAttributes.None
            };

            View conteudo = temConsulta
                ? new Border
                {
                    BackgroundColor = Color.FromArgb("#1A9E85"),
                    Stroke = Colors.Transparent,
                    StrokeShape = new RoundRectangle { CornerRadius = 15 },
                    HeightRequest = 28,
                    WidthRequest = 28,
                    HorizontalOptions = LayoutOptions.Center,
                    Content = label
                }
                : label;

            CalendarioGrid.Add(conteudo, i % 7, i / 7);
        }
    }

    private View CriarCartaoConsulta(ConsultaResponse consulta)
    {
        var cor = Color.FromArgb("#1A9E85");

        return new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Colors.Transparent,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Content = new HorizontalStackLayout
            {
                Spacing = 12,
                Children =
                {
                    new Border
                    {
                        Stroke = Colors.Transparent,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        HeightRequest = 45,
                        WidthRequest = 45,
                        BackgroundColor = Color.FromArgb("#E6F4F1"),
                        Content = new VerticalStackLayout
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new Label
                                {
                                    Text = consulta.Data.ToString("dd"),
                                    FontSize = 14,
                                    FontAttributes = FontAttributes.Bold,
                                    TextColor = cor,
                                    HorizontalOptions = LayoutOptions.Center
                                },
                                new Label
                                {
                                    Text = consulta.Data.ToString("MMM").ToUpper(),
                                    FontSize = 9,
                                    TextColor = cor,
                                    HorizontalOptions = LayoutOptions.Center
                                }
                            }
                        }
                    },
                    new VerticalStackLayout
                    {
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label
                            {
                                Text = consulta.Tipo,
                                FontSize = 14,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Color.FromArgb("#2C3E50")
                            },
                            new Label
                            {
                                Text = $"{consulta.Data:HH:mm} · {consulta.Observacoes}",
                                FontSize = 11,
                                TextColor = Color.FromArgb("#7F8C8D")
                            }
                        }
                    }
                }
            }
        };
    }

    private async void OnInicioTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
