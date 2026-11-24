using Materna.API.Data;
using Materna.API.Models;

namespace Materna.API.Services;

public class ExameService
{
    public List<Exame> Listar()
    {
        return DadosSistema.Exames;
    }

    public Exame? BuscarPorId(int id)
    {
        return DadosSistema.Exames
            .FirstOrDefault(e => e.Id == id);
    }

    public Exame Criar(Exame exame)
    {
        exame.Id = DadosSistema.Exames.Count > 0
            ? DadosSistema.Exames.Max(e => e.Id) + 1
            : 1;

        DadosSistema.Exames.Add(exame);

        return exame;
    }

    public bool Atualizar(int id, Exame exame)
    {
        var existente = BuscarPorId(id);

        if (existente == null)
            return false;

        existente.GestanteId = exame.GestanteId;
        existente.Nome = exame.Nome;
        existente.Data = exame.Data;
        existente.Resultado = exame.Resultado;

        return true;
    }

    public bool Excluir(int id)
    {
        var exame = BuscarPorId(id);

        if (exame == null)
            return false;

        DadosSistema.Exames.Remove(exame);

        return true;
    }
}