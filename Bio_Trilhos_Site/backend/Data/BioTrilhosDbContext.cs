using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Data;

/// <summary>
/// DbContext para Bio Trilhos - Estufa Inteligente.
/// Configuração do Entity Framework Core com PostgreSQL.
/// </summary>
public class BioTrilhosDbContext : DbContext
{
    public BioTrilhosDbContext(DbContextOptions<BioTrilhosDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Estufa> Estufas { get; set; } = null!;
    public DbSet<Sensor> Sensores { get; set; } = null!;
    public DbSet<LeituraSensor> LeiturasSensores { get; set; } = null!;
    public DbSet<Reservatorio> Reservatorios { get; set; } = null!;
    public DbSet<HistoricoReservatorio> HistoricosReservatorios { get; set; } = null!;
    public DbSet<Irrigacao> Irrigacoes { get; set; } = null!;
    public DbSet<ConfiguracoesAmbientais> ConfiguracoesAmbientais { get; set; } = null!;
    public DbSet<DadosMeteorologicos> DadosMeteorologicos { get; set; } = null!;
    public DbSet<Alerta> Alertas { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações para Estufa
        modelBuilder.Entity<Estufa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Localizacao).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
        });

        // Configurações para Sensor
        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Localizacao).HasMaxLength(255);
            entity.Property(e => e.IdentificadorDispositivo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UnidadeMedida).HasMaxLength(50);
            
            entity.HasOne(e => e.Estufa)
                .WithMany(e => e.Sensores)
                .HasForeignKey(e => e.EstufaId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.EstufaId);
            entity.HasIndex(e => e.IdentificadorDispositivo);
        });

        // Configurações para LeituraSensor
        modelBuilder.Entity<LeituraSensor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Valor).HasPrecision(10, 2);
            entity.Property(e => e.UnidadeMedida).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            
            entity.HasOne(e => e.Sensor)
                .WithMany(e => e.Leituras)
                .HasForeignKey(e => e.SensorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Índices para consultas frequentes
            entity.HasIndex(e => e.SensorId);
            entity.HasIndex(e => e.DataHoraLeitura);
            entity.HasIndex(e => new { e.SensorId, e.DataHoraLeitura });
        });

        // Configurações para Reservatorio
        modelBuilder.Entity<Reservatorio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CapacidadeMaxima).HasPrecision(12, 2);
            entity.Property(e => e.NivelAtual).HasPrecision(12, 2);
            entity.Property(e => e.PercentualAtual).HasPrecision(5, 2);
            entity.Property(e => e.Status).HasMaxLength(50);
            
            entity.HasOne(e => e.Estufa)
                .WithMany(e => e.Reservatorios)
                .HasForeignKey(e => e.EstufaId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.EstufaId);
        });

        // Configurações para HistoricoReservatorio
        modelBuilder.Entity<HistoricoReservatorio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NivelAnterior).HasPrecision(12, 2);
            entity.Property(e => e.NivelAtual).HasPrecision(12, 2);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            
            entity.HasOne(e => e.Reservatorio)
                .WithMany(e => e.Historicos)
                .HasForeignKey(e => e.ReservatorioId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.ReservatorioId);
            entity.HasIndex(e => e.DataHora);
        });

        // Configurações para Irrigacao
        modelBuilder.Entity<Irrigacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Motivo).HasMaxLength(500);
            
            entity.HasOne(e => e.Estufa)
                .WithMany(e => e.Irrigacoes)
                .HasForeignKey(e => e.EstufaId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Reservatorio)
                .WithMany(e => e.Irrigacoes)
                .HasForeignKey(e => e.ReservatorioId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(e => e.EstufaId);
            entity.HasIndex(e => e.DataHoraInicio);
        });

        // Configurações para ConfiguracoesAmbientais
        modelBuilder.Entity<ConfiguracoesAmbientais>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UmidadeSoloMinima).HasPrecision(5, 2);
            entity.Property(e => e.UmidadeSoloMaxima).HasPrecision(5, 2);
            entity.Property(e => e.TemperaturaMinima).HasPrecision(5, 2);
            entity.Property(e => e.TemperaturaMaxima).HasPrecision(5, 2);
            entity.Property(e => e.UmidadeArMinima).HasPrecision(5, 2);
            entity.Property(e => e.UmidadeArMaxima).HasPrecision(5, 2);
            entity.Property(e => e.NivelMinimoReservatorio).HasPrecision(5, 2);
            
            entity.HasOne(e => e.Estufa)
                .WithMany(e => e.ConfiguracoesAmbientais)
                .HasForeignKey(e => e.EstufaId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.EstufaId).IsUnique();
        });

        // Configurações para DadosMeteorologicos
        modelBuilder.Entity<DadosMeteorologicos>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TemperaturaExterna).HasPrecision(5, 2);
            entity.Property(e => e.UmidadeExterna).HasPrecision(5, 2);
            entity.Property(e => e.CondicaoClimatica).HasMaxLength(255);
            entity.Property(e => e.ProbabilidadeChuva).HasPrecision(5, 2);
            entity.Property(e => e.PrevisaoChuva).HasMaxLength(500);
            entity.Property(e => e.VelocidadeVento).HasPrecision(5, 2);
            entity.Property(e => e.FonteAPI).HasMaxLength(100);
            
            entity.HasOne(e => e.Estufa)
                .WithMany(e => e.DadosMeteorologicos)
                .HasForeignKey(e => e.EstufaId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.EstufaId);
            entity.HasIndex(e => e.DataHoraConsulta);
        });

        // Configurações para Alerta
        modelBuilder.Entity<Alerta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Mensagem).IsRequired().HasMaxLength(1000);
            
            entity.HasOne(e => e.Estufa)
                .WithMany(e => e.Alertas)
                .HasForeignKey(e => e.EstufaId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.EstufaId);
            entity.HasIndex(e => e.DataCriacao);
        });

        // Configurações para Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SenhaHash).IsRequired();
            
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
