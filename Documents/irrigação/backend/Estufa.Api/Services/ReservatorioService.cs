using Estufa.Api.Models;
using Estufa.Api.Repositories;

namespace Estufa.Api.Services
{
    public class ReservatorioService : IReservatorioService
    {
        private readonly IReservatorioRepository _repo;

        public ReservatorioService(IReservatorioRepository repo)
        {
            _repo = repo;
        }

        public async Task<Reservatorio?> GetPrincipalAsync()
        {
            return await _repo.GetPrincipalAsync();
        }

        public async Task<Reservatorio> UpdateAsync(Reservatorio res)
        {
            _repo.Update(res);
            await _repo.SaveChangesAsync();
            return res;
        }
    }
}
