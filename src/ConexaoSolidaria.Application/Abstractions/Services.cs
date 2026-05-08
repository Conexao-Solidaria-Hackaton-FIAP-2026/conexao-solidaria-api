using ConexaoSolidaria.Domain.Entities;

namespace ConexaoSolidaria.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verify(string senha, string hash);
}

public interface IJwtTokenService
{
    string GerarToken(Usuario usuario);
}

public interface IEventPublisher
{
    Task PublicarAsync<T>(T evento, string routingKey, CancellationToken ct = default) where T : class;
}
