
using Microsoft.AspNetCore.Mvc;
using Materna.API.Models;
using Materna.API.Services;

namespace Materna.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly ConsultaService _service = new();

    [HttpGet]
    public ActionResult<List<Consulta>> Get()
    {
        return Ok(_service.Listar());
    }

    [HttpGet("busca-ativa")]
    public ActionResult<List<BuscaAtivaResponse>> GetBuscaAtiva()
    {
        return Ok(_service.ListarBuscaAtiva());
    }

    [HttpGet("gestante/{gestanteId:int}")]
    public ActionResult<List<Consulta>> GetPorGestante(int gestanteId)
    {
        return Ok(_service.ListarPorGestante(gestanteId));
    }

    [HttpGet("{id:int}")]
    public ActionResult<Consulta> GetPorId(int id)
    {
        var consulta = _service.BuscarPorId(id);

        if (consulta == null)
            return NotFound();

        return Ok(consulta);
    }

    [HttpPost]
    public ActionResult<Consulta> Post(Consulta consulta)
    {
        var nova = _service.Criar(consulta);

        return CreatedAtAction(nameof(GetPorId), new { id = nova.Id }, nova);
    }

    [HttpPut("{id:int}")]
    public IActionResult Put(int id, Consulta consulta)
    {
        if (!_service.Atualizar(id, consulta))
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_service.Excluir(id))
            return NotFound();

        return NoContent();
    }
}
