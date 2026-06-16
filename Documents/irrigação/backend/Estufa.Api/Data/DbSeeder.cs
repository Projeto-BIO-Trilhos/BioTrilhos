using Estufa.Api.Models;
using Estufa.Api.Services.Helpers;

namespace Estufa.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(EstufaDbContext context)
        {
            // Ensure database created
            await context.Database.EnsureCreatedAsync();

            // Seed admin user
            if (!context.Usuarios.Any())
            {
                var admin = new Usuario
                {
                    Nome = "Administrador",
                    Email = "admin@local",
                    PasswordHash = PasswordHasher.Hash("Senha@123"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };
                context.Usuarios.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
