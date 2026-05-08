using ConexaoSolidaria.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConexaoSolidaria.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<CampanhaService>();
        services.AddScoped<DoacaoService>();
        return services;
    }
}
