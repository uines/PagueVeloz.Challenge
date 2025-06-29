using PagueVeloz.Challenge.Application.Commands.Cliente;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Interfaces;

namespace PagueVeloz.Challenge.Application.Handlers
{
    public class CriarClienteCommandHandler
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarClienteCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClienteResponseDTO> Handle(CriarClienteCommand command)
        {
            var clienteExistente = await _clienteRepository.GetByDocumentoAsync(command.Documento);
            if (clienteExistente != null)
            {
                throw new InvalidOperationException("Já existe um cliente cadastrado com este documento.");
            }

            var novoCliente = new Cliente(command.Nome, command.Documento, command.Tipo);
            await _clienteRepository.AddAsync(novoCliente);

            var saved = await _unitOfWork.Commit();
            if (!saved)
            {
                throw new InvalidOperationException("Erro ao salvar o novo cliente e sua conta.");
            }

            return new ClienteResponseDTO
            {
                Id = novoCliente.Id,
                Nome = novoCliente.Nome,
                Documento = novoCliente.Documento,
                Tipo = novoCliente.Tipo,
                Conta = new ContaResponseDTO
                {
                    Id = novoCliente.Conta.Id,
                    Tipo = novoCliente.Conta.Tipo,
                    SaldoDisponivel = novoCliente.Conta.SaldoDisponivel,
                    SaldoBloqueado = novoCliente.Conta.SaldoBloqueado,
                    SaldoFuturo = novoCliente.Conta.SaldoFuturo,
                    Status = novoCliente.Conta.Status,
                    DataAbertura = novoCliente.Conta.DataAbertura,
                    DataEncerramento = novoCliente.Conta.DataEncerramento
                }
            };
        }
    }
}