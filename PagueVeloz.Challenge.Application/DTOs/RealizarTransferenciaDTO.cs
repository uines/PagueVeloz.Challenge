namespace PagueVeloz.Challenge.Application.DTOs
{
    public class RealizarTransferenciaDTO
    {
        public Guid ContaOrigemId { get; set; }
        public Guid ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}