using Microsoft.AspNetCore.Mvc;
using Materna.API.Models;
using Materna.API.Services;

namespace Materna.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GestantesController : ControllerBase
{
    private readonly GestanteService _service = new GestanteService();

    [HttpGet]
    public ActionResult<List<Gestante>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpGet("{id}")]
    public ActionResult<Gestante> GetPorId(int id)
    {
        var gestante = _service.BuscarPorId(id);

        if (gestante == null)
            return NotFound();

        return Ok(gestante);
    }

    [HttpPost]
    public ActionResult<Gestante> Post(Gestante gestante)
    {
        var novaGestante = _service.Criar(gestante);

        return CreatedAtAction(
            nameof(GetPorId),
            new { id = novaGestante.Id },
            novaGestante
        );
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, Gestante gestante)
    {
        var atualizado = _service.Atualizar(id, gestante);

        if (!atualizado)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var excluido = _service.Excluir(id);

        if (!excluido)
            return NotFound();

        return NoContent();
    }
}