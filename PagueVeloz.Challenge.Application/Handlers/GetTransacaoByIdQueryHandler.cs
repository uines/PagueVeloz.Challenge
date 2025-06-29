using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Queries.Transacao;
using PagueVeloz.Challenge.Domain.Interfaces;

namespace PagueVeloz.Challenge.Application.Handlers
{
    public class GetTransacaoByIdQueryHandler
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public GetTransacaoByIdQueryHandler(ITransacaoRepository transacaoRepository)
        {
            _transacaoRepository = transacaoRepository;
        }

        public async Task<TransacaoResponseDTO?> Handle(GetTransacaoByIdQuery query)
        {
            var transacao = await _transacaoRepository.GetByIdAsync(query.TransacaoId);

            if (transacao == null)
            {
                return null; 
            }

            return new TransacaoResponseDTO
            {
                Id = transacao.Id,
                Tipo = transacao.Tipo,
                Valor = transacao.Valor,
                DataHora = transacao.DataHora,
                Status = transacao.Status,
                Descricao = transacao.Descricao,
                ContaOrigemId = transacao.ContaOrigemId,
                ContaDestinoId = transacao.ContaDestinoId
            };
        }
    }
}