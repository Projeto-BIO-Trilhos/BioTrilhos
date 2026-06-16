using Estufa.Api.Models;
using Estufa.Api.Repositories;

namespace Estufa.Api.Services
{
    public class IrrigacaoService : IIrrigacaoService
    {
        private readonly IIrrigacaoRepository _repo;

        public IrrigacaoService(IIrrigacaoRepository repo)
        {
            _repo = repo;
        }

        public async Task<Irrigacao> AcionarAsync(Irrigacao irrigacao)
        {
            irrigacao.DataHora = DateTime.UtcNow;
            await _repo.AddAsync(irrigacao);
            await _repo.SaveChangesAsync();
            return irrigacao;
        }

        public async Task<IEnumerable<Irrigacao>> HistoricoAsync(DateTime? from = null, DateTime? to = null)
        {
            return await _repo.GetHistoricoAsync(from, to);
        }
    }
}
