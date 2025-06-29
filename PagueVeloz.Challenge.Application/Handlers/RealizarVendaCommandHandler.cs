using PagueVeloz.Challenge.Application.Commands.Transacao;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Enums;
using PagueVeloz.Challenge.Domain.Interfaces;

namespace PagueVeloz.Challenge.Application.Handlers
{
    public class RealizarVendaCommandHandler
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarVendaCommandHandler(
            IContaRepository contaRepository,
            ITransacaoRepository transacaoRepository,
            IUnitOfWork unitOfWork)
        {
            _contaRepository = contaRepository;
            _transacaoRepository = transacaoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TransacaoResponseDTO> Handle(RealizarVendaCommand command)
        {
            var contaOrigem = await _contaRepository.GetByIdAsync(command.ContaOrigemId);

            if (contaOrigem == null)
            {
                throw new InvalidOperationException("Conta de origem não encontrada.");
            }

            if (contaOrigem.Status != StatusConta.Ativa)
            {
                throw new InvalidOperationException("A conta de origem não está ativa.");
            }

            var transacao = new Transacao(command.TipoVenda, command.Valor, command.ContaOrigemId, command.Descricao);

            switch (command.TipoVenda)
            {
                case TipoTransacao.VendaDebito:
                    contaOrigem.Debitar(command.Valor);
                    break;
                case TipoTransacao.VendaCreditoAVista:
                    contaOrigem.Creditar(command.Valor);
                    break;
                case TipoTransacao.VendaCreditoParcelado:
                    contaOrigem.Creditar(command.Valor, ehFuturo: true);
                    break;
                default:
                    throw new ArgumentException("Tipo de venda inválido.", nameof(command.TipoVenda));
            }

            await _transacaoRepository.AddAsync(transacao);

            _contaRepository.Update(contaOrigem);

            var saved = await _unitOfWork.Commit();

            if (!saved)
            {
                transacao.Rejeitar("Falha ao processar a venda (erro de persistência).");
                await _unitOfWork.Commit();
                throw new InvalidOperationException("Não foi possível completar a venda devido a um erro de persistência.");
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