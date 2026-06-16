using Estufa.Api.Models;
using Estufa.Api.Repositories;

namespace Estufa.Api.Services
{
    public class LeituraService : ILeituraService
    {
        private readonly ILeituraRepository _leituraRepo;

        public LeituraService(ILeituraRepository leituraRepo)
        {
            _leituraRepo = leituraRepo;
        }

        public async Task<Leitura> AddAsync(Leitura leitura)
        {
            await _leituraRepo.AddAsync(leitura);
            await _leituraRepo.SaveChangesAsync();
            return leitura;
        }

        public async Task<IEnumerable<Leitura>> GetRecentAsync(int minutes)
        {
            return await _leituraRepo.GetRecentAsync(minutes);
        }
    }
}
