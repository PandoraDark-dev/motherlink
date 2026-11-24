
using Microsoft.AspNetCore.Mvc;
using Materna.API.Models;
using Materna.API.Services;

namespace Materna.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanosPartoController : ControllerBase
{
    private readonly PlanoPartoService _service = new();

    [HttpGet("gestante/{gestanteId:int}")]
    public ActionResult<PlanoParto> GetPorGestante(int gestanteId)
    {
        var plano = _service.BuscarPorGestante(gestanteId);

        if (plano == null)
            return NotFound();

        return Ok(plano);
    }

    [HttpPut("gestante/{gestanteId:int}")]
    public ActionResult<PlanoParto> Salvar(int gestanteId, PlanoParto plano)
    {
        if (gestanteId != plano.GestanteId)
            return BadRequest("O ID da gestante não corresponde ao plano.");

        var salvo = _service.Salvar(gestanteId, plano.Preferencias);

        if (salvo == null)
            return NotFound("Gestante não encontrada.");

        return Ok(salvo);
    }
}
