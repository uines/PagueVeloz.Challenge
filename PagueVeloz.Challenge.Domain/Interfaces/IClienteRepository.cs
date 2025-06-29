using PagueVeloz.Challenge.Domain.Entities;

namespace PagueVeloz.Challenge.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task AddAsync(Cliente cliente);
        Task<Cliente?> GetByIdAsync(Guid id);
        Task<Cliente?> GetByDocumentoAsync(string documento);
        void Update(Cliente cliente);
        void Remove(Cliente cliente);
    }
}