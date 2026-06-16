using Estufa.Api.Models;

namespace Estufa.Api.Services
{
    public interface IReservatorioService
    {
        Task<Reservatorio?> GetPrincipalAsync();
        Task<Reservatorio> UpdateAsync(Reservatorio res);
    }
}
