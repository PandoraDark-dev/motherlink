
using System.Net.Http.Json;

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

    public async Task<LoginResponse?> Login(string cpf, string cartaoSUS, string senha)
    {
        var dados = new LoginRequest
        {
            CPF = cpf,
            CartaoSUS = cartaoSUS,
            Senha = senha
        };

        var resposta = await _httpClient.PostAsJsonAsync("api/Auth/login", dados);

        if (!resposta.IsSuccessStatusCode)
        {
            return new LoginResponse
            {
                Sucesso = false,
                Mensagem = resposta.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ? "CPF, Cartão SUS ou senha inválidos."
                    : $"Erro na API: {(int)resposta.StatusCode} - {resposta.ReasonPhrase}"
            };
        }

        return await resposta.Content.ReadFromJsonAsync<LoginResponse>();
    }
}

public class LoginRequest
{
    public string CPF { get; set; } = string.Empty;
    public string CartaoSUS { get; set; } = string.Empty;
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