using Microsoft.AspNetCore.Mvc;
using Materna.API.Models;
using Materna.API.Services;

namespace Materna.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamesController : ControllerBase
{
    private readonly ExameService _service = new ExameService();

    [HttpGet]
    public ActionResult<List<Exame>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpGet("{id}")]
    public ActionResult<Exame> GetPorId(int id)
    {
        var exame = _service.BuscarPorId(id);

        if (exame == null)
            return NotFound();

        return Ok(exame);
    }

    [HttpPost]
    public ActionResult<Exame> Post(Exame exame)
    {
        var novo = _service.Criar(exame);

        return CreatedAtAction(
            nameof(GetPorId),
            new { id = novo.Id },
            novo
        );
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, Exame exame)
    {
        if (!_service.Atualizar(id, exame))
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