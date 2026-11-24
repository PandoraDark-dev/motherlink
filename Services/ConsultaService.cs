
using Materna.API.Data;
using Materna.API.Models;

namespace Materna.API.Services;

public class ConsultaService
{
    public List<Consulta> Listar()
    {
        return DadosSistema.Consultas;
    }

    public List<Consulta> ListarPorGestante(int gestanteId)
    {
        return DadosSistema.Consultas
            .Where(c => c.GestanteId == gestanteId)
            .OrderBy(c => c.Data)
            .ToList();
    }

    public List<BuscaAtivaResponse> ListarBuscaAtiva()
    {
        var hoje = DateTime.Today;

        return DadosSistema.Gestantes
            .Select(g =>
            {
                var consultas = DadosSistema.Consultas
                    .Where(c => c.GestanteId == g.Id)
                    .ToList();

                var futura = consultas.Any(c => c.Data.Date > hoje);

                var ultima = consultas
                    .Where(c => c.Data.Date <= hoje)
                    .OrderByDescending(c => c.Data)
                    .FirstOrDefault();

                var dias = ultima == null
                    ? (int?)null
                    : (hoje - ultima.Data.Date).Days;

                return new
                {
                    Gestante = g,
                    Futura = futura,
                    Ultima = ultima,
                    Dias = dias
                };
            })
            .Where(x => !x.Futura && (x.Dias == null || x.Dias >= 30))
            .Select(x => new BuscaAtivaResponse
            {
                GestanteId = x.Gestante.Id,
                Nome = x.Gestante.Nome,
                CPF = x.Gestante.CPF,
                CartaoSUS = x.Gestante.CartaoSUS,
                UltimaConsulta = x.Ultima?.Data,
                DiasSemConsulta = x.Dias,
                Situacao = x.Ultima == null
                    ? "Sem consulta registrada"
                    : "Acompanhamento pendente"
            })
            .ToList();
    }

    public Consulta? BuscarPorId(int id)
    {
        return DadosSistema.Consultas.FirstOrDefault(c => c.Id == id);
    }

    public Consulta Criar(Consulta consulta)
    {
        consulta.Id = DadosSistema.Consultas.Count > 0
            ? DadosSistema.Consultas.Max(c => c.Id) + 1
            : 1;

        DadosSistema.Consultas.Add(consulta);

        return consulta;
    }

    public bool Atualizar(int id, Consulta consulta)
    {
        var existente = BuscarPorId(id);

        if (existente == null)
            return false;

        existente.GestanteId = consulta.GestanteId;
        existente.ProfissionalId = consulta.ProfissionalId;
        existente.Data = consulta.Data;
        existente.Tipo = consulta.Tipo;
        existente.Observacoes = consulta.Observacoes;

        return true;
    }

    public bool Excluir(int id)
    {
        var consulta = BuscarPorId(id);

        if (consulta == null)
            return false;

        DadosSistema.Consultas.Remove(consulta);

        return true;
    }
}
