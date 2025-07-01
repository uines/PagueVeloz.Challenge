using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Queries.Cliente;
using PagueVeloz.Challenge.Domain.Interfaces;
namespace PagueVeloz.Challenge.Application.Handlers
{
    public class GetClienteByIdQueryHandler
    {
        private readonly IClienteRepository _clienteRepository;

        public GetClienteByIdQueryHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ClienteResponseDTO?> Handle(GetClienteByIdQuery query)
        {
            var cliente = await _clienteRepository.GetByIdAsync(query.ClienteId);

            if (cliente == null)
            {
                return null;
            }

            return new ClienteResponseDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Documento = cliente.Documento,
                Tipo = cliente.Tipo,
                Conta = new ContaResponseDTO
                {
                    Id = cliente.Conta.Id,
                    Tipo = cliente.Conta.Tipo,
                    SaldoDisponivel = cliente.Conta.SaldoDisponivel,
                    SaldoBloqueado = cliente.Conta.SaldoBloqueado,
                    SaldoFuturo = cliente.Conta.SaldoFuturo,
                    Status = cliente.Conta.Status,
                    DataAbertura = cliente.Conta.DataAbertura,
                    DataEncerramento = cliente.Conta.DataEncerramento
                }
            };
        }
    }
}