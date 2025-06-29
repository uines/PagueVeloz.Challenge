namespace PagueVeloz.Challenge.Application.DTOs
{
    public class RealizarEstornoDTO
    {
        public Guid TransacaoOriginalId { get; set; }
        public string Descricao { get; set; } = "Estorno de transação";
    }
}