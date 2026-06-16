using Estufa.Api.Models;

namespace Estufa.Api.Repositories
{
    public interface IReservatorioRepository : IRepository<Reservatorio>
    {
        Task<Reservatorio?> GetPrincipalAsync();
    }
}
