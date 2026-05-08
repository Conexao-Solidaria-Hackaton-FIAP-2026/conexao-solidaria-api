using ConexaoSolidaria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConexaoSolidaria.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Campanha> Campanhas => Set<Campanha>();
    public DbSet<Doacao> Doacoes => Set<Doacao>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(x => x.Id);
            e.Property(x => x.NomeCompleto).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Cpf).HasMaxLength(11).IsRequired();
            e.Property(x => x.SenhaHash).IsRequired();
            e.Property(x => x.Role).HasConversion<int>();
        });

        b.Entity<Campanha>(e =>
        {
            e.ToTable("Campanhas");
            e.HasKey(x => x.Id);
            e.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
            e.Property(x => x.Descricao).HasMaxLength(2000).IsRequired();
            e.Property(x => x.MetaFinanceira).HasPrecision(18, 2);
            e.Property(x => x.ValorArrecadado).HasPrecision(18, 2);
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => x.Status);
        });

        b.Entity<Doacao>(e =>
        {
            e.ToTable("Doacoes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Valor).HasPrecision(18, 2);
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => x.CampanhaId);
            e.HasIndex(x => x.DoadorId);
        });
    }
}
