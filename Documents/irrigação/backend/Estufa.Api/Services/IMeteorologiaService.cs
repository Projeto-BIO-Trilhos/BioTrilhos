using Estufa.Api.Models;

namespace Estufa.Api.Services
{
    public interface IMeteorologiaService
    {
        Task<EventoMeteorologico> RegistrarEventoAsync(string tipo, string descricao, string dadosJson);
        Task<IEnumerable<EventoMeteorologico>> GetRecentosAsync(int days);
        Task<EventoMeteorologico?> GetByIdAsync(int id);
    }
}
