
using CommunityToolkit.Maui.Storage;
using MaternaApp.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MaternaApp.Views;

public partial class EsusPage : ContentPage
{
    private readonly ApiService _api = new();
    private List<GestanteResponse> _gestantes = new();
    private List<ConsultaResponse> _consultas = new();
    private GestanteResponse? _gestanteSelecionada;
    private PlanoPartoResponse? _planoParto;

    private readonly Dictionary<string, string> _nomesPreferencias = new()
    {
        ["Acompanhante"] = "Acompanhante",
        ["TipoParto"] = "Tipo de parto preferido",
        ["AlivioDor"] = "Alívio da dor",
        ["Cordao"] = "Corte do cordão umbilical",
        ["PelePele"] = "Contato pele a pele",
        ["Amamentacao"] = "Amamentação"
    };

    public EsusPage()
    {
        InitializeComponent();
        CarregarGestantes();
    }

    private async void CarregarGestantes()
    {
        try
        {
            _gestantes = await _api.ListarGestantes();
            pkGestante.ItemsSource = _gestantes;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar as gestantes.\n{ex.Message}", "OK");
        }
    }

    private async void OnGestanteSelecionada(object sender, EventArgs e)
    {
        if (pkGestante.SelectedItem is not GestanteResponse gestante)
            return;

        _gestanteSelecionada = gestante;
        _consultas.Clear();
        _planoParto = null;

        btnExportar.IsEnabled = false;
        ListaConsultas.ItemsSource = null;

        lblNome.Text = gestante.Nome;
        lblCpf.Text = gestante.CPF;
        lblSus.Text = string.IsNullOrWhiteSpace(gestante.CartaoSUS)
            ? "Não informado"
            : gestante.CartaoSUS;
        lblDum.Text = gestante.DUM.ToString("dd/MM/yyyy");
        lblDpp.Text = gestante.DPP.ToString("dd/MM/yyyy");

        AtualizarPlanoNaTela(null);

        try
        {
            _consultas = await _api.ListarConsultasGestante(gestante.Id);

            _consultas = _consultas
                .OrderByDescending(c => c.Data)
                .ToList();

            ListaConsultas.ItemsSource = _consultas;
            btnExportar.IsEnabled = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar o histórico.\n{ex.Message}", "OK");
        }

        try
        {
            _planoParto = await _api.BuscarPlanoParto(gestante.Id);
            AtualizarPlanoNaTela(_planoParto);
        }
        catch (Exception ex)
        {
            AtualizarPlanoNaTela(null);

            await DisplayAlert(
                "Aviso",
                $"Não foi possível consultar o Plano de Parto.\n{ex.Message}",
                "OK");
        }
    }

    private void AtualizarPlanoNaTela(PlanoPartoResponse? plano)
    {
        var preferencias = plano?.Preferencias;

        lblAcompanhante.Text = preferencias?.GetValueOrDefault("Acompanhante", "Não informado") ?? "Não informado";
        lblTipoParto.Text = preferencias?.GetValueOrDefault("TipoParto", "Não informado") ?? "Não informado";
        lblAlivioDor.Text = preferencias?.GetValueOrDefault("AlivioDor", "Não informado") ?? "Não informado";
        lblCordao.Text = preferencias?.GetValueOrDefault("Cordao", "Não informado") ?? "Não informado";
        lblPelePele.Text = preferencias?.GetValueOrDefault("PelePele", "Não informado") ?? "Não informado";
        lblAmamentacao.Text = preferencias?.GetValueOrDefault("Amamentacao", "Não informado") ?? "Não informado";

        lblAtualizacaoPlano.Text = plano == null
            ? "Nenhum plano registrado"
            : $"Última atualização: {plano.AtualizadoEm:dd/MM/yyyy HH:mm}";
    }

    private async void OnExportarPdfClicked(object sender, EventArgs e)
    {
        if (_gestanteSelecionada is not GestanteResponse gestante)
            return;

        try
        {
            btnExportar.IsEnabled = false;

            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("MotherLink")
                                .FontSize(24)
                                .Bold()
                                .FontColor("#17685F");

                            column.Item()
                                .Text("Prontuário pré-natal")
                                .FontSize(14)
                                .FontColor("#526B66");

                            column.Item()
                                .PaddingVertical(10)
                                .LineHorizontal(1)
                                .LineColor("#DCE9E5");
                        });

                    page.Content()
                        .PaddingVertical(15)
                        .Column(column =>
                        {
                            column.Spacing(12);

                            column.Item()
                                .Text("Identificação da gestante")
                                .FontSize(16)
                                .Bold()
                                .FontColor("#17685F");

                            column.Item().Text($"Nome: {gestante.Nome}");
                            column.Item().Text($"CPF: {gestante.CPF}");
                            column.Item().Text($"Cartão SUS: {gestante.CartaoSUS}");
                            column.Item().Text($"DUM: {gestante.DUM:dd/MM/yyyy}");
                            column.Item().Text($"DPP: {gestante.DPP:dd/MM/yyyy}");

                            column.Item()
                                .PaddingTop(15)
                                .Text("Histórico de consultas")
                                .FontSize(16)
                                .Bold()
                                .FontColor("#17685F");

                            if (_consultas.Count == 0)
                            {
                                column.Item().Text("Nenhuma consulta registrada.");
                            }
                            else
                            {
                                foreach (var consulta in _consultas)
                                {
                                    column.Item()
                                        .BorderBottom(1)
                                        .BorderColor("#DCE9E5")
                                        .PaddingVertical(8)
                                        .Column(item =>
                                        {
                                            item.Item().Text(consulta.Tipo).Bold();

                                            item.Item().Text(
                                                consulta.Data.ToString("dd/MM/yyyy HH:mm"));

                                            item.Item().Text(
                                                string.IsNullOrWhiteSpace(consulta.Observacoes)
                                                    ? "Sem observações"
                                                    : consulta.Observacoes);
                                        });
                                }
                            }

                            column.Item()
                                .PaddingTop(15)
                                .Text("Plano de Parto")
                                .FontSize(16)
                                .Bold()
                                .FontColor("#17685F");

                            if (_planoParto == null || _planoParto.Preferencias.Count == 0)
                            {
                                column.Item().Text("Nenhum Plano de Parto registrado.");
                            }
                            else
                            {
                                foreach (var item in _nomesPreferencias)
                                {
                                    column.Item().Text(
                                        $"{item.Value}: {_planoParto.Preferencias.GetValueOrDefault(item.Key, "Não informado")}");
                                }

                                column.Item()
                                    .Text($"Última atualização: {_planoParto.AtualizadoEm:dd/MM/yyyy HH:mm}")
                                    .FontSize(9)
                                    .FontColor("#526B66");
                            }

                            column.Item()
                                .PaddingTop(20)
                                .Text($"Data de emissão: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Documento gerado pelo sistema MotherLink")
                        .FontSize(9)
                        .FontColor("#81938F");
                });
            }).GeneratePdf();

            var nomeArquivo = $"Relatorio_PreNatal_{SanitizarNome(gestante.Nome)}_{DateTime.Now:yyyyMMdd}.pdf";

            using var stream = new MemoryStream(pdf);
            var resultado = await FileSaver.Default.SaveAsync(nomeArquivo, stream);

            if (resultado.IsSuccessful)
                await DisplayAlert("Sucesso", "Relatório PDF salvo no dispositivo.", "OK");
            else if (resultado.Exception != null)
                await DisplayAlert("Erro", resultado.Exception.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível gerar o PDF.\n{ex.Message}", "OK");
        }
        finally
        {
            btnExportar.IsEnabled = _gestanteSelecionada != null;
        }
    }

    private static string SanitizarNome(string nome)
    {
        foreach (var caractere in Path.GetInvalidFileNameChars())
            nome = nome.Replace(caractere, '_');

        return nome.Replace(" ", "_");
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
