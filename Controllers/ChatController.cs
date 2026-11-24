
using Microsoft.AspNetCore.Mvc;
using Materna.API.Models;
using Materna.API.Services;

namespace Materna.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _service = new();

    [HttpGet("gestante/{gestanteId}")]
    public IActionResult ListarPorGestante(int gestanteId)
    {
        return Ok(_service.ListarPorGestante(gestanteId));
    }

    [HttpGet("profissional/{profissionalId}")]
    public IActionResult ListarPorProfissional(int profissionalId)
    {
        return Ok(_service.ListarPorProfissional(profissionalId));
    }

    [HttpPost]
    public IActionResult Enviar([FromBody] MensagemRequest request)
    {
        try
        {
            return Ok(_service.Enviar(request));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{mensagemId}/lida")]
    public IActionResult MarcarComoLida(int mensagemId, [FromQuery] int destinatarioId)
    {
        return _service.MarcarComoLida(mensagemId, destinatarioId)
            ? Ok()
            : NotFound(new { mensagem = "Mensagem não encontrada." });
    }
}
