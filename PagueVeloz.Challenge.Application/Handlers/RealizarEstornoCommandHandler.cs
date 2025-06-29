using PagueVeloz.Challenge.Application.Commands.Transacao;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Enums;
using PagueVeloz.Challenge.Domain.Interfaces;

namespace PagueVeloz.Challenge.Application.Handlers
{
    public class RealizarEstornoCommandHandler
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IContaRepository _contaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarEstornoCommandHandler(
            ITransacaoRepository transacaoRepository,
            IContaRepository contaRepository,
            IUnitOfWork unitOfWork)
        {
            _transacaoRepository = transacaoRepository;
            _contaRepository = contaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TransacaoResponseDTO> Handle(RealizarEstornoCommand command)
        {
            var transacaoOriginal = await _transacaoRepository.GetByIdAsync(command.TransacaoOriginalId);

            if (transacaoOriginal == null)
            {
                throw new InvalidOperationException("Transação original não encontrada.");
            }

            if (transacaoOriginal.Status == StatusTransacao.Estornada || transacaoOriginal.Status == StatusTransacao.Cancelada)
            {
                throw new InvalidOperationException("Esta transação já foi estornada ou cancelada.");
            }
            if (transacaoOriginal.Status != StatusTransacao.Aprovada)
            {
                throw new InvalidOperationException("Somente transações aprovadas podem ser estornadas.");
            }
            if (transacaoOriginal.Tipo == TipoTransacao.Transferencia)
            {
                throw new InvalidOperationException("Não é possível estornar uma transferência diretamente.");
            }

            var conta = await _contaRepository.GetByIdAsync(transacaoOriginal.ContaOrigemId);
            if (conta == null)
            {
                throw new InvalidOperationException("Conta associada à transação original não encontrada.");
            }
            if (conta.Status != StatusConta.Ativa)
            {
                throw new InvalidOperationException("A conta associada à transação original não está ativa para realizar o estorno.");
            }

            var estornoValor = transacaoOriginal.Valor;

            if (transacaoOriginal.Tipo == TipoTransacao.VendaDebito)
            {
                conta.Creditar(estornoValor);
            }
            else if (transacaoOriginal.Tipo == TipoTransacao.VendaCreditoAVista)
            {
                if (conta.SaldoDisponivel < estornoValor)
                {
                    throw new InvalidOperationException("Saldo disponível insuficiente na conta para realizar o estorno da venda original.");
                }
                conta.Debitar(estornoValor);
            }
            else if (transacaoOriginal.Tipo == TipoTransacao.VendaCreditoParcelado)
            {

                if (conta.SaldoFuturo >= estornoValor)
                {
                    conta.DebitarFuturo(estornoValor); 
                }
                else if (conta.SaldoDisponivel >= estornoValor)
                {
                    conta.Debitar(estornoValor);
                }
                else
                {
                    throw new InvalidOperationException("Saldo insuficiente (disponível ou futuro) para estornar a venda parcelada.");
                }
            }
            else
            {
                throw new InvalidOperationException("Tipo de transação original não suportado para estorno direto.");
            }

            var estornoTransacao = new Transacao(
                TipoTransacao.Estorno,
                estornoValor,
                transacaoOriginal.ContaOrigemId,
                command.Descricao
            );

            transacaoOriginal.Cancelar($"Estornada pela transação {estornoTransacao.Id}");

            await _transacaoRepository.AddAsync(estornoTransacao);
            _transacaoRepository.Update(transacaoOriginal); 
            _contaRepository.Update(conta); 

            var saved = await _unitOfWork.Commit();

            if (!saved)
            {
                estornoTransacao.Rejeitar("Falha ao processar o estorno (erro de persistência).");
                await _unitOfWork.Commit(); 
                throw new InvalidOperationException("Não foi possível completar o estorno devido a um erro de persistência.");
            }

            estornoTransacao.Aprovar(); 
            _transacaoRepository.Update(estornoTransacao);
            await _unitOfWork.Commit(); 

            return new TransacaoResponseDTO
            {
                Id = estornoTransacao.Id,
                Tipo = estornoTransacao.Tipo,
                Valor = estornoTransacao.Valor,
                DataHora = estornoTransacao.DataHora,
                Status = estornoTransacao.Status,
                Descricao = estornoTransacao.Descricao,
                ContaOrigemId = estornoTransacao.ContaOrigemId,
                ContaDestinoId = estornoTransacao.ContaDestinoId
            };
        }
    }
}