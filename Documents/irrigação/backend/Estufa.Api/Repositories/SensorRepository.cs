using Estufa.Api.Data;
using Estufa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Estufa.Api.Repositories
{
    public class SensorRepository : Repository<Sensor>, ISensorRepository
    {
        public SensorRepository(EstufaDbContext context) : base(context)
        {
        }

        public async Task<Sensor?> GetByNameAsync(string nome)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Nome == nome);
        }
    }
}
