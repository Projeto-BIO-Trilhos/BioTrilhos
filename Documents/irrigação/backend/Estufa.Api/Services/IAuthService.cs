using Estufa.Api.Models;

namespace Estufa.Api.Services
{
    public interface IAuthService
    {
        Task<Usuario> RegisterAsync(string nome, string email, string password);
        Task<string?> LoginAsync(string email, string password);
    }
}
