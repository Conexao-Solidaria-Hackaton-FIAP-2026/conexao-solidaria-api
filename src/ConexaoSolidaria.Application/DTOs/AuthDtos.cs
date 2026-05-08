namespace ConexaoSolidaria.Application.DTOs;

public record CadastrarDoadorRequest(string NomeCompleto, string Email, string Cpf, string Senha);
public record CadastrarGestorRequest(string NomeCompleto, string Email, string Cpf, string Senha);
public record LoginRequest(string Email, string Senha);
public record LoginResponse(string Token, string Role, string NomeCompleto);
