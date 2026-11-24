using Microsoft.AspNetCore.Mvc;
using Materna.API.Models;
using Materna.API.Services;

namespace Materna.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfissionaisController : ControllerBase
{
    private readonly ProfissionalService _service = new ProfissionalService();

    [HttpGet]
    public ActionResult<List<Profissional>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpGet("{id}")]
    public ActionResult<Profissional> GetPorId(int id)
    {
        var profissional = _service.BuscarPorId(id);

        if (profissional == null)
            return NotFound();

        return Ok(profissional);
    }

    [HttpPost]
    public ActionResult<Profissional> Post(Profissional profissional)
    {
        var novo = _service.Criar(profissional);

        return CreatedAtAction(
            nameof(GetPorId),
            new { id = novo.Id },
            novo
        );
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, Profissional profissional)
    {
        if (!_service.Atualizar(id, profissional))
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_service.Excluir(id))
            return NotFound();

        return NoContent();
    }
}