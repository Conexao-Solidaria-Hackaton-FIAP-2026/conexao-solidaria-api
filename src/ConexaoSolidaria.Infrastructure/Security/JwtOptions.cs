namespace ConexaoSolidaria.Infrastructure.Security;

public class JwtOptions
{
    public const string Section = "Jwt";
    public string Issuer { get; set; } = "ConexaoSolidaria";
    public string Audience { get; set; } = "ConexaoSolidaria.Clients";
    public string SecretKey { get; set; } = default!;
    public int ExpiracaoMinutos { get; set; } = 60;
}
