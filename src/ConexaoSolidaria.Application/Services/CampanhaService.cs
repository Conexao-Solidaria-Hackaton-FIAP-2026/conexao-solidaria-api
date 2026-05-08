using ConexaoSolidaria.Application.Abstractions;
using ConexaoSolidaria.Application.DTOs;
using ConexaoSolidaria.Domain.Entities;
using ConexaoSolidaria.Domain.Exceptions;

namespace ConexaoSolidaria.Application.Services;

public class CampanhaService
{
    private readonly ICampanhaRepository _repo;
    private readonly IUnitOfWork _uow;

    public CampanhaService(ICampanhaRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Guid> CriarAsync(CriarCampanhaRequest req, Guid gestorId, CancellationToken ct)
    {
        var c = new Campanha(req.Titulo, req.Descricao, req.DataInicio, req.DataFim,
                             req.MetaFinanceira, gestorId);
        await _repo.AdicionarAsync(c, ct);
        await _uow.SalvarAsync(ct);
        return c.Id;
    }

    public async Task EditarAsync(Guid id, EditarCampanhaRequest req, CancellationToken ct)
    {
        var c = await _repo.ObterPorIdAsync(id, ct)
                ?? throw new DomainException("Campanha nao encontrada.");
        c.Editar(req.Titulo, req.Descricao, req.DataInicio, req.DataFim,
                 req.MetaFinanceira, req.Status);
        _repo.Atualizar(c);
        await _uow.SalvarAsync(ct);
    }

    public async Task<IReadOnlyList<CampanhaPublicaDto>> ListarAtivasPublicasAsync(CancellationToken ct)
    {
        var ativas = await _repo.ListarAtivasAsync(ct);
        return ativas.Select(c => new CampanhaPublicaDto(
            c.Id, c.Titulo, c.MetaFinanceira, c.ValorArrecadado)).ToList();
    }
}
