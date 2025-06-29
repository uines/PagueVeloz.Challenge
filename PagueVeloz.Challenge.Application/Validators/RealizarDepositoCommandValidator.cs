using FluentValidation;
using PagueVeloz.Challenge.Application.Commands.Transacao;

namespace PagueVeloz.Challenge.Application.Validators
{
    public class RealizarDepositoCommandValidator : AbstractValidator<RealizarDepositoCommand>
    {
        public RealizarDepositoCommandValidator()
        {
            RuleFor(x => x.ContaDestinoId)
                .NotEmpty().WithMessage("O ID da conta de destino é obrigatório.");

            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor do depósito deve ser maior que zero.");
        }
    }
}