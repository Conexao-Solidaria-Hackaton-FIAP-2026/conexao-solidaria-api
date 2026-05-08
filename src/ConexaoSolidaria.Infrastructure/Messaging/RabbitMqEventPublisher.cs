using System.Text;
using System.Text.Json;
using ConexaoSolidaria.Application.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ConexaoSolidaria.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly RabbitMqConnectionFactory _factory;
    private readonly RabbitMqOptions _opt;

    public RabbitMqEventPublisher(RabbitMqConnectionFactory factory, IOptions<RabbitMqOptions> opt)
    {
        _factory = factory;
        _opt = opt.Value;
    }

    public async Task PublicarAsync<T>(T evento, string routingKey, CancellationToken ct = default)
        where T : class
    {
        var conn = await _factory.GetConnectionAsync(ct);
        await using var channel = await conn.CreateChannelAsync(cancellationToken: ct);

        // Envelope compatível com MassTransit (worker usa IConsumer<T>)
        var typeName = typeof(T).Name;
        var envelope = new
        {
            messageId = Guid.NewGuid(),
            messageType = new[] { $"urn:message:DonationWorker.Domain:{typeName}" },
            message = evento
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope));
        var props = new BasicProperties { Persistent = true, ContentType = "application/vnd.masstransit+json" };

        // MassTransit cria exchange com nome do tipo — publica direto nela
        await channel.BasicPublishAsync(exchange: "", routingKey: "DoacaoRecebida",
                                         mandatory: false, basicProperties: props,
                                         body: body, cancellationToken: ct);
    }
}
