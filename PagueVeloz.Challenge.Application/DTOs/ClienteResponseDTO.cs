using PagueVeloz.Challenge.Domain.Enums;

namespace PagueVeloz.Challenge.Application.DTOs
{
    public class ClienteResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public TipoCliente Tipo { get; set; }
        public ContaResponseDTO Conta { get; set; } = new ContaResponseDTO();
    }
}