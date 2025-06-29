using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Domain.Enums; 

namespace PagueVeloz.Challenge.Application.Commands.Cliente
{
    public class CriarClienteCommand
    {
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public TipoCliente Tipo { get; set; }

        public CriarClienteCommand(CriarClienteDTO dto)
        {
            Nome = dto.Nome;
            Documento = dto.Documento;
            Tipo = dto.Tipo;
        }
    }
}