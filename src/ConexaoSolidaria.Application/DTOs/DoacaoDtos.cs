namespace ConexaoSolidaria.Application.DTOs;

public record CriarDoacaoRequest(Guid IdCampanha, decimal ValorDoacao);
public record DoacaoRegistradaResponse(Guid DoacaoId, string Status);
public record DoacaoResponse(Guid Id, Guid DoadorId, decimal Valor, string Status, DateTime? ProcessadaEm);
