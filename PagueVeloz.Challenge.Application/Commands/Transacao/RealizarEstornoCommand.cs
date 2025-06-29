using PagueVeloz.Challenge.Application.DTOs;

namespace PagueVeloz.Challenge.Application.Commands.Transacao
{
    public class RealizarEstornoCommand
    {
        public Guid TransacaoOriginalId { get; set; }
        public string Descricao { get; set; } = "Estorno de transação original";

        public RealizarEstornoCommand(RealizarEstornoDTO dto)
        {
            TransacaoOriginalId = dto.TransacaoOriginalId;
            Descricao = dto.Descricao;
        }
    }
}