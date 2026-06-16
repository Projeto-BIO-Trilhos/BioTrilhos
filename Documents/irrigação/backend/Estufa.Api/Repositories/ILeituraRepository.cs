using Estufa.Api.Models;

namespace Estufa.Api.Repositories
{
    public interface ILeituraRepository : IRepository<Leitura>
    {
        Task<IEnumerable<Leitura>> GetRecentAsync(int minutes);
    }
}
