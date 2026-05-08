using ConexaoSolidaria.Domain.Common;
using ConexaoSolidaria.Domain.Enums;
using ConexaoSolidaria.Domain.Exceptions;

namespace ConexaoSolidaria.Domain.Entities;

public class Usuario : Entity
{
    public string NomeCompleto { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Cpf { get; private set; } = default!;
    public string SenhaHash { get; private set; } = default!;
    public UsuarioRole Role { get; private set; }

    private Usuario() { }

    public Usuario(string nomeCompleto, string email, string cpf, string senhaHash, UsuarioRole role)
    {
        if (string.IsNullOrWhiteSpace(nomeCompleto)) throw new DomainException("Nome obrigatorio.");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email obrigatorio.");
        if (!ValidarCpf(cpf)) throw new DomainException("CPF invalido.");
        if (string.IsNullOrWhiteSpace(senhaHash)) throw new DomainException("Senha obrigatoria.");

        NomeCompleto = nomeCompleto;
        Email = email.Trim().ToLowerInvariant();
        Cpf = new string(cpf.Where(char.IsDigit).ToArray());
        SenhaHash = senhaHash;
        Role = role;
    }

    public static bool ValidarCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        var digitos = new string(cpf.Where(char.IsDigit).ToArray());
        if (digitos.Length != 11) return false;
        if (digitos.Distinct().Count() == 1) return false;

        int Calc(string baseStr, int pesoInicial)
        {
            var soma = 0;
            for (var i = 0; i < baseStr.Length; i++)
                soma += (baseStr[i] - '0') * (pesoInicial - i);
            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        var d1 = Calc(digitos[..9], 10);
        var d2 = Calc(digitos[..9] + d1, 11);
        return digitos[9] - '0' == d1 && digitos[10] - '0' == d2;
    }
}
