using ConexaoSolidaria.Application.Abstractions;
using ConexaoSolidaria.Application.DTOs;
using ConexaoSolidaria.Domain.Entities;
using ConexaoSolidaria.Domain.Events;
using ConexaoSolidaria.Domain.Exceptions;

namespace ConexaoSolidaria.Application.Services;

public class DoacaoService
{
    public const string RoutingKeyDoacaoRecebida = "doacao.recebida";

    private readonly ICampanhaRepository _campanhas;
    private readonly IDoacaoRepository _doacoes;
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _publisher;

    public DoacaoService(ICampanhaRepository campanhas, IDoacaoRepository doacoes,
                         IUnitOfWork uow, IEventPublisher publisher)
    {
        _campanhas = campanhas;
        _doacoes = doacoes;
        _uow = uow;
        _publisher = publisher;
    }

    public async Task<IReadOnlyList<DoacaoResponse>> ListarPorCampanhaAsync(Guid campanhaId, CancellationToken ct)
    {
        var doacoes = await _doacoes.ListarPorCampanhaAsync(campanhaId, ct);
        return doacoes.Select(d => new DoacaoResponse(d.Id, d.DoadorId, d.Valor, d.Status.ToString(), d.ProcessadaEm)).ToList();
    }

    public async Task<DoacaoRegistradaResponse> RegistrarAsync(CriarDoacaoRequest req, Guid doadorId,
                                                                CancellationToken ct)
    {
        var campanha = await _campanhas.ObterPorIdAsync(req.IdCampanha, ct)
                       ?? throw new DomainException("Campanha nao encontrada.");
        if (!campanha.AceitaDoacao())
            throw new DomainException("Campanha encerrada ou cancelada.");

        var doacao = new Doacao(req.IdCampanha, doadorId, req.ValorDoacao);
        await _doacoes.AdicionarAsync(doacao, ct);
        await _uow.SalvarAsync(ct);

        var evento = new DoacaoRecebidaEvent
        {
            Id          = doacao.Id,
            CampanhaId  = doacao.CampanhaId,
            DoadorId    = doacao.DoadorId,
            ValorDoacao = doacao.Valor,
            CriadoEm    = DateTime.UtcNow
        };
        await _publisher.PublicarAsync(evento, RoutingKeyDoacaoRecebida, ct);

        return new DoacaoRegistradaResponse(doacao.Id, doacao.Status.ToString());
    }
}
