using Estufa.Api.Data;
using Estufa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Estufa.Api.Repositories
{
    public class MeteorologiaRepository : IMeteorologiaRepository
    {
        private readonly EstufaDbContext _context;

        public MeteorologiaRepository(EstufaDbContext context)
        {
            _context = context;
        }

        public async Task<EventoMeteorologico> CreateEventoAsync(EventoMeteorologico evento)
        {
            _context.EventosMeteorologicos.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task<IEnumerable<EventoMeteorologico>> GetEventosRecentesAsync(int days)
        {
            var cutoff = DateTime.UtcNow.AddDays(-days);
            return await _context.EventosMeteorologicos.Where(e => e.DataHora >= cutoff).ToListAsync();
        }

        public async Task<EventoMeteorologico?> GetByIdAsync(int id)
        {
            return await _context.EventosMeteorologicos.FindAsync(id);
        }
    }
}
