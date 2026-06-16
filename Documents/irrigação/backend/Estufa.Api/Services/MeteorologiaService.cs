using Estufa.Api.Models;
using Estufa.Api.Repositories;
using Microsoft.AspNetCore.SignalR;
using Estufa.Api.Hubs;

namespace Estufa.Api.Services
{
    public class MeteorologiaService : IMeteorologiaService
    {
        private readonly IMeteorologiaRepository _repo;
        private readonly IHubContext<EstufaHub> _hubContext;

        public MeteorologiaService(IMeteorologiaRepository repo, IHubContext<EstufaHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }

        public async Task<EventoMeteorologico> RegistrarEventoAsync(string tipo, string descricao, string dadosJson)
        {
            var ev = new EventoMeteorologico
            {
                Tipo = tipo,
                Descricao = descricao,
                Dados = dadosJson,
                DataHora = DateTime.UtcNow
            };
            var created = await _repo.CreateEventoAsync(ev);
            // broadcast to SignalR clients
            try
            {
                await _hubContext.Clients.All.SendAsync("NewMeteorologyEvent", created);
            }
            catch { /* swallow broadcast errors */ }
            return created;
        }

        public async Task<IEnumerable<EventoMeteorologico>> GetRecentosAsync(int days)
        {
            return await _repo.GetEventosRecentesAsync(days);
        }

        public async Task<EventoMeteorologico?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }
}
