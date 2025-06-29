using PagueVeloz.Challenge.Application.DTOs;

namespace PagueVeloz.Challenge.Application.Commands.Transacao
{
    public class RealizarDepositoCommand
    {
        public Guid ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = "Primeiro depósito";

        public RealizarDepositoCommand(RealizarDepositoDTO dto)
        {
            ContaDestinoId = dto.ContaDestinoId;
            Valor = dto.Valor;
            Descricao = dto.Descricao;
        }
    }
}