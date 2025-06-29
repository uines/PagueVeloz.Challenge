using PagueVeloz.Challenge.Application.DTOs;

namespace PagueVeloz.Challenge.Application.Commands.Transacao
{
    public class RealizarTransferenciaCommand
    {
        public Guid ContaOrigemId { get; set; }
        public Guid ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = string.Empty;

        public RealizarTransferenciaCommand(RealizarTransferenciaDTO dto)
        {
            ContaOrigemId = dto.ContaOrigemId;
            ContaDestinoId = dto.ContaDestinoId;
            Valor = dto.Valor;
            Descricao = dto.Descricao;
        }
    }
}