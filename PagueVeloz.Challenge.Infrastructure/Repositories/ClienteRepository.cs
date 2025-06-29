using Microsoft.EntityFrameworkCore;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Interfaces;
using PagueVeloz.Challenge.Infrastructure.Data;
using PagueVeloz.Challenge.Infrastructure.Resilience;
using Microsoft.Extensions.Logging;

namespace PagueVeloz.Challenge.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(AppDbContext context, ILogger<ClienteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Cliente cliente)
        {
            await PolicyConfiguration.RetryPolicy(_logger)
                                     .ExecuteAsync(async () =>
                                     {
                                         await _context.Clientes.AddAsync(cliente);
                                     });
        }

        public async Task<Cliente?> GetByIdAsync(Guid id)
        {
            return await PolicyConfiguration.RetryPolicy(_logger)
                                            .ExecuteAsync(async () =>
                                            {
                                                return await _context.Clientes
                                                                     .Include(c => c.Conta)
                                                                     .FirstOrDefaultAsync(c => c.Id == id);
                                            });
        }

        public async Task<Cliente?> GetByDocumentoAsync(string documento)
        {
            return await PolicyConfiguration.RetryPolicy(_logger)
                                            .ExecuteAsync(async () =>
                                            {
                                                return await _context.Clientes
                                                                    .Include(c => c.Conta)
                                                                    .FirstOrDefaultAsync(c => c.Documento == documento);
                                            });
        }

        public void Remove(Cliente cliente)
        {
            _context.Clientes.Remove(cliente);
        }

        public void Update(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            _context.Contas.Update(cliente.Conta);
        }
    }
}