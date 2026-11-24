
using Materna.API.Data;
using Materna.API.Models;

namespace Materna.API.Services;

public class SuplementoService
{
    public List<Suplemento> ListarPorGestante(int gestanteId)
    {
        return DadosSistema.Suplementos
            .Where(s => s.GestanteId == gestanteId && s.Ativo)
            .ToList();
    }

    public Suplemento? BuscarPorId(int id)
    {
        return DadosSistema.Suplementos
            .FirstOrDefault(s => s.Id == id && s.Ativo);
    }

    public Suplemento? Cadastrar(Suplemento suplemento)
    {
        if (!DadosSistema.Gestantes.Any(g => g.Id == suplemento.GestanteId) ||
            !DadosSistema.Profissionais.Any(p => p.Id == suplemento.ProfissionalId))
            return null;

        suplemento.Id = DadosSistema.Suplementos.Count > 0
            ? DadosSistema.Suplementos.Max(s => s.Id) + 1
            : 1;

        suplemento.CadastradoEm = DateTime.Now;
        suplemento.Ativo = true;

        DadosSistema.Suplementos.Add(suplemento);

        return suplemento;
    }

    public Suplemento? Atualizar(int id, Suplemento dados)
    {
        var suplemento = BuscarPorId(id);

        if (suplemento == null)
            return null;

        suplemento.Nome = dados.Nome;
        suplemento.Dosagem = dados.Dosagem;
        suplemento.Horarios = dados.Horarios;
        suplemento.Orientacoes = dados.Orientacoes;

        return suplemento;
    }

    public bool Excluir(int id)
    {
        var suplemento = BuscarPorId(id);

        if (suplemento == null)
            return false;

        suplemento.Ativo = false;

        return true;
    }

    public List<RegistroSuplemento> ListarHistorico(int gestanteId, DateTime data)
    {
        return DadosSistema.RegistrosSuplementos
            .Where(r => r.GestanteId == gestanteId && r.Data.Date == data.Date)
            .OrderBy(r => r.Horario)
            .ToList();
    }

    public RegistroSuplemento? RegistrarDose(int suplementoId, DateTime data, string horario, bool tomado)
    {
        var suplemento = BuscarPorId(suplementoId);

        if (suplemento == null)
            return null;

        var registro = DadosSistema.RegistrosSuplementos.FirstOrDefault(r =>
            r.SuplementoId == suplementoId &&
            r.Data.Date == data.Date &&
            r.Horario == horario);

        if (registro == null)
        {
            registro = new RegistroSuplemento
            {
                Id = DadosSistema.RegistrosSuplementos.Count > 0
                    ? DadosSistema.RegistrosSuplementos.Max(r => r.Id) + 1
                    : 1,
                SuplementoId = suplementoId,
                GestanteId = suplemento.GestanteId,
                Data = data.Date,
                Horario = horario
            };

            DadosSistema.RegistrosSuplementos.Add(registro);
        }

        registro.Tomado = tomado;
        registro.RegistradoEm = DateTime.Now;

        return registro;
    }
}
