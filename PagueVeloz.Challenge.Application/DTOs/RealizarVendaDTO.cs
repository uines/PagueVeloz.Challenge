using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Application.DTOs
{
    public class RealizarVendaDTO
    {
        public Guid ContaOrigemId { get; set; }
        public decimal Valor { get; set; }
        public TipoTransacao TipoVenda { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}