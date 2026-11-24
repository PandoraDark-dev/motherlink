
using Microsoft.AspNetCore.Mvc;
using Materna.API.Data;
using Materna.API.Models;

namespace Materna.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest login)
    {
        var gestante = DadosSistema.Gestantes.FirstOrDefault(g =>
            (g.CPF == login.CPF || g.CartaoSUS == login.CartaoSUS) &&
            g.Senha == login.Senha);

        if (gestante != null)
        {
            return Ok(new LoginResponse
            {
                Sucesso = true,
                TipoUsuario = "Gestante",
                Id = gestante.Id,
                Nome = gestante.Nome,
                Mensagem = "Login realizado com sucesso."
            });
        }

        var profissional = DadosSistema.Profissionais.FirstOrDefault(p =>
            (p.Registro == login.COREN || p.Email == login.EMAIL) &&
            p.Senha == login.Senha);

        if (profissional != null)
        {
            return Ok(new LoginResponse
            {
                Sucesso = true,
                TipoUsuario = "Profissional",
                Funcao = profissional.Funcao,
                Id = profissional.Id,
                Nome = profissional.Nome,
                Mensagem = "Login realizado com sucesso."
            });
        }

        return Unauthorized(new LoginResponse
        {
            Sucesso = false,
            Mensagem = "Dados inválidos."
        });
    }
}
