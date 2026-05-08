using ConexaoSolidaria.Domain.Common;
using ConexaoSolidaria.Domain.Exceptions;

namespace ConexaoSolidaria.Domain.Entities;

public enum DoacaoStatus { Pendente, Processada, Falha }

public class Doacao : Entity
{
    public Guid CampanhaId { get; private set; }
    public Guid DoadorId { get; private set; }
    public decimal Valor { get; private set; }
    public DoacaoStatus Status { get; private set; }
    public DateTime? ProcessadaEm { get; private set; }

    private Doacao() { }

    public Doacao(Guid campanhaId, Guid doadorId, decimal valor)
    {
        if (campanhaId == Guid.Empty) throw new DomainException("CampanhaId invalido.");
        if (doadorId == Guid.Empty) throw new DomainException("DoadorId invalido.");
        if (valor <= 0) throw new DomainException("Valor deve ser maior que zero.");

        CampanhaId = campanhaId;
        DoadorId = doadorId;
        Valor = valor;
        Status = DoacaoStatus.Pendente;
    }

    public void MarcarProcessada()
    {
        Status = DoacaoStatus.Processada;
        ProcessadaEm = DateTime.UtcNow;
    }

    public void MarcarFalha() => Status = DoacaoStatus.Falha;
}
