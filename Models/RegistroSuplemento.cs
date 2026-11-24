
namespace Materna.API.Models;

public class RegistroSuplemento
{
    public int Id { get; set; }
    public int SuplementoId { get; set; }
    public int GestanteId { get; set; }
    public DateTime Data { get; set; }
    public string Horario { get; set; } = string.Empty;
    public bool Tomado { get; set; }
    public DateTime? RegistradoEm { get; set; }
}
