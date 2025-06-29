using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Interfaces;
using PagueVeloz.Challenge.Infrastructure.Data;
using PagueVeloz.Challenge.Infrastructure.Resilience;
using Polly;

namespace PagueVeloz.Challenge.Infrastructure.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ContaRepository> _logger;

        public ContaRepository(AppDbContext context)
        {
            _context = context;
        }

        public ContaRepository(AppDbContext context, ILogger<ContaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Conta conta)
        {
            await PolicyConfiguration.RetryPolicy(_logger)
                                     .ExecuteAsync(async () =>
                                     {
                                         await _context.Contas.AddAsync(conta);
                                     });
        }

        public async Task<Conta?> GetByIdAsync(Guid id)
        {
            return await PolicyConfiguration.RetryPolicy(_logger)
                                            .ExecuteAsync(async () =>
                                            {
                                                return await _context.Contas.FirstOrDefaultAsync(c => c.Id == id);
                                            });
        }

        public void Remove(Conta conta)
        {
            _context.Contas.Remove(conta);
        }

        public void Update(Conta conta)
        {
            _context.Contas.Update(conta);
        }
    }
}