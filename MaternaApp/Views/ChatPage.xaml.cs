
using MaternaApp.Services;

namespace MaternaApp.Views;

public partial class ChatPage : ContentPage
{
    private readonly ApiService _api = new();
    private readonly int _gestanteId;
    private readonly int _profissionalId;
    private readonly int _usuarioId;
    private readonly string _tipoUsuario;

    private bool _carregando;
    private bool _estaVisivel;
    private bool _atualizacaoIniciada;

    public ChatPage(
        int gestanteId,
        int profissionalId,
        int usuarioId,
        string tipoUsuario,
        string nomeContato)
    {
        InitializeComponent();

        _gestanteId = gestanteId;
        _profissionalId = profissionalId;
        _usuarioId = usuarioId;
        _tipoUsuario = tipoUsuario.ToLowerInvariant();

        lblContato.Text = nomeContato;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _estaVisivel = true;

        await CarregarMensagens();

        if (_atualizacaoIniciada)
            return;

        _atualizacaoIniciada = true;

        Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            if (!_estaVisivel)
            {
                _atualizacaoIniciada = false;
                return false;
            }

            _ = CarregarMensagens(false);
            return true;
        });
    }

    protected override void OnDisappearing()
    {
        _estaVisivel = false;
        base.OnDisappearing();
    }

    private async Task CarregarMensagens(bool mostrarErro = true)
    {
        if (_carregando)
            return;

        _carregando = true;

        try
        {
            var mensagens = _tipoUsuario == "gestante"
                ? await _api.ListarMensagensGestante(_gestanteId)
                : await _api.ListarMensagensProfissional(_profissionalId);

            mensagens = mensagens
                .Where(m => m.GestanteId == _gestanteId)
                .OrderBy(m => m.EnviadaEm)
                .ToList();

            ListaMensagens.ItemsSource = mensagens.Select(m =>
            {
                var minhaMensagem = m.RemetenteId == _usuarioId &&
                                    m.RemetenteTipo == _tipoUsuario;

                return new MensagemVisual
                {
                    Conteudo = m.Conteudo,
                    Horario = m.EnviadaEm.ToString("HH:mm"),
                    Alinhamento = minhaMensagem
                        ? LayoutOptions.End
                        : LayoutOptions.Start,
                    Cor = minhaMensagem
                        ? Color.FromArgb("#347F80")
                        : Colors.White,
                    CorTexto = minhaMensagem
                        ? Colors.White
                        : Color.FromArgb("#294B49"),
                    CorHorario = minhaMensagem
                        ? Color.FromArgb("#E1F2EF")
                        : Color.FromArgb("#78908D")
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            if (mostrarErro)
                await DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            _carregando = false;
        }
    }

    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        var conteudo = txtMensagem.Text?.Trim();

        if (string.IsNullOrWhiteSpace(conteudo))
            return;

        try
        {
            await _api.EnviarMensagem(new MensagemRequest
            {
                GestanteId = _gestanteId,
                ProfissionalId = _profissionalId,
                RemetenteId = _usuarioId,
                RemetenteTipo = _tipoUsuario,
                Conteudo = conteudo
            });

            txtMensagem.Text = string.Empty;

            await CarregarMensagens();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao enviar", ex.Message, "OK");
        }
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    public class MensagemVisual
    {
        public string Conteudo { get; set; } = string.Empty;
        public string Horario { get; set; } = string.Empty;
        public LayoutOptions Alinhamento { get; set; }
        public Color Cor { get; set; } = Colors.White;
        public Color CorTexto { get; set; } = Colors.Black;
        public Color CorHorario { get; set; } = Colors.Gray;
    }
}
