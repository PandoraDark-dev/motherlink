
using Materna.API.Data;
using Materna.API.Models;

namespace Materna.API.Services;

public class PlanoPartoService
{
    public PlanoParto? BuscarPorGestante(int gestanteId)
    {
        return DadosSistema.PlanosParto
            .FirstOrDefault(p => p.GestanteId == gestanteId);
    }

    public PlanoParto? Salvar(int gestanteId, Dictionary<string, string> preferencias)
    {
        if (!DadosSistema.Gestantes.Any(g => g.Id == gestanteId))
            return null;

        var plano = BuscarPorGestante(gestanteId);

        if (plano == null)
        {
            plano = new PlanoParto
            {
                Id = DadosSistema.PlanosParto.Count > 0
                    ? DadosSistema.PlanosParto.Max(p => p.Id) + 1
                    : 1,
                GestanteId = gestanteId
            };

            DadosSistema.PlanosParto.Add(plano);
        }

        plano.Preferencias = new Dictionary<string, string>(preferencias);
        plano.AtualizadoEm = DateTime.Now;

        return plano;
    }
}
