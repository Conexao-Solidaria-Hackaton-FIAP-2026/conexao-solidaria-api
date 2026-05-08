using ConexaoSolidaria.Domain.Entities;

namespace ConexaoSolidaria.Application.Abstractions;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> EmailExisteAsync(string email, CancellationToken ct = default);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct = default);
}

public interface ICampanhaRepository
{
    Task<Campanha?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Campanha>> ListarAtivasAsync(CancellationToken ct = default);
    Task AdicionarAsync(Campanha campanha, CancellationToken ct = default);
    void Atualizar(Campanha campanha);
}

public interface IDoacaoRepository
{
    Task<Doacao?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Doacao>> ListarPorCampanhaAsync(Guid campanhaId, CancellationToken ct = default);
    Task AdicionarAsync(Doacao doacao, CancellationToken ct = default);
    void Atualizar(Doacao doacao);
}

public interface IUnitOfWork
{
    Task<int> SalvarAsync(CancellationToken ct = default);
}
