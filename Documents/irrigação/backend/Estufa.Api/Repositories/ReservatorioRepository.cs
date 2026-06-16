using Estufa.Api.Data;
using Estufa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Estufa.Api.Repositories
{
    public class ReservatorioRepository : Repository<Reservatorio>, IReservatorioRepository
    {
        public ReservatorioRepository(EstufaDbContext context) : base(context)
        {
        }

        public async Task<Reservatorio?> GetPrincipalAsync()
        {
            return await _dbSet.OrderByDescending(r => r.Id).FirstOrDefaultAsync();
        }
    }
}
