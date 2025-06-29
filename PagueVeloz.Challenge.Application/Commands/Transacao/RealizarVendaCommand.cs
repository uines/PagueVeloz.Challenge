using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Application.Commands.Transacao
{
    public class RealizarVendaCommand
    {
        public Guid ContaOrigemId { get; set; }
        public decimal Valor { get; set; }
        public TipoTransacao TipoVenda { get; set; }
        public string Descricao { get; set; } = string.Empty;

        public RealizarVendaCommand(RealizarVendaDTO dto)
        {
            ContaOrigemId = dto.ContaOrigemId;
            Valor = dto.Valor;
            TipoVenda = dto.TipoVenda;
            Descricao = dto.Descricao;
        }
    }
}