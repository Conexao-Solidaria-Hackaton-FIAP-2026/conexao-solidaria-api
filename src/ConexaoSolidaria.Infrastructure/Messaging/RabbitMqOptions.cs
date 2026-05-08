namespace ConexaoSolidaria.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public const string Section = "RabbitMq";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string Exchange { get; set; } = "conexao.solidaria";
    public string QueueDoacoes { get; set; } = "doacoes.recebidas";
    public string RoutingKeyDoacoes { get; set; } = "doacao.recebida";
}
