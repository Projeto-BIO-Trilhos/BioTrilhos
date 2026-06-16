using Estufa.Api.Data;
using Estufa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Estufa.Api.Repositories
{
    public class LeituraRepository : Repository<Leitura>, ILeituraRepository
    {
        public LeituraRepository(EstufaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Leitura>> GetRecentAsync(int minutes)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-minutes);
            return await _dbSet.Where(l => l.DataHora >= cutoff).ToListAsync();
        }
    }
}
