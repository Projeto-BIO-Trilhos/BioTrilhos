using Estufa.Api.Models;

namespace Estufa.Api.Services
{
    public interface ILeituraService
    {
        Task<IEnumerable<Leitura>> GetRecentAsync(int minutes);
        Task<Leitura> AddAsync(Leitura leitura);
    }
}
