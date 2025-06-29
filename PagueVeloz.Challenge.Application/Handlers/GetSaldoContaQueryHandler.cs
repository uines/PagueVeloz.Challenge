using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Queries.Conta;
using PagueVeloz.Challenge.Domain.Interfaces;

namespace PagueVeloz.Challenge.Application.Handlers
{
    public class GetSaldoContaQueryHandler
    {
        private readonly IContaRepository _contaRepository;

        public GetSaldoContaQueryHandler(IContaRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<SaldoContaDTO> Handle(GetSaldoContaQuery query)
        {
            var conta = await _contaRepository.GetByIdAsync(query.ContaId);

            if (conta == null)
            {
                throw new InvalidOperationException("Conta não encontrada.");
            }

            return new SaldoContaDTO
            {
                ContaId = conta.Id,
                Tipo = conta.Tipo,
                SaldoDisponivel = conta.SaldoDisponivel,
                SaldoBloqueado = conta.SaldoBloqueado,
                SaldoFuturo = conta.SaldoFuturo,
                Status = conta.Status
            };
        }
    }
}