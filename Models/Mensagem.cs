
namespace Materna.API.Models;

public class Mensagem
{
    public int Id { get; set; }
    public int GestanteId { get; set; }
    public int ProfissionalId { get; set; }
    public int RemetenteId { get; set; }
    public string RemetenteTipo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime EnviadaEm { get; set; } = DateTime.Now;
    public bool Lida { get; set; }
}
