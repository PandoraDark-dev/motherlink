namespace Materna.API.Models;

public class Gestante
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string CPF { get; set; } = string.Empty;

    public string CartaoSUS { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;

    public DateTime DUM { get; set; }

    public DateTime DPP { get; set; }
}