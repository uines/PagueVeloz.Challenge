using FluentValidation;
using PagueVeloz.Challenge.Application.Commands.Transacao;

namespace PagueVeloz.Challenge.Application.Validators
{
    public class RealizarTransferenciaCommandValidator : AbstractValidator<RealizarTransferenciaCommand>
    {
        public RealizarTransferenciaCommandValidator()
        {
            RuleFor(x => x.ContaOrigemId)
                .NotEmpty().WithMessage("O ID da conta de origem é obrigatório.");

            RuleFor(x => x.ContaDestinoId)
                .NotEmpty().WithMessage("O ID da conta de destino é obrigatório.")
                .NotEqual(x => x.ContaOrigemId).WithMessage("A conta de destino não pode ser igual à conta de origem.");

            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor da transferência deve ser maior que zero.");
        }
    }
}