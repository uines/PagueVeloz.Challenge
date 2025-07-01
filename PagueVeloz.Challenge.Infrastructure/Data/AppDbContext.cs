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
        }
    }
}