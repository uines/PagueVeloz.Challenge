using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagueVeloz.Challenge.Domain.Entities;
using PagueVeloz.Challenge.Domain.Interfaces;
using PagueVeloz.Challenge.Infrastructure.Data;
using PagueVeloz.Challenge.Infrastructure.Resilience;

namespace PagueVeloz.Challenge.Infrastructure.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TransacaoRepository> _logger; 

        public TransacaoRepository(AppDbContext context, ILogger<TransacaoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Transacao transacao)
        {
            await PolicyConfiguration.RetryPolicy(_logger)
                                     .ExecuteAsync(async () =>
                                     {
                                         await _context.Transacoes.AddAsync(transacao);
                                     });
        }

        public async Task<Transacao?> GetByIdAsync(Guid id)
        {
            return await PolicyConfiguration.RetryPolicy(_logger)
                                            .ExecuteAsync(async () =>
                                            {
                                                return await _context.Transacoes
                                                                     .Include(t => t.ContaOrigem)
                                                                     .Include(t => t.ContaDestino)
                                                                     .FirstOrDefaultAsync(t => t.Id == id);
                                            });
        }

        public async Task<IEnumerable<Transacao>> GetByContaIdAsync(Guid contaId, DateTime? dataInicio = null, DateTime? dataFim = null, int skip = 0, int take = 20)
        {
            return await PolicyConfiguration.RetryPolicy(_logger)
                                            .ExecuteAsync(async () =>
                                            {
                                                var query = _context.Transacoes
                                                                    .Where(t => t.ContaOrigemId == contaId || t.ContaDestinoId == contaId)
                                                                    .OrderByDescending(t => t.DataHora)
                                                                    .AsQueryable();

                                                if (dataInicio.HasValue)
                                                {
                                                    query = query.Where(t => t.DataHora >= dataInicio.Value);
                                                }

                                                if (dataFim.HasValue)
                                                {
                                                    query = query.Where(t => t.DataHora <= dataFim.Value);
                                                }

                                                query = query.Skip(skip).Take(take);

                                                return await query.ToListAsync();
                                            });
        }

        public void Update(Transacao transacao)
        {
            _context.Transacoes.Update(transacao);
        }
    }
}