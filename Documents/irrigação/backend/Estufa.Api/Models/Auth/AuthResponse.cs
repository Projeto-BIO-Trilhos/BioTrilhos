namespace Estufa.Api.Models.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string ExpiresIn { get; set; } = null!;
    }
}
