using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Domain.Entities
{
    public class Transacao
    {
        public Guid Id { get; private set; }
        public TipoTransacao Tipo { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataHora { get; private set; }
        public StatusTransacao Status { get; private set; }
        public string Descricao { get; private set; } = string.Empty;

        public Guid ContaOrigemId { get; private set; }
        public Conta ContaOrigem { get; private set; } 

        public Guid? ContaDestinoId { get; private set; }
        public Conta? ContaDestino { get; private set; }

        private Transacao() { }

        public Transacao(TipoTransacao tipo, decimal valor, Guid contaOrigemId, string descricao = "")
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor da transação deve ser positivo.");

            Id = Guid.NewGuid();
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.UtcNow;
            Status = StatusTransacao.Pendente;
            Descricao = descricao;
            ContaOrigemId = contaOrigemId;
            ContaDestinoId = null; 
        }
        public Transacao(TipoTransacao tipo, decimal valor, Guid contaOrigemId, Guid contaDestinoId, string descricao = "")
        {
            if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "O valor da transação deve ser positivo.");
            if (contaOrigemId == contaDestinoId) throw new InvalidOperationException("Conta de origem e destino não podem ser as mesmas.");

            Id = Guid.NewGuid();
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.UtcNow;
            Status = StatusTransacao.Pendente; 
            Descricao = descricao;
            ContaOrigemId = contaOrigemId;
            ContaDestinoId = contaDestinoId;
        }

        public void Aprovar()
        {
            if (Status != StatusTransacao.Pendente)
                throw new InvalidOperationException("A transação não está em status pendente para ser aprovada.");
            Status = StatusTransacao.Aprovada;
        }

        public void Rejeitar(string motivo)
        {
            if (Status != StatusTransacao.Pendente)
                throw new InvalidOperationException("A transação não está em status pendente para ser rejeitada.");
            Status = StatusTransacao.Rejeitada;
            Descricao += $" (Rejeitada: {motivo})";
        }

        public void Cancelar(string motivo)
        {
            if (Status == StatusTransacao.Aprovada || Status == StatusTransacao.Pendente)
            {
                Status = StatusTransacao.Cancelada;
                Descricao += $" (Cancelada: {motivo})";
            }
            else
            {
                throw new InvalidOperationException("A transação não pode ser cancelada neste status.");
            }
        }
    }
}