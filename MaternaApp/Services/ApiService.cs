
using System.Net.Http.Json;
using System.Text.Json;

namespace MaternaApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5240/"),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task<LoginResponse?> LoginGestante(string cpf, string cartaoSUS, string senha)
    {
        return await Login(new LoginRequest
        {
            CPF = cpf,
            CartaoSUS = cartaoSUS,
            Senha = senha
        });
    }

    public async Task<LoginResponse?> LoginProfissional(string coren, string email, string senha)
    {
        return await Login(new LoginRequest
        {
            COREN = coren,
            EMAIL = email,
            Senha = senha
        });
    }

    private async Task<LoginResponse?> Login(LoginRequest dados)
    {
        var resposta = await _httpClient.PostAsJsonAsync("api/Auth/login", dados);
        var conteudo = await resposta.Content.ReadAsStringAsync();

        if (!resposta.IsSuccessStatusCode)
        {
            return new LoginResponse
            {
                Sucesso = false,
                Mensagem = $"Erro {(int)resposta.StatusCode}: {conteudo}"
            };
        }

        return JsonSerializer.Deserialize<LoginResponse>(conteudo, _jsonOptions);
    }

    public async Task<GestanteResponse?> CadastrarGestante(GestanteRequest dados)
    {
        var resposta = await _httpClient.PostAsJsonAsync("api/Gestantes", dados);

        if (!resposta.IsSuccessStatusCode)
        {
            var erro = await resposta.Content.ReadAsStringAsync();
            throw new HttpRequestException(erro);
        }

        return await resposta.Content.ReadFromJsonAsync<GestanteResponse>(_jsonOptions);
    }

    public async Task<List<GestanteResponse>> ListarGestantes()
    {
        var resposta = await _httpClient.GetAsync("api/Gestantes");

        if (!resposta.IsSuccessStatusCode)
        {
            var erro = await resposta.Content.ReadAsStringAsync();
            throw new HttpRequestException(erro);
        }

        return await resposta.Content.ReadFromJsonAsync<List<GestanteResponse>>(_jsonOptions)
            ?? new List<GestanteResponse>();
    }

    public async Task<GestanteResponse?> BuscarGestantePorId(int id)
    {
        var resposta = await _httpClient.GetAsync($"api/Gestantes/{id}");

        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        resposta.EnsureSuccessStatusCode();

        return await resposta.Content.ReadFromJsonAsync<GestanteResponse>(_jsonOptions);
    }

    public async Task<List<ConsultaResponse>> ListarConsultasGestante(int gestanteId)
    {
        var resposta = await _httpClient.GetAsync($"api/Consultas/gestante/{gestanteId}");

        if (!resposta.IsSuccessStatusCode)
        {
            var erro = await resposta.Content.ReadAsStringAsync();
            throw new HttpRequestException(erro);
        }

        return await resposta.Content.ReadFromJsonAsync<List<ConsultaResponse>>(_jsonOptions)
            ?? new List<ConsultaResponse>();
    }

    public async Task<ConsultaResponse?> CadastrarConsulta(ConsultaRequest dados)
    {
        var resposta = await _httpClient.PostAsJsonAsync("api/Consultas", dados);

        if (!resposta.IsSuccessStatusCode)
        {
            var erro = await resposta.Content.ReadAsStringAsync();
            throw new HttpRequestException(erro);
        }

        return await resposta.Content.ReadFromJsonAsync<ConsultaResponse>(_jsonOptions);
    }
}

public class LoginRequest
{
    public string CartaoSUS { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string COREN { get; set; } = string.Empty;
    public string EMAIL { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Sucesso { get; set; }
    public string TipoUsuario { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
}

public class GestanteRequest
{
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string CartaoSUS { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public DateTime DUM { get; set; }
    public DateTime DPP { get; set; }
}

public class GestanteResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string CartaoSUS { get; set; } = string.Empty;
    public DateTime DUM { get; set; }
    public DateTime DPP { get; set; }
}

public class ConsultaResponse
{
    public int Id { get; set; }
    public int GestanteId { get; set; }
    public int ProfissionalId { get; set; }
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
}

public class ConsultaRequest
{
    public int GestanteId { get; set; }
    public int ProfissionalId { get; set; }
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
}
