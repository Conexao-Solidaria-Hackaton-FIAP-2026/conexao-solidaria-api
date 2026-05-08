using System.Security.Claims;
using ConexaoSolidaria.Application.DTOs;
using ConexaoSolidaria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConexaoSolidaria.Api.Controllers;

[ApiController]
[Route("api/campanhas")]
public class CampanhasController : ControllerBase
{
    private readonly CampanhaService _svc;
    public CampanhasController(CampanhaService svc) => _svc = svc;

    [HttpPost]
    [Authorize(Policy = "GestorONG")]
    public async Task<IActionResult> Criar([FromBody] CriarCampanhaRequest req, CancellationToken ct)
    {
        var gestorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                  ?? User.FindFirstValue("sub")!);
        var id = await _svc.CriarAsync(req, gestorId, ct);
        return Created($"/api/campanhas/{id}", new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "GestorONG")]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarCampanhaRequest req,
                                             CancellationToken ct)
    {
        await _svc.EditarAsync(id, req, ct);
        return NoContent();
    }
}
