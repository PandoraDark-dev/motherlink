
using Materna.API.Data;
using Materna.API.Models;

namespace Materna.API.Services;

public class ProfissionalService
{
    public List<Profissional> Listar()
    {
        return DadosSistema.Profissionais;
    }

    public Profissional? BuscarPorId(int id)
    {
        return DadosSistema.Profissionais.FirstOrDefault(p => p.Id == id);
    }

    public Profissional Criar(Profissional profissional)
    {
        profissional.Id = DadosSistema.Profissionais.Count > 0
            ? DadosSistema.Profissionais.Max(p => p.Id) + 1
            : 1;

        profissional.Funcao = "Profissional";

        DadosSistema.Profissionais.Add(profissional);

        return profissional;
    }

    public bool Atualizar(int id, Profissional profissional)
    {
        var existente = BuscarPorId(id);

        if (existente == null)
            return false;

        existente.Nome = profissional.Nome;
        existente.CPF = profissional.CPF;
        existente.Registro = profissional.Registro;
        existente.Especialidade = profissional.Especialidade;
        existente.Email = profissional.Email;
        existente.Senha = profissional.Senha;

        return true;
    }

    public bool Excluir(int id)
    {
        var profissional = BuscarPorId(id);

        if (profissional == null)
            return false;

        DadosSistema.Profissionais.Remove(profissional);

        return true;
    }
}
