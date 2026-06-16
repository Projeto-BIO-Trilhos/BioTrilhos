using Estufa.Api.Models;
using Estufa.Api.Repositories;
using Estufa.Api.Services.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Estufa.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<Usuario> _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IRepository<Usuario> userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<Usuario> RegisterAsync(string nome, string email, string password)
        {
            // check existing
            var existing = (await _userRepo.FindAsync(u => u.Email == email)).FirstOrDefault();
            if (existing != null) throw new InvalidOperationException("Email já cadastrado.");

            var user = new Usuario
            {
                Nome = nome,
                Email = email,
                PasswordHash = PasswordHasher.Hash(password),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            return user;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = (await _userRepo.FindAsync(u => u.Email == email)).FirstOrDefault();
            if (user == null) return null;
            if (!PasswordHasher.Verify(password, user.PasswordHash)) return null;

            var jwt = _config.GetSection("Jwt");
            var key = jwt.GetValue<string>("Key") ?? throw new InvalidOperationException("JWT key not configured");
            var issuer = jwt.GetValue<string>("Issuer");
            var audience = jwt.GetValue<string>("Audience");
            var expires = jwt.GetValue<int?>("ExpireMinutes") ?? 120;

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("uid", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var keyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(keyBytes, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expires), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
