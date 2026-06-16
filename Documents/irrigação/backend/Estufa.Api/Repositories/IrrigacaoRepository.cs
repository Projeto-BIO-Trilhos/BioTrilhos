using Estufa.Api.Data;
using Estufa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Estufa.Api.Repositories
{
    public class IrrigacaoRepository : Repository<Irrigacao>, IIrrigacaoRepository
    {
        public IrrigacaoRepository(EstufaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Irrigacao>> GetHistoricoAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _dbSet.AsQueryable();
            if (from.HasValue) query = query.Where(i => i.DataHora >= from.Value);
            if (to.HasValue) query = query.Where(i => i.DataHora <= to.Value);
            return await query.OrderByDescending(i => i.DataHora).ToListAsync();
        }
    }
}
