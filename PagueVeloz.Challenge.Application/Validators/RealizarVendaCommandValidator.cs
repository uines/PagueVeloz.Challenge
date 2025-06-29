using FluentValidation;
using PagueVeloz.Challenge.Application.Commands.Transacao;
using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Application.Validators
{
    public class RealizarVendaCommandValidator : AbstractValidator<RealizarVendaCommand>
    {
        public RealizarVendaCommandValidator()
        {
            RuleFor(x => x.ContaOrigemId)
                .NotEmpty().WithMessage("O ID da conta de origem é obrigatório.");

            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor da venda deve ser maior que zero.");

            RuleFor(x => x.TipoVenda)
                .IsInEnum().WithMessage("O tipo de venda é inválido.")
                .Must(tipo => tipo == TipoTransacao.VendaDebito ||
                              tipo == TipoTransacao.VendaCreditoAVista ||
                              tipo == TipoTransacao.VendaCreditoParcelado)
                .WithMessage("O tipo de transação especificado não é um tipo de venda válido.");
        }
    }
}