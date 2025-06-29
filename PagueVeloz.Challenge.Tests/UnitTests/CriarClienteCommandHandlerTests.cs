using Moq;
using PagueVeloz.Challenge.Application.Commands.Cliente;
using PagueVeloz.Challenge.Application.DTOs;
using PagueVeloz.Challenge.Application.Handlers;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Enums;
using PagueVeloz.Challenge.Domain.Interfaces;
using Xunit;

namespace PagueVeloz.Challenge.Tests.UnitTests
{
    public class CriarClienteCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DeveCriarClienteComSucesso()
        {
            // Arrange
            var clienteDto = new CriarClienteDTO
            {
                Nome = "Teste Cliente",
                Documento = "12345678900",
                Tipo = TipoCliente.PF
            };
            var command = new CriarClienteCommand(clienteDto);
          
            var mockClienteRepository = new Mock<IClienteRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockClienteRepository.Setup(r => r.GetByDocumentoAsync(It.IsAny<string>()))
                                 .ReturnsAsync((Cliente?)null);

            mockUnitOfWork.Setup(uow => uow.Commit()).ReturnsAsync(true);

            var handler = new CriarClienteCommandHandler(mockClienteRepository.Object, mockUnitOfWork.Object);

            // Act
            var result = await handler.Handle(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clienteDto.Nome, result.Nome);
            Assert.Equal(clienteDto.Documento, result.Documento);
            Assert.Equal(clienteDto.Tipo, result.Tipo);
            Assert.NotNull(result.Conta);
            Assert.Equal(StatusConta.Ativa, result.Conta.Status);

            mockClienteRepository.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once); 
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoDocumentoJaExiste()
        {
            // Arrange
            var clienteDto = new CriarClienteDTO
            {
                Nome = "Teste Cliente",
                Documento = "12345678900",
                Tipo = TipoCliente.PF
            };
            var command = new CriarClienteCommand(clienteDto);

            var existingCliente = new Cliente("Existing", "12345678900", TipoCliente.PF);

            var mockClienteRepository = new Mock<IClienteRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockClienteRepository.Setup(r => r.GetByDocumentoAsync(It.IsAny<string>()))
                                 .ReturnsAsync(existingCliente);

            var handler = new CriarClienteCommandHandler(mockClienteRepository.Object, mockUnitOfWork.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
            Assert.Equal("Já existe um cliente cadastrado com este documento.", exception.Message);

            mockClienteRepository.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoCommitFalha()
        {
            // Arrange
            var clienteDto = new CriarClienteDTO
            {
                Nome = "Teste Cliente",
                Documento = "12345678900",
                Tipo = TipoCliente.PF
            };
            var command = new CriarClienteCommand(clienteDto);

            var mockClienteRepository = new Mock<IClienteRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockClienteRepository.Setup(r => r.GetByDocumentoAsync(It.IsAny<string>()))
                                 .ReturnsAsync((Cliente?)null);

            mockUnitOfWork.SetupSequence(uow => uow.Commit())
                          .ReturnsAsync(false) 
                          .ReturnsAsync(true); 

            var handler = new CriarClienteCommandHandler(mockClienteRepository.Object, mockUnitOfWork.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
            Assert.Equal("Erro ao salvar o novo cliente e sua conta.", exception.Message);

            mockClienteRepository.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Commit(), Times.AtLeastOnce);
        }
    }
}