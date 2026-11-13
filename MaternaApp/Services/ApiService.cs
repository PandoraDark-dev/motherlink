
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

    private async Task<T?> Enviar<T>(HttpResponseMessage resposta)
    {
        if (!resposta.IsSuccessStatusCode)
            throw new HttpRequestException(await resposta.Content.ReadAsStringAsync());

        return await resposta.Content.ReadFromJsonAsync<T>(_jsonOptions);
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
        return await Enviar<GestanteResponse>(resposta);
    }

    public async Task<List<GestanteResponse>> ListarGestantes()
    {
        var resposta = await _httpClient.GetAsync("api/Gestantes");
        return await Enviar<List<GestanteResponse>>(resposta) ?? new();
    }

    public async Task<GestanteResponse?> BuscarGestantePorId(int id)
    {
        var resposta = await _httpClient.GetAsync($"api/Gestantes/{id}");

        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        return await Enviar<GestanteResponse>(resposta);
    }

    public async Task<List<ConsultaResponse>> ListarConsultasGestante(int gestanteId)
    {
        var resposta = await _httpClient.GetAsync($"api/Consultas/gestante/{gestanteId}");
        return await Enviar<List<ConsultaResponse>>(resposta) ?? new();
    }

    public async Task<List<BuscaAtivaResponse>> ListarBuscaAtiva()
    {
        var resposta = await _httpClient.GetAsync("api/Consultas/busca-ativa");
        return await Enviar<List<BuscaAtivaResponse>>(resposta) ?? new();
    }

    public async Task<ConsultaResponse?> CadastrarConsulta(ConsultaRequest dados)
    {
        var resposta = await _httpClient.PostAsJsonAsync("api/Consultas", dados);
        return await Enviar<ConsultaResponse>(resposta);
    }

    public async Task<List<ProfissionalResponse>> ListarProfissionais()
    {
        var resposta = await _httpClient.GetAsync("api/Profissionais");
        return await Enviar<List<ProfissionalResponse>>(resposta) ?? new();
    }

    public async Task<ProfissionalResponse?> CadastrarProfissional(ProfissionalRequest dados)
    {
        var resposta = await _httpClient.PostAsJsonAsync("api/Profissionais", dados);
        return await Enviar<ProfissionalResponse>(resposta);
    }

    public async Task<PlanoPartoResponse?> BuscarPlanoParto(int gestanteId)
    {
        var resposta = await _httpClient.GetAsync($"api/PlanosParto/gestante/{gestanteId}");

        if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        return await Enviar<PlanoPartoResponse>(resposta);
    }

    public async Task<PlanoPartoResponse?> SalvarPlanoParto(int gestanteId, Dictionary<string, string> preferencias)
    {
        var dados = new PlanoPartoResponse
        {
            GestanteId = gestanteId,
            Preferencias = preferencias
        };

        var resposta = await _httpClient.PutAsJsonAsync(
            $"api/PlanosParto/gestante/{gestanteId}", dados);

        return await Enviar<PlanoPartoResponse>(resposta);
    }

    public async Task<List<SuplementoResponse>> ListarSuplementos(int gestanteId)
    {
        var resposta = await _httpClient.GetAsync($"api/Suplementos/gestante/{gestanteId}");
        return await Enviar<List<SuplementoResponse>>(resposta) ?? new();
    }

    public async Task<SuplementoResponse?> CadastrarSuplemento(SuplementoRequest dados)
    {
        var resposta = await _httpClient.PostAsJsonAsync("api/Suplementos", dados);
        return await Enviar<SuplementoResponse>(resposta);
    }

    public async Task<SuplementoResponse?> AtualizarSuplemento(int id, SuplementoRequest dados)
    {
        var resposta = await _httpClient.PutAsJsonAsync($"api/Suplementos/{id}", dados);
        return await Enviar<SuplementoResponse>(resposta);
    }

    public async Task ExcluirSuplemento(int id)
    {
        var resposta = await _httpClient.DeleteAsync($"api/Suplementos/{id}");

        if (!resposta.IsSuccessStatusCode)
            throw new HttpRequestException(await resposta.Content.ReadAsStringAsync());
    }

    public async Task<List<RegistroSuplementoResponse>> ListarHistoricoSuplementos(int gestanteId, DateTime? data = null)
    {
        var url = $"api/Suplementos/historico/{gestanteId}";

        if (data.HasValue)
            url += $"?data={data.Value:yyyy-MM-dd}";

        var resposta = await _httpClient.GetAsync(url);
        return await Enviar<List<RegistroSuplementoResponse>>(resposta) ?? new();
    }

    public async Task<RegistroSuplementoResponse?> RegistrarDose(int suplementoId, DoseSuplementoRequest dados)
    {
        var resposta = await _httpClient.PostAsJsonAsync(
            $"api/Suplementos/{suplementoId}/doses", dados);

        return await Enviar<RegistroSuplementoResponse>(resposta);
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
    public string Funcao { get; set; } = string.Empty;
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

public class BuscaAtivaResponse
{
    public int GestanteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string CartaoSUS { get; set; } = string.Empty;
    public DateTime? UltimaConsulta { get; set; }
    public int? DiasSemConsulta { get; set; }
    public string Situacao { get; set; } = string.Empty;
}

public class ConsultaRequest
{
    public int GestanteId { get; set; }
    public int ProfissionalId { get; set; }
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
}

public class ProfissionalRequest
{
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Registro { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class ProfissionalResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Registro { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Funcao { get; set; } = string.Empty;
}

public class PlanoPartoResponse
{
    public int Id { get; set; }
    public int GestanteId { get; set; }
    public Dictionary<string, string> Preferencias { get; set; } = new();
    public DateTime AtualizadoEm { get; set; }
}

public class SuplementoRequest
{
    public int GestanteId { get; set; }
    public int ProfissionalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Dosagem { get; set; } = string.Empty;
    public List<string> Horarios { get; set; } = new();
    public string Orientacoes { get; set; } = string.Empty;
}

public class SuplementoResponse
{
    public int Id { get; set; }
    public int GestanteId { get; set; }
    public int ProfissionalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Dosagem { get; set; } = string.Empty;
    public List<string> Horarios { get; set; } = new();
    public string Orientacoes { get; set; } = string.Empty;
    public DateTime CadastradoEm { get; set; }
    public bool Ativo { get; set; }
}

public class DoseSuplementoRequest
{
    public DateTime Data { get; set; }
    public string Horario { get; set; } = string.Empty;
    public bool Tomado { get; set; }
}

public class RegistroSuplementoResponse
{
    public int Id { get; set; }
    public int SuplementoId { get; set; }
    public int GestanteId { get; set; }
    public DateTime Data { get; set; }
    public string Horario { get; set; } = string.Empty;
    public bool Tomado { get; set; }
    public DateTime? RegistradoEm { get; set; }
}
