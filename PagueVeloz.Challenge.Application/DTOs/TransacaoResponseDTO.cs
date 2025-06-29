using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Application.DTOs
{
    public class TransacaoResponseDTO
    {
        public Guid Id { get; set; }
        public TipoTransacao Tipo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }
        public StatusTransacao Status { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public Guid ContaOrigemId { get; set; }
        public Guid? ContaDestinoId { get; set; }
    }
}