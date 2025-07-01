using FluentValidation;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Application.Validators
{
    public class CriarClienteDTOValidator : AbstractValidator<CriarClienteDTO> 
    {
        public CriarClienteDTOValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(250).WithMessage("O nome não pode exceder 250 caracteres.");

            RuleFor(x => x.Documento)
                .Must(BeValidCpf).When(x => x.Tipo == TipoCliente.PF)
                .WithMessage("O CPF é inválido.");

            RuleFor(x => x.Documento)
                .Must(BeValidCnpj).When(x => x.Tipo == TipoCliente.PJ)
                .WithMessage("O CNPJ é inválido.");

            RuleFor(x => x.Tipo)
                .IsInEnum().WithMessage("O tipo de cliente é inválido.");

            RuleFor(x => x.TipoConta)
               .IsInEnum().WithMessage("O tipo de conta é inválido.");
        }

        private bool BeValidCpf(string documento)
        {
            var cleanDocument = new string(documento.Where(char.IsDigit).ToArray());
            return IsCpfValid(cleanDocument);
        }

        private bool BeValidCnpj(string documento)
        {
            var cleanDocument = new string(documento.Where(char.IsDigit).ToArray());
            return IsCnpjValid(cleanDocument);
        }

        private string CleanDocument(string documento)
        {
            return new string(documento.Where(char.IsDigit).ToArray());
        }

        private bool IsCpfValid(string documento)
        {
            var cpf = CleanDocument(documento);

            if (cpf.Length != 11) return false;
            if (cpf.Distinct().Count() == 1) return false; 

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        private bool IsCnpjValid(string documento)
        {
            var cnpj = CleanDocument(documento);

            if (cnpj.Length != 14) return false;
            if (cnpj.Distinct().Count() == 1) return false; 

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCnpj;
            string digito;
            int soma;
            int resto;

            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }
    }
}