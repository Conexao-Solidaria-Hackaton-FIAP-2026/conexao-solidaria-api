using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ConexaoSolidaria.Infrastructure.Messaging;

public class RabbitMqConnectionFactory : IAsyncDisposable
{
    private readonly RabbitMqOptions _opt;
    private IConnection? _connection;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> opt) => _opt = opt.Value;

    public async Task<IConnection> GetConnectionAsync(CancellationToken ct = default)
    {
        if (_connection is { IsOpen: true }) return _connection;
        await _lock.WaitAsync(ct);
        try
        {
            if (_connection is { IsOpen: true }) return _connection;
            var factory = new ConnectionFactory
            {
                HostName = _opt.Host,
                Port = _opt.Port,
                UserName = _opt.UserName,
                Password = _opt.Password,
                VirtualHost = _opt.VirtualHost,
                AutomaticRecoveryEnabled = true
            };
            _connection = await factory.CreateConnectionAsync(ct);
            return _connection;
        }
        finally { _lock.Release(); }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null) await _connection.DisposeAsync();
    }
}
