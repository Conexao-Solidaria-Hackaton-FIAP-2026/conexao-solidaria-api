using ConexaoSolidaria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConexaoSolidaria.Api.Controllers;

[ApiController]
[Route("api/painel")]
public class PainelPublicoController : ControllerBase
{
    private readonly CampanhaService _svc;
    public PainelPublicoController(CampanhaService svc) => _svc = svc;

    [HttpGet("campanhas-ativas")]
    public async Task<IActionResult> Listar(CancellationToken ct)
        => Ok(await _svc.ListarAtivasPublicasAsync(ct));
}
