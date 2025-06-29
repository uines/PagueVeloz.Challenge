using PagueVeloz.Challenge.Domain.Entities;

namespace PagueVeloz.Challenge.Domain.Interfaces
{
    public interface ITransacaoRepository
    {
        Task AddAsync(Transacao transacao);
        Task<Transacao?> GetByIdAsync(Guid id);
        Task<IEnumerable<Transacao>> GetByContaIdAsync(Guid contaId, DateTime? dataInicio = null, DateTime? dataFim = null, int skip = 0, int take = 20);
        void Update(Transacao transacao);
    }
}