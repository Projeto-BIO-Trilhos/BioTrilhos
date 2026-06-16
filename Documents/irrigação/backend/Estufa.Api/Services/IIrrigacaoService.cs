using Estufa.Api.Models;

namespace Estufa.Api.Services
{
    public interface IIrrigacaoService
    {
        Task<Irrigacao> AcionarAsync(Irrigacao irrigacao);
        Task<IEnumerable<Irrigacao>> HistoricoAsync(DateTime? from = null, DateTime? to = null);
    }
}
