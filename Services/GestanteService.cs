using Materna.API.Data;
using Materna.API.Models;

namespace Materna.API.Services;

public class GestanteService
{
    public List<Gestante> Listar()
    {
        return DadosSistema.Gestantes;
    }

    public Gestante? BuscarPorId(int id)
    {
        return DadosSistema.Gestantes
            .FirstOrDefault(g => g.Id == id);
    }

    public Gestante Criar(Gestante gestante)
    {
        gestante.Id = DadosSistema.Gestantes.Count > 0
            ? DadosSistema.Gestantes.Max(g => g.Id) + 1
            : 1;

        DadosSistema.Gestantes.Add(gestante);

        return gestante;
    }

    public bool Atualizar(int id, Gestante gestante)
    {
        var existente = BuscarPorId(id);

        if (existente == null)
            return false;

        existente.Nome = gestante.Nome;
        existente.CPF = gestante.CPF;
        existente.CartaoSUS = gestante.CartaoSUS;
        existente.Senha = gestante.Senha;
        existente.DUM = gestante.DUM;
        existente.DPP = gestante.DPP;

        return true;
    }

    public bool Excluir(int id)
    {
        var gestante = BuscarPorId(id);

        if (gestante == null)
            return false;

        DadosSistema.Gestantes.Remove(gestante);

        return true;
    }
}