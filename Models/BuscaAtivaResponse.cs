
namespace Materna.API.Models;

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
