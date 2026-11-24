
using Materna.API.Models;
using Materna.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Materna.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuplementosController : ControllerBase
{
    private readonly SuplementoService _service = new();

    [HttpGet("gestante/{gestanteId}")]
    public ActionResult<List<Suplemento>> Listar(int gestanteId)
    {
        return Ok(_service.ListarPorGestante(gestanteId));
    }

    [HttpPost]
    public ActionResult<Suplemento> Cadastrar(Suplemento suplemento)
    {
        if (string.IsNullOrWhiteSpace(suplemento.Nome) ||
            string.IsNullOrWhiteSpace(suplemento.Dosagem) ||
            suplemento.Horarios.Count == 0)
            return BadRequest("Informe nome, dosagem e horários.");

        var resultado = _service.Cadastrar(suplemento);

        if (resultado == null)
            return BadRequest("Gestante ou profissional não encontrado.");

        return Ok(resultado);
    }

    [HttpPut("{id}")]
    public ActionResult<Suplemento> Atualizar(int id, Suplemento dados)
    {
        if (string.IsNullOrWhiteSpace(dados.Nome) ||
            string.IsNullOrWhiteSpace(dados.Dosagem) ||
            dados.Horarios.Count == 0)
            return BadRequest("Informe nome, dosagem e horários.");

        var resultado = _service.Atualizar(id, dados);

        if (resultado == null)
            return NotFound("Suplemento não encontrado.");

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public IActionResult Excluir(int id)
    {
        if (!_service.Excluir(id))
            return NotFound("Suplemento não encontrado.");

        return Ok("Suplemento excluído.");
    }

    [HttpGet("historico/{gestanteId}")]
    public ActionResult<List<RegistroSuplemento>> Historico(
        int gestanteId,
        [FromQuery] DateTime? data)
    {
        return Ok(_service.ListarHistorico(
            gestanteId,
            data?.Date ?? DateTime.Today));
    }

    [HttpPost("{id}/doses")]
    public ActionResult<RegistroSuplemento> RegistrarDose(
        int id,
        RegistrarDoseRequest dados)
    {
        if (string.IsNullOrWhiteSpace(dados.Horario))
            return BadRequest("Informe o horário da dose.");

        var resultado = _service.RegistrarDose(
            id,
            dados.Data,
            dados.Horario,
            dados.Tomado);

        if (resultado == null)
            return NotFound("Suplemento não encontrado.");

        return Ok(resultado);
    }
}

public class RegistrarDoseRequest
{
    public DateTime Data { get; set; }
    public string Horario { get; set; } = string.Empty;
    public bool Tomado { get; set; }
}
