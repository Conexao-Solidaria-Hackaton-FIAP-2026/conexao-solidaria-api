using System.Security.Claims;
using ConexaoSolidaria.Application.DTOs;
using ConexaoSolidaria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConexaoSolidaria.Api.Controllers;

[ApiController]
[Route("api/doacoes")]
[Authorize(Policy = "Doador")]
public class DoacoesController : ControllerBase
{
    private readonly DoacaoService _svc;
    public DoacoesController(DoacaoService svc) => _svc = svc;

    [HttpGet("campanha/{campanhaId:guid}")]
    public async Task<IActionResult> ListarPorCampanha(Guid campanhaId, CancellationToken ct)
        => Ok(await _svc.ListarPorCampanhaAsync(campanhaId, ct));

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarDoacaoRequest req, CancellationToken ct)
    {
        var doadorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                   ?? User.FindFirstValue("sub")!);
        var resp = await _svc.RegistrarAsync(req, doadorId, ct);
        return Accepted(resp);
    }
}
