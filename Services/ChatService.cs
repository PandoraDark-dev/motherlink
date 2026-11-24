
using Materna.API.Data;
using Materna.API.Models;

namespace Materna.API.Services;

public class ChatService
{
    public List<Mensagem> ListarPorGestante(int gestanteId)
    {
        return DadosSistema.Mensagens
            .Where(m => m.GestanteId == gestanteId)
            .OrderBy(m => m.EnviadaEm)
            .ToList();
    }

    public List<Mensagem> ListarPorProfissional(int profissionalId)
    {
        return DadosSistema.Mensagens
            .Where(m => m.ProfissionalId == profissionalId)
            .OrderBy(m => m.EnviadaEm)
            .ToList();
    }

    public Mensagem Enviar(MensagemRequest request)
    {
        var tipo = request.RemetenteTipo?.Trim().ToLowerInvariant();
        var conteudo = request.Conteudo?.Trim();

        if (request.GestanteId <= 0 ||
            request.ProfissionalId <= 0 ||
            request.RemetenteId <= 0 ||
            string.IsNullOrWhiteSpace(conteudo) ||
            (tipo != "gestante" && tipo != "profissional"))
            throw new ArgumentException("Dados da mensagem inválidos.");

        if (!DadosSistema.Gestantes.Any(g => g.Id == request.GestanteId) ||
            !DadosSistema.Profissionais.Any(p => p.Id == request.ProfissionalId))
            throw new ArgumentException("Gestante ou profissional não encontrado.");

        if ((tipo == "gestante" && request.RemetenteId != request.GestanteId) ||
            (tipo == "profissional" && request.RemetenteId != request.ProfissionalId))
            throw new ArgumentException("Remetente inválido.");

        var mensagem = new Mensagem
        {
            Id = DadosSistema.Mensagens.Count == 0
                ? 1
                : DadosSistema.Mensagens.Max(m => m.Id) + 1,
            GestanteId = request.GestanteId,
            ProfissionalId = request.ProfissionalId,
            RemetenteId = request.RemetenteId,
            RemetenteTipo = tipo,
            Conteudo = conteudo,
            EnviadaEm = DateTime.Now
        };

        DadosSistema.Mensagens.Add(mensagem);

        return mensagem;
    }

    public bool MarcarComoLida(int mensagemId, int destinatarioId)
    {
        var mensagem = DadosSistema.Mensagens.FirstOrDefault(m =>
            m.Id == mensagemId &&
            (m.RemetenteTipo == "gestante"
                ? m.ProfissionalId == destinatarioId
                : m.GestanteId == destinatarioId));

        if (mensagem == null)
            return false;

        mensagem.Lida = true;
        return true;
    }
}
