namespace Materna.API.Models;

public class Exame
{
    public int Id { get; set; }
    public int GestanteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public string Resultado { get; set; } = string.Empty;
}