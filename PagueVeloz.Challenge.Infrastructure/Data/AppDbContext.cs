using Microsoft.EntityFrameworkCore;
using PagueVeloz.Challenge.Domain.Entities;

namespace PagueVeloz.Challenge.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(builder =>
            {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Nome).IsRequired().HasMaxLength(250);
                builder.Property(c => c.Documento).IsRequired().HasMaxLength(18); 
                builder.HasIndex(c => c.Documento).IsUnique(); 

                builder.HasOne(c => c.Conta)
                       .WithOne()
                       .HasForeignKey<Cliente>(c => c.ContaId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Cascade); 

                builder.Property(c => c.Tipo)
                       .HasConversion<string>();
            });

            modelBuilder.Entity<Conta>(builder =>
            {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Tipo).IsRequired().HasConversion<string>(); 
                builder.Property(c => c.SaldoDisponivel).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(c => c.SaldoBloqueado).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(c => c.SaldoFuturo).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(c => c.Status).IsRequired().HasConversion<string>(); 
                builder.Property(c => c.DataAbertura).IsRequired();
            });

            modelBuilder.Entity<Transacao>(builder =>
            {
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Tipo).IsRequired().HasConversion<string>(); 
                builder.Property(t => t.Valor).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(t => t.DataHora).IsRequired();
                builder.Property(t => t.Status).IsRequired().HasConversion<string>(); 
                builder.Property(t => t.Descricao).HasMaxLength(500);

                builder.HasOne(t => t.ContaOrigem)
                       .WithMany()
                       .HasForeignKey(t => t.ContaOrigemId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Restrict); 

                builder.HasOne(t => t.ContaDestino)
                       .WithMany()
                       .HasForeignKey(t => t.ContaDestinoId)
                       .IsRequired(false) 
                       .OnDelete(DeleteBehavior.Restrict);
            });

            // Adicionar dados iniciais (Seed Data) - Opcional para o desafio, mas útil para testes
            // Exemplo de como você faria se quisesse dados iniciais no DB:
            // Observação: Para seed data de entidades com GUIDs ou que dependem umas das outras,
            // é mais complexo e requer IDs fixos ou uma lógica de seed mais avançada (ex: um seeder service).
            // Para este desafio, a criação via endpoint de Depósito Inicial é mais prática para testes.
            /*
            Guid clientePFId = Guid.Parse("A0A0A0A0-A0A0-A0A0-A0A0-000000000001");
            Guid contaPFId = Guid.Parse("B0B0B0B0-B0B0-B0B0-B0B0-000000000001");

            modelBuilder.Entity<Conta>().HasData(
                new Conta(TipoConta.Corrente)
                {
                    Id = contaPFId,
                    SaldoDisponivel = 1000.00m, // Saldo inicial para testes!
                    SaldoBloqueado = 0.00m,
                    SaldoFuturo = 0.00m,
                    Status = StatusConta.Ativa,
                    DataAbertura = DateTime.UtcNow.AddDays(-30),
                    DataEncerramento = null
                }
            );

            modelBuilder.Entity<Cliente>().HasData(
                new Cliente("Cliente Exemplo PF", "111.111.111-11", TipoCliente.PF)
                {
                    Id = clientePFId,
                    ContaId = contaPFId // Vincula ao ID da conta
                    // Note: A propriedade de navegação 'Conta' não é setada aqui, EF Core lida com isso.
                }
            );
            */
        }
    }
}