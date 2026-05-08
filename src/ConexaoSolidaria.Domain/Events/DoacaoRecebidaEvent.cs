namespace ConexaoSolidaria.Domain.Events;

public record DoacaoRecebidaEvent
{
    public Guid     Id          { get; init; }
    public Guid     CampanhaId  { get; init; }
    public Guid     DoadorId    { get; init; }
    public decimal  ValorDoacao { get; init; }
    public DateTime CriadoEm    { get; init; }
}
