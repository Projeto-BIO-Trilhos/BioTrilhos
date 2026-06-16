using Estufa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Estufa.Api.Data
{
    public class EstufaDbContext : DbContext
    {
        public EstufaDbContext(DbContextOptions<EstufaDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Sensor> Sensores { get; set; } = null!;
        public DbSet<Leitura> Leituras { get; set; } = null!;
        public DbSet<Irrigacao> Irrigacoes { get; set; } = null!;
        public DbSet<Reservatorio> Reservatorios { get; set; } = null!;
        public DbSet<EventoMeteorologico> EventosMeteorologicos { get; set; } = null!;
        public DbSet<Configuracao> Configuracoes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Sensor>()
                .HasMany(s => s.Leituras)
                .WithOne(l => l.Sensor!)
                .HasForeignKey(l => l.SensorId);
        }
    }
}
