namespace Materna.API.Models;

public class LoginRequest
{
    public string CartaoSUS { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string COREN { get; set; } = string.Empty;
    public string EMAIL { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}