using PagueVeloz.Challenge.Domain.Entities;

namespace PagueVeloz.Challenge.Domain.Interfaces
{
    public interface IContaRepository
    {
        Task AddAsync(Conta conta);
        Task<Conta?> GetByIdAsync(Guid id);
        void Update(Conta conta);
        void Remove(Conta conta);
    }
}