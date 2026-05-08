using ConexaoSolidaria.Domain.Entities;
using ConexaoSolidaria.Domain.Enums;
using ConexaoSolidaria.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ConexaoSolidaria.Domain.Tests;

public class CampanhaTests
{
    [Fact]
    public void DeveCriarCampanhaValida()
    {
        var c = new Campanha("T", "D", DateTime.UtcNow, DateTime.UtcNow.AddDays(10),
                             100m, Guid.NewGuid());
        c.Status.Should().Be(CampanhaStatus.Ativa);
        c.ValorArrecadado.Should().Be(0);
    }

    [Fact]
    public void DeveFalharQuandoDataFimNoPassado()
    {
        var act = () => new Campanha("T", "D", DateTime.UtcNow.AddDays(-10),
                                      DateTime.UtcNow.AddDays(-1), 100m, Guid.NewGuid());
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void DeveFalharQuandoMetaZero()
    {
        var act = () => new Campanha("T", "D", DateTime.UtcNow, DateTime.UtcNow.AddDays(1),
                                      0m, Guid.NewGuid());
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RegistrarDoacaoDeveIncrementarValor()
    {
        var c = new Campanha("T", "D", DateTime.UtcNow, DateTime.UtcNow.AddDays(5),
                             500m, Guid.NewGuid());
        c.RegistrarDoacao(100m);
        c.RegistrarDoacao(50m);
        c.ValorArrecadado.Should().Be(150m);
    }
}
