
namespace Materna.API.Models;

public class LoginResponse
{
    public bool Sucesso { get; set; }
    public string TipoUsuario { get; set; } = string.Empty;
    public string Funcao { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
}
