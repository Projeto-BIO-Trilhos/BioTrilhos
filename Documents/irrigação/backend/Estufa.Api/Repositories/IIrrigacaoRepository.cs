using Estufa.Api.Models;

namespace Estufa.Api.Repositories
{
    public interface IIrrigacaoRepository : IRepository<Irrigacao>
    {
        Task<IEnumerable<Irrigacao>> GetHistoricoAsync(DateTime? from = null, DateTime? to = null);
    }
}
