# Conexao Solidaria - API (Pessoa 1)

Plataforma digital MVP para a ONG Esperanca Solidaria.

## Divisao do grupo

Este repositorio contem o escopo da **Pessoa 1**:

- API principal (JWT, RBAC, Doador, Campanhas, Painel de Transparencia, publicacao de evento no RabbitMQ)
- `docker-compose.yml` (stack local)
- GitHub Actions (CI)

Fora do escopo deste desenvolvedor (Pessoa 2):

- Worker consumidor do RabbitMQ (`donation-worker`)
- Dockerfiles (API + Worker)
- Manifests Kubernetes (Deployments, Services, ConfigMaps)
- Grafana (dashboards + metricas)

Decisoes conjuntas ja contempladas aqui:

- Banco **SQL Server**
- Contrato do evento `DoacaoRecebidaEvent`
- Variaveis de ambiente esperadas por cada servico (ver `appsettings.json` + `docker-compose.yml`)
- Mono-repo

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
docker-compose.yml                  # Stack local (SQL Server + RabbitMQ + API)
```

## Fluxo de Doacao (Assincrono)

1. `POST /api/doacoes` persiste doacao em `Pendente` + publica `DoacaoRecebidaEvent` na fila `DoacaoRecebida` (envelope MassTransit).
2. API responde **202 Accepted** imediatamente.
3. Worker (escopo Pessoa 2) consome fila `DoacaoRecebida`, atualiza status para `Processada` e soma valor na campanha.

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

| Metodo | Rota                             | Role       |
|--------|----------------------------------|------------|
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

> **SQL Server e RabbitMQ sao provisionados pelo repositorio de infra (Pessoa 2).**
> Certifique-se de que a stack de infra esta rodando antes de subir esta API.

Com a infra ja no ar, sobe apenas a API:

```bash
dotnet run --project src/ConexaoSolidaria.Api
```

- API: http://localhost:5000/swagger (ou porta definida em `launchSettings.json`)

Quando o Dockerfile da API estiver disponivel (Pessoa 2):

```bash
docker compose up --build
```

## Variaveis de ambiente consumidas pela API

| Variavel                           | Default (compose)                                              |
|------------------------------------|----------------------------------------------------------------|
| `ConnectionStrings__SqlServer`     | `Server=sqlserver,1433;Database=ConexaoSolidaria;...`          |
| `RabbitMq__Host`                   | `rabbitmq`                                                     |
| `RabbitMq__Port`                   | `5672`                                                         |
| `RabbitMq__Exchange`               | `conexao.solidaria`                                            |
| `RabbitMq__QueueDoacoes`           | `doacoes.recebidas`                                            |
| `RabbitMq__RoutingKeyDoacoes`      | `doacao.recebida`                                              |
| `Jwt__SecretKey`                   | (minimo 32 chars)                                              |
| `Jwt__Issuer` / `Jwt__Audience`    | `ConexaoSolidaria` / `ConexaoSolidaria.Clients`                |
| `Database__AutoMigrate`            | `true` (aplica EF migrations no startup)                       |

## Testes

```bash
dotnet test ConexaoSolidaria.slnx
```

## Primeiro Gestor

Use o endpoint `POST /api/auth/cadastrar-gestor` com o mesmo payload do doador. Cria usuario com role `GestorONG` diretamente.
