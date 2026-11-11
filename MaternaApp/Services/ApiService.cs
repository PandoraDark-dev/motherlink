
using System.Net.Http.Json;
using System.Text.Json;

namespace MaternaApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

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

        return JsonSerializer.Deserialize<LoginResponse>(
            conteudo,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
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
