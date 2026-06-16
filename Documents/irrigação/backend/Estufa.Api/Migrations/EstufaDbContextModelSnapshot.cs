using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estufa.Api.Migrations
{
    [DbContext(typeof(Estufa.Api.Data.EstufaDbContext))]
    partial class EstufaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("Estufa.Api.Models.Sensor", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Nome").IsRequired();
                b.Property<string>("Tipo");
                b.Property<string>("Localizacao");
                b.Property<string>("Unidade");
                b.Property<bool>("Ativo");
                b.HasKey("Id");
                b.ToTable("Sensores");
            });

            modelBuilder.Entity("Estufa.Api.Models.Usuario", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Nome").IsRequired();
                b.Property<string>("Email").IsRequired();
                b.Property<string>("PasswordHash").IsRequired();
                b.Property<string>("Role");
                b.Property<DateTime>("CreatedAt");
                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.ToTable("Usuarios");
            });

            modelBuilder.Entity("Estufa.Api.Models.Reservatorio", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Nome");
                b.Property<double>("Nivel");
                b.Property<double>("CapacidadeLitros");
                b.Property<DateTime>("UltimaAtualizacao");
                b.HasKey("Id");
                b.ToTable("Reservatorios");
            });

            modelBuilder.Entity("Estufa.Api.Models.Irrigacao", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<DateTime>("DataHora");
                b.Property<TimeSpan?>("Duracao");
                b.Property<string>("Status");
                b.Property<string>("Observacoes");
                b.HasKey("Id");
                b.ToTable("Irrigacoes");
            });

            modelBuilder.Entity("Estufa.Api.Models.EventoMeteorologico", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Tipo");
                b.Property<string>("Descricao");
                b.Property<DateTime>("DataHora");
                b.Property<string>("Dados");
                b.HasKey("Id");
                b.ToTable("EventosMeteorologicos");
            });

            modelBuilder.Entity("Estufa.Api.Models.Configuracao", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Chave").IsRequired();
                b.Property<string>("Valor").IsRequired();
                b.Property<string>("Descricao");
                b.HasKey("Id");
                b.ToTable("Configuracoes");
            });

            modelBuilder.Entity("Estufa.Api.Models.Leitura", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("SensorId");
                b.Property<double?>("Temperatura");
                b.Property<int?>("UmidadeAr");
                b.Property<int?>("UmidadeSolo");
                b.Property<DateTime>("DataHora");
                b.HasKey("Id");
                b.HasIndex("SensorId");
                b.ToTable("Leituras");
            });

            modelBuilder.Entity("Estufa.Api.Models.Leitura", b =>
            {
                b.HasOne("Estufa.Api.Models.Sensor")
                    .WithMany("Leituras")
                    .HasForeignKey("SensorId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
