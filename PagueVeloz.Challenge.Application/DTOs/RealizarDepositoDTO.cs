namespace PagueVeloz.Challenge.Application.DTOs
{
    public class RealizarDepositoDTO
    {
        public Guid ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = "Primeiro depósito";
    }
}