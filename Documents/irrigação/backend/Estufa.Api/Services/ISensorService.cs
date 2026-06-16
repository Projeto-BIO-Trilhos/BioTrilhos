using Estufa.Api.Models;

namespace Estufa.Api.Services
{
    public interface ISensorService
    {
        Task<IEnumerable<Sensor>> GetAllAsync();
        Task<Sensor?> GetByIdAsync(int id);
        Task<Sensor> CreateAsync(Sensor sensor);
        Task UpdateAsync(Sensor sensor);
        Task DeleteAsync(int id);
    }
}
