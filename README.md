# Conexao Solidaria

Plataforma digital MVP para a ONG Esperanca Solidaria. Permite gestao de campanhas, registro de doacoes e processamento assincrono via RabbitMQ.

## Stack

- .NET 10 (ASP.NET Core Web API)
- SQL Server 2022
- RabbitMQ 3.13 (publicacao de eventos)
- Entity Framework Core 10
- JWT Bearer + BCrypt
- Serilog + Prometheus (endpoint `/metrics`)
- xUnit + FluentAssertions

## Estrutura

```
ConexaoSolidaria.slnx
src/
  ConexaoSolidaria.Api/             # Web API - autenticacao, campanhas, doacoes
  ConexaoSolidaria.Application/     # Casos de uso, DTOs, abstracoes
  ConexaoSolidaria.Domain/          # Entidades, regras, eventos
  ConexaoSolidaria.Infrastructure/  # EF Core (SQL Server), repositorios, RabbitMQ, JWT, BCrypt
tests/
  ConexaoSolidaria.Domain.Tests/    # xUnit
.github/workflows/ci.yml            # Pipeline CI (build + test)
docker-compose.yml                  # Stack local
```

## Fluxo de Doacao (Assincrono)

1. `POST /api/doacoes` persiste doacao em `Pendente` + publica `DoacaoRecebidaEvent` na fila `DoacaoRecebida` (envelope MassTransit).
2. API responde **202 Accepted** imediatamente.
3. Worker consome fila `DoacaoRecebida`, atualiza status para `Processada` e soma valor na campanha.

### Contrato do evento

```csharp
public record DoacaoRecebidaEvent
{
    public Guid     Id          { get; init; }
    public Guid     CampanhaId  { get; init; }
    public Guid     DoadorId    { get; init; }
    public decimal  ValorDoacao { get; init; }
    public DateTime CriadoEm    { get; init; }
}
```

Publicado com envelope MassTransit (`ContentType = application/vnd.masstransit+json`) direto na fila `DoacaoRecebida`.

## Endpoints

| Metodo | Rota                                    | Role       |
|--------|-----------------------------------------|------------|
| POST   | /api/auth/cadastrar-doador              | Publico    |
| POST   | /api/auth/cadastrar-gestor              | Publico    |
| POST   | /api/auth/login                         | Publico    |
| GET    | /api/painel/campanhas-ativas            | Publico    |
| POST   | /api/campanhas                          | GestorONG  |
| PUT    | /api/campanhas/{id}                     | GestorONG  |
| POST   | /api/doacoes                            | Doador     |
| GET    | /api/doacoes/campanha/{campanhaId}      | Doador     |
| GET    | /health                                 | Publico    |
| GET    | /metrics                                | Publico    |

## Rodar localmente

> **SQL Server e RabbitMQ sao provisionados pelo repositorio de infra.**
> Certifique-se de que a stack de infra esta rodando antes de subir esta API.

```bash
dotnet run --project src/ConexaoSolidaria.Api
```

- API: http://localhost:5000/swagger (ou porta definida em `launchSettings.json`)

Com Docker:

```bash
docker compose up --build
```

## Variaveis de ambiente

| Variavel                           | Default                                                        |
|------------------------------------|----------------------------------------------------------------|
| `ConnectionStrings__SqlServer`     | `Server=sqlserver,1433;Database=ConexaoSolidaria;...`          |
| `RabbitMq__Host`                   | `rabbitmq`                                                     |
| `RabbitMq__Port`                   | `5672`                                                         |
| `Jwt__SecretKey`                   | (minimo 32 chars)                                              |
| `Jwt__Issuer` / `Jwt__Audience`    | `ConexaoSolidaria`                                             |
| `Database__AutoMigrate`            | `true` (aplica EF migrations no startup)                       |

## Testes

```bash
dotnet test ConexaoSolidaria.slnx
```

## Primeiro Gestor

Use `POST /api/auth/cadastrar-gestor` com o mesmo payload do doador. Cria usuario com role `GestorONG` diretamente.
