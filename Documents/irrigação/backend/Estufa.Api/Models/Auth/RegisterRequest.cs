using System.ComponentModel.DataAnnotations;

namespace Estufa.Api.Models.Auth
{
    public class RegisterRequest
    {
        [Required]
        public string Nome { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
