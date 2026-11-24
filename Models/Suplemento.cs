
namespace Materna.API.Models;

public class Suplemento
{
    public int Id { get; set; }
    public int GestanteId { get; set; }
    public int ProfissionalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Dosagem { get; set; } = string.Empty;
    public List<string> Horarios { get; set; } = new();
    public string Orientacoes { get; set; } = string.Empty;
    public DateTime CadastradoEm { get; set; }
    public bool Ativo { get; set; } = true;
}
