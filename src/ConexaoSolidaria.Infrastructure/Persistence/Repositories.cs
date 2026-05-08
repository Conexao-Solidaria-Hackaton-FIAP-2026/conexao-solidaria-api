using ConexaoSolidaria.Application.Abstractions;
using ConexaoSolidaria.Domain.Entities;
using ConexaoSolidaria.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ConexaoSolidaria.Infrastructure.Persistence;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _ctx;
    public UsuarioRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default)
        => _ctx.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

    public Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => _ctx.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<bool> EmailExisteAsync(string email, CancellationToken ct = default)
        => _ctx.Usuarios.AnyAsync(u => u.Email == email.ToLower(), ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default)
        => await _ctx.Usuarios.AddAsync(usuario, ct);
}

public class CampanhaRepository : ICampanhaRepository
{
    private readonly AppDbContext _ctx;
    public CampanhaRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<Campanha?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => _ctx.Campanhas.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Campanha>> ListarAtivasAsync(CancellationToken ct = default)
        => await _ctx.Campanhas.AsNoTracking()
                     .Where(c => c.Status == CampanhaStatus.Ativa)
                     .ToListAsync(ct);

    public async Task AdicionarAsync(Campanha campanha, CancellationToken ct = default)
        => await _ctx.Campanhas.AddAsync(campanha, ct);

    public void Atualizar(Campanha campanha) => _ctx.Campanhas.Update(campanha);
}

public class DoacaoRepository : IDoacaoRepository
{
    private readonly AppDbContext _ctx;
    public DoacaoRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<Doacao?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => _ctx.Doacoes.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IReadOnlyList<Doacao>> ListarPorCampanhaAsync(Guid campanhaId, CancellationToken ct = default)
        => await _ctx.Doacoes.AsNoTracking()
                     .Where(d => d.CampanhaId == campanhaId)
                     .ToListAsync(ct);

    public async Task AdicionarAsync(Doacao doacao, CancellationToken ct = default)
        => await _ctx.Doacoes.AddAsync(doacao, ct);

    public void Atualizar(Doacao doacao) => _ctx.Doacoes.Update(doacao);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;
    public UnitOfWork(AppDbContext ctx) => _ctx = ctx;
    public Task<int> SalvarAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);
}
