using Estufa.Api.Models;

namespace Estufa.Api.Repositories
{
    public interface IMeteorologiaRepository
    {
        Task<EventoMeteorologico> CreateEventoAsync(EventoMeteorologico evento);
        Task<IEnumerable<EventoMeteorologico>> GetEventosRecentesAsync(int days);
        Task<EventoMeteorologico?> GetByIdAsync(int id);
    }
}
