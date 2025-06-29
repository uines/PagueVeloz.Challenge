using PagueVeloz.Challenge.Application.Commands.Transacao;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Enums;
using PagueVeloz.Challenge.Domain.Interfaces;

namespace PagueVeloz.Challenge.Application.Handlers
{
    public class RealizarTransferenciaCommandHandler
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarTransferenciaCommandHandler(
            IContaRepository contaRepository,
            ITransacaoRepository transacaoRepository,
            IUnitOfWork unitOfWork)
        {
            _contaRepository = contaRepository;
            _transacaoRepository = transacaoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TransacaoResponseDTO> Handle(RealizarTransferenciaCommand command)
        {
            if (command.ContaOrigemId == command.ContaDestinoId)
            {
                throw new InvalidOperationException("Contas de origem e destino não podem ser as mesmas para uma transferência.");
            }
            if (command.Valor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(command.Valor), "O valor da transferência deve ser positivo.");
            }


            var contaOrigem = await _contaRepository.GetByIdAsync(command.ContaOrigemId);
            var contaDestino = await _contaRepository.GetByIdAsync(command.ContaDestinoId);

            if (contaOrigem == null)
            {
                throw new InvalidOperationException("Conta de origem não encontrada.");
            }
            if (contaDestino == null)
            {
                throw new InvalidOperationException("Conta de destino não encontrada.");
            }

            if (contaOrigem.Status != StatusConta.Ativa)
            {
                throw new InvalidOperationException("A conta de origem não está ativa.");
            }
            if (contaDestino.Status != StatusConta.Ativa)
            {
                throw new InvalidOperationException("A conta de destino não está ativa.");
            }

            contaOrigem.Debitar(command.Valor); 
            contaDestino.Creditar(command.Valor);

            var transacao = new Transacao(
                TipoTransacao.Transferencia,
                command.Valor,
                command.ContaOrigemId,
                command.ContaDestinoId,
                command.Descricao
            );

            await _transacaoRepository.AddAsync(transacao);
            _contaRepository.Update(contaOrigem);
            _contaRepository.Update(contaDestino);

            var saved = await _unitOfWork.Commit();

            if (!saved)
            {
                transacao.Rejeitar("Falha ao processar a transferência (erro de persistência).");
                await _unitOfWork.Commit();
                throw new InvalidOperationException("Não foi possível completar a transferência devido a um erro de persistência.");
            }

            transacao.Aprovar();
            _transacaoRepository.Update(transacao);
            await _unitOfWork.Commit(); 

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