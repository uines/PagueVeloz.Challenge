using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Enums;
using Xunit;

namespace PagueVeloz.Challenge.Tests.UnitTests
{
    public class ContaTests
    {
        [Fact] 
        public void Conta_DeveSerCriadaComSaldoZeroEStatusAtiva()
        {
            // Arrange
            var tipoConta = TipoConta.Corrente;

            // Act
            var conta = new Conta(tipoConta);

            // Assert
            Assert.NotEqual(Guid.Empty, conta.Id);
            Assert.Equal(tipoConta, conta.Tipo);
            Assert.Equal(0, conta.SaldoDisponivel);
            Assert.Equal(0, conta.SaldoBloqueado);
            Assert.Equal(0, conta.SaldoFuturo);
            Assert.Equal(StatusConta.Ativa, conta.Status);
            Assert.NotNull(conta.DataAbertura);
            Assert.Null(conta.DataEncerramento);
        }

        [Theory] 
        [InlineData(100, 100, 0, 0)] 
        [InlineData(50.50, 50.50, 0, 0)]
        public void Creditar_DeveAumentarSaldoDisponivel(decimal valorCredito, decimal esperadoDisponivel, decimal esperadoBloqueado, decimal esperadoFuturo)
        {
            // Arrange
            var conta = new Conta(TipoConta.Corrente);

            // Act
            conta.Creditar(valorCredito);

            // Assert
            Assert.Equal(esperadoDisponivel, conta.SaldoDisponivel);
            Assert.Equal(esperadoBloqueado, conta.SaldoBloqueado);
            Assert.Equal(esperadoFuturo, conta.SaldoFuturo);
        }

        [Fact]
        public void Creditar_ComValorNegativo_DeveLancarExcecao()
        {
            // Arrange
            var conta = new Conta(TipoConta.Corrente);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => conta.Creditar(-10));
        }

        [Fact]
        public void Debitar_DeveDiminuirSaldoDisponivel()
        {
            // Arrange
            var conta = new Conta(TipoConta.Corrente);
            conta.Creditar(200); 

            // Act
            conta.Debitar(50);

            // Assert
            Assert.Equal(150, conta.SaldoDisponivel);
        }

        [Fact]
        public void Debitar_ComSaldoInsuficiente_DeveLancarExcecao()
        {
            // Arrange
            var conta = new Conta(TipoConta.Corrente);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => conta.Debitar(100));
        }

        [Fact]
        public void Inativar_ContaAtivaSemSaldo_DeveInativar()
        {
            // Arrange
            var conta = new Conta(TipoConta.Corrente);

            // Act
            conta.Inativar();

            // Assert
            Assert.Equal(StatusConta.Inativa, conta.Status);
            Assert.NotNull(conta.DataEncerramento);
        }

        [Fact]
        public void Inativar_ContaAtivaComSaldo_DeveLancarExcecao()
        {
            // Arrange
            var conta = new Conta(TipoConta.Corrente);
            conta.Creditar(100);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => conta.Inativar());
        }

        [Fact]
        public void DebitarFuturo_DeveDiminuirSaldoFuturo()
        {
            // Arrange
            var conta = new Conta(TipoConta.Corrente);
            conta.Creditar(300, ehFuturo: true); 

            // Act
            conta.DebitarFuturo(100);

            // Assert
            Assert.Equal(200, conta.SaldoFuturo);
        }

        [Fact]
        public void DebitarFuturo_ComSaldoInsuficiente_DeveLancarExcecao()
        {
            // Arrange
            var conta = new Conta(TipoConta.Corrente); 

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => conta.DebitarFuturo(100));
        }
    }
}