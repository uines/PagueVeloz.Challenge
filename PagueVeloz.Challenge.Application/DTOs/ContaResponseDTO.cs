using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Application.DTOs
{
    public class ContaResponseDTO
    {
        public Guid Id { get; set; }
        public TipoConta Tipo { get; set; }
        public decimal SaldoDisponivel { get; set; }
        public decimal SaldoBloqueado { get; set; }
        public decimal SaldoFuturo { get; set; }
        public StatusConta Status { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataEncerramento { get; set; }
    }
}