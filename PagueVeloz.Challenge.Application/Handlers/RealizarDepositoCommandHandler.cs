using PagueVeloz.Challenge.Application.Commands.Transacao;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Enums;
using PagueVeloz.Challenge.Domain.Interfaces;

namespace PagueVeloz.Challenge.Application.Handlers
{
    public class RealizarDepositoCommandHandler
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarDepositoCommandHandler(
            IContaRepository contaRepository,
            ITransacaoRepository transacaoRepository,
            IUnitOfWork unitOfWork)
        {
            _contaRepository = contaRepository;
            _transacaoRepository = transacaoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TransacaoResponseDTO> Handle(RealizarDepositoCommand command)
        {
            if (command.Valor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(command.Valor), "O valor do depósito deve ser positivo.");
            }

            var contaDestino = await _contaRepository.GetByIdAsync(command.ContaDestinoId);

            if (contaDestino == null)
            {
                throw new InvalidOperationException("Conta de destino não encontrada para o depósito.");
            }

            if (contaDestino.Status != StatusConta.Ativa)
            {
                throw new InvalidOperationException("A conta de destino não está ativa para receber depósito.");
            }

            contaDestino.Creditar(command.Valor);

            var transacao = new Transacao(
                TipoTransacao.Deposito,
                command.Valor,
                contaDestino.Id,
                command.Descricao
            );

            await _transacaoRepository.AddAsync(transacao);
            _contaRepository.Update(contaDestino);

            var saved = await _unitOfWork.Commit();

            if (!saved)
            {
                transacao.Rejeitar("Falha ao processar o depósito (erro de persistência).");
                await _unitOfWork.Commit();
                throw new InvalidOperationException("Não foi possível completar o depósito devido a um erro de persistência.");
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