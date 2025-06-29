using FluentValidation;
using PagueVeloz.Challenge.Application.Commands.Transacao;

namespace PagueVeloz.Challenge.Application.Validators
{
    public class RealizarEstornoCommandValidator : AbstractValidator<RealizarEstornoCommand>
    {
        public RealizarEstornoCommandValidator()
        {
            RuleFor(x => x.TransacaoOriginalId)
                .NotEmpty().WithMessage("O ID da transação original é obrigatório.");
        }
    }
}