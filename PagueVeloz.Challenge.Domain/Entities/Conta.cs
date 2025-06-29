using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Domain.Entities
{
    public class Conta
    {
        public Guid Id { get; private set; }
        public TipoConta Tipo { get; private set; }
        public decimal SaldoDisponivel { get; private set; }
        public decimal SaldoBloqueado { get; private set; }
        public decimal SaldoFuturo { get; private set; }
        public StatusConta Status { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public DateTime? DataEncerramento { get; private set; }

        private Conta() { }

        public Conta(TipoConta tipo)
        {
            Id = Guid.NewGuid();
            Tipo = tipo;
            SaldoDisponivel = 0;
            SaldoBloqueado = 0;
            SaldoFuturo = 0;
            Status = StatusConta.Ativa; 
            DataAbertura = DateTime.UtcNow;
            DataEncerramento = null;
        }

        public void Inativar()
        {
            if (Status == StatusConta.Inativa)
                throw new InvalidOperationException("A conta já está inativa.");

            if (SaldoDisponivel != 0 || SaldoBloqueado != 0 || SaldoFuturo != 0)
                throw new InvalidOperationException("Não é possível inativar uma conta com saldo.");

            Status = StatusConta.Inativa;
            DataEncerramento = DateTime.UtcNow;
        }

        public void Ativar()
        {
            if (Status == StatusConta.Ativa)
                throw new InvalidOperationException("A conta já está ativa.");

            Status = StatusConta.Ativa;
            DataEncerramento = null; 
        }

        public void Creditar(decimal valor, bool ehFuturo = false, bool ehBloqueado = false)
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser positivo.");
            if (Status == StatusConta.Inativa) throw new InvalidOperationException("Não é possível creditar em uma conta inativa.");

            if (ehFuturo)
            {
                SaldoFuturo += valor;
            }
            else if (ehBloqueado)
            {
                SaldoBloqueado += valor;
            }
            else
            {
                SaldoDisponivel += valor;
            }
        }

        public void Debitar(decimal valor)
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser positivo.");
            if (Status == StatusConta.Inativa) throw new InvalidOperationException("Não é possível debitar de uma conta inativa.");

            if (SaldoDisponivel < valor)
                throw new InvalidOperationException("Saldo disponível insuficiente.");

            SaldoDisponivel -= valor;
        }

        public void MoverSaldoBloqueadoParaDisponivel(decimal valor)
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser positivo.");
            if (SaldoBloqueado < valor)
                throw new InvalidOperationException("Saldo bloqueado insuficiente.");

            SaldoBloqueado -= valor;
            SaldoDisponivel += valor;
        }

        public void MoverSaldoFuturoParaDisponivel(decimal valor)
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser positivo.");
            if (SaldoFuturo < valor)
                throw new InvalidOperationException("Saldo futuro insuficiente.");

            SaldoFuturo -= valor;
            SaldoDisponivel += valor;
        }

        public void DebitarFuturo(decimal valor)
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser positivo.");
            if (Status == StatusConta.Inativa) throw new InvalidOperationException("Não é possível debitar de uma conta inativa.");

            if (SaldoFuturo < valor)
                throw new InvalidOperationException("Saldo futuro insuficiente.");

            SaldoFuturo -= valor;
        }
    }
}