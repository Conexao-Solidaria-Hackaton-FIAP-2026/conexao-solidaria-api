using ConexaoSolidaria.Application.DTOs;
using ConexaoSolidaria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConexaoSolidaria.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _svc;
    public AuthController(AuthService svc) => _svc = svc;

    [HttpPost("cadastrar-doador")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CadastrarDoador([FromBody] CadastrarDoadorRequest req,
                                                      CancellationToken ct)
    {
        var id = await _svc.CadastrarDoadorAsync(req, ct);
        return Created($"/api/usuarios/{id}", new { id });
    }

    [HttpPost("cadastrar-gestor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CadastrarGestor([FromBody] CadastrarGestorRequest req,
                                                      CancellationToken ct)
    {
        var id = await _svc.CadastrarGestorAsync(req, ct);
        return Created($"/api/usuarios/{id}", new { id });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        => Ok(await _svc.LoginAsync(req, ct));
}
