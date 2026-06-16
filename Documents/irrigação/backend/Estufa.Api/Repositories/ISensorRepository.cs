using Estufa.Api.Models;

namespace Estufa.Api.Repositories
{
    public interface ISensorRepository : IRepository<Sensor>
    {
        Task<Sensor?> GetByNameAsync(string nome);
    }
}
