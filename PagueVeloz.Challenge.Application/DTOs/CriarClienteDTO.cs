using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Application.DTOs
{
    public class CriarClienteDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public TipoCliente Tipo { get; set; }
        public TipoConta TipoConta { get; set; }
    }
}