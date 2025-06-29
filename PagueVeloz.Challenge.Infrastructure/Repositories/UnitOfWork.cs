using Microsoft.Extensions.Logging;
using PagueVeloz.Challenge.Domain.Interfaces;
using PagueVeloz.Challenge.Infrastructure.Data;
using PagueVeloz.Challenge.Infrastructure.Resilience;

namespace PagueVeloz.Challenge.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UnitOfWork> _logger; 

        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Commit()
        {
            return await PolicyConfiguration.RetryPolicy(_logger)
                                            .ExecuteAsync(async () =>
                                            {
                                                return await _context.SaveChangesAsync() > 0;
                                            });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}