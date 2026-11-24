
namespace Materna.API.Models;

public class PlanoParto
{
    public int Id { get; set; }
    public int GestanteId { get; set; }
    public Dictionary<string, string> Preferencias { get; set; } = new();
    public DateTime AtualizadoEm { get; set; }
}
