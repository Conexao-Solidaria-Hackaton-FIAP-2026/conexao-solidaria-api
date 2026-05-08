using ConexaoSolidaria.Domain.Common;
using ConexaoSolidaria.Domain.Enums;
using ConexaoSolidaria.Domain.Exceptions;

namespace ConexaoSolidaria.Domain.Entities;

public class Campanha : Entity
{
    public string Titulo { get; private set; } = default!;
    public string Descricao { get; private set; } = default!;
    public DateTime DataInicio { get; private set; }
    public DateTime DataFim { get; private set; }
    public decimal MetaFinanceira { get; private set; }
    public decimal ValorArrecadado { get; private set; }
    public CampanhaStatus Status { get; private set; }
    public Guid GestorId { get; private set; }

    private Campanha() { }

    public Campanha(string titulo, string descricao, DateTime dataInicio, DateTime dataFim,
                    decimal metaFinanceira, Guid gestorId)
    {
        if (string.IsNullOrWhiteSpace(titulo)) throw new DomainException("Titulo obrigatorio.");
        if (string.IsNullOrWhiteSpace(descricao)) throw new DomainException("Descricao obrigatoria.");
        if (dataFim < DateTime.UtcNow) throw new DomainException("DataFim nao pode estar no passado.");
        if (dataInicio > dataFim) throw new DomainException("DataInicio maior que DataFim.");
        if (metaFinanceira <= 0) throw new DomainException("MetaFinanceira deve ser maior que zero.");

        Titulo = titulo;
        Descricao = descricao;
        DataInicio = dataInicio;
        DataFim = dataFim;
        MetaFinanceira = metaFinanceira;
        ValorArrecadado = 0m;
        Status = CampanhaStatus.Ativa;
        GestorId = gestorId;
    }

    public void Editar(string titulo, string descricao, DateTime dataInicio, DateTime dataFim,
                       decimal metaFinanceira, CampanhaStatus status)
    {
        if (dataFim < DateTime.UtcNow && status == CampanhaStatus.Ativa)
            throw new DomainException("DataFim no passado para campanha Ativa.");
        if (metaFinanceira <= 0) throw new DomainException("MetaFinanceira deve ser maior que zero.");

        Titulo = titulo;
        Descricao = descricao;
        DataInicio = dataInicio;
        DataFim = dataFim;
        MetaFinanceira = metaFinanceira;
        Status = status;
    }

    public void RegistrarDoacao(decimal valor)
    {
        if (Status != CampanhaStatus.Ativa)
            throw new DomainException("Campanha nao esta ativa.");
        if (valor <= 0) throw new DomainException("Valor invalido.");
        ValorArrecadado += valor;
    }

    public bool AceitaDoacao() => Status == CampanhaStatus.Ativa;
}
