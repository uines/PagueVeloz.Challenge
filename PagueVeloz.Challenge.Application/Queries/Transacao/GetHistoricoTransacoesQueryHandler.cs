using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Queries.Transacao;
using PagueVeloz.Challenge.Domain.Interfaces;

namespace PagueVeloz.Challenge.Application.Handlers
{
    public class GetHistoricoTransacoesQueryHandler
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public GetHistoricoTransacoesQueryHandler(ITransacaoRepository transacaoRepository)
        {
            _transacaoRepository = transacaoRepository;
        }

        public async Task<IEnumerable<TransacaoResponseDTO>> Handle(GetHistoricoTransacoesQuery query)
        {
            var transacoes = await _transacaoRepository.GetByContaIdAsync(
                query.ContaId,
                query.DataInicio,
                query.DataFim,
                query.Skip,
                query.Take
            );

            return transacoes.Select(t => new TransacaoResponseDTO
            {
                Id = t.Id,
                Tipo = t.Tipo,
                Valor = t.Valor,
                DataHora = t.DataHora,
                Status = t.Status,
                Descricao = t.Descricao,
                ContaOrigemId = t.ContaOrigemId,
                ContaDestinoId = t.ContaDestinoId
            }).ToList();
        }
    }
}