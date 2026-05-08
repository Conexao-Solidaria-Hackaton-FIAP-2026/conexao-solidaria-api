using ConexaoSolidaria.Domain.Enums;

namespace ConexaoSolidaria.Application.DTOs;

public record CriarCampanhaRequest(
    string Titulo,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira);

public record EditarCampanhaRequest(
    string Titulo,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira,
    CampanhaStatus Status);

public record CampanhaPublicaDto(
    Guid Id,
    string Titulo,
    decimal MetaFinanceira,
    decimal ValorArrecadado);

public record CampanhaDetalheDto(
    Guid Id,
    string Titulo,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira,
    decimal ValorArrecadado,
    CampanhaStatus Status);
