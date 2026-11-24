namespace Materna.API.Models;

public class Consulta
{
    public int Id { get; set; }
    public int GestanteId { get; set; }
    public int ProfissionalId { get; set; }
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
}