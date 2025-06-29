using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq; // Usado para mocks se houverem serviços que não são DB
using PagueVeloz.Challenge.Application.Commands.Cliente;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Handlers;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Enums;
using PagueVeloz.Challenge.Domain.Interfaces;
using PagueVeloz.Challenge.Infrastructure.Data;
using PagueVeloz.Challenge.Infrastructure.Repositories;
using Xunit;

namespace PagueVeloz.Challenge.Tests.IntegrationTests
{
    public class ClienteIntegrationTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CriarClienteCommandHandler _criarClienteHandler;
        private readonly Mock<ILogger<ClienteRepository>> _mockClienteRepoLogger;
        private readonly Mock<ILogger<UnitOfWork>> _mockUnitOfWorkLogger;

        public ClienteIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:") 
                .Options;

            _context = new AppDbContext(options);
            _context.Database.OpenConnection(); 
            _context.Database.Migrate(); 

            _mockClienteRepoLogger = new Mock<ILogger<ClienteRepository>>();
            _mockUnitOfWorkLogger = new Mock<ILogger<UnitOfWork>>();

            _clienteRepository = new ClienteRepository(_context, _mockClienteRepoLogger.Object);
            _unitOfWork = new UnitOfWork(_context, _mockUnitOfWorkLogger.Object);
            _criarClienteHandler = new CriarClienteCommandHandler(_clienteRepository, _unitOfWork);
        }

        [Fact]
        public async Task CriarCliente_DevePersistirCorretamenteNoBancoDeDados()
        {
            // Arrange
            var clienteDto = new CriarClienteDTO
            {
                Nome = "Cliente Integracao",
                Documento = "98765432100",
                Tipo = TipoCliente.PF
            };
            var command = new CriarClienteCommand(clienteDto);

            // Act
            var result = await _criarClienteHandler.Handle(command);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);

            var clienteNoDb = await _context.Clientes
                                            .Include(c => c.Conta)
                                            .FirstOrDefaultAsync(c => c.Id == result.Id);

            Assert.NotNull(clienteNoDb);
            Assert.Equal(clienteDto.Nome, clienteNoDb.Nome);
            Assert.Equal(clienteDto.Documento, clienteNoDb.Documento);
            Assert.Equal(clienteDto.Tipo, clienteNoDb.Tipo);
            Assert.NotNull(clienteNoDb.Conta);
            Assert.Equal(result.Conta.Id, clienteNoDb.Conta.Id);
            Assert.Equal(StatusConta.Ativa, clienteNoDb.Conta.Status);
            Assert.Equal(0, clienteNoDb.Conta.SaldoDisponivel);
        }

        [Fact]
        public async Task CriarCliente_ComDocumentoDuplicado_DeveLancarExcecao()
        {
            // Arrange
            var primeiroClienteDto = new CriarClienteDTO
            {
                Nome = "Primeiro Cliente",
                Documento = "11122233344",
                Tipo = TipoCliente.PF
            };
            var primeiroCommand = new CriarClienteCommand(primeiroClienteDto);
            await _criarClienteHandler.Handle(primeiroCommand);

            // Arrange
            var segundoClienteDto = new CriarClienteDTO
            {
                Nome = "Segundo Cliente",
                Documento = "11122233344", 
                Tipo = TipoCliente.PF
            };
            var segundoCommand = new CriarClienteCommand(segundoClienteDto);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _criarClienteHandler.Handle(segundoCommand));
            Assert.Equal("Já existe um cliente cadastrado com este documento.", exception.Message);
        }

        public void Dispose()
        {
            _context.Database.CloseConnection(); 
            _context.Dispose(); 
        }
    }
}