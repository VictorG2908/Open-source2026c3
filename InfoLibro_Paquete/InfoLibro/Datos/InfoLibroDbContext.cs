using Microsoft.EntityFrameworkCore;
using InfoLibro.Modelos;

namespace InfoLibro.Datos;

public class InfoLibroDbContext : DbContext
{
    public InfoLibroDbContext(DbContextOptions<InfoLibroDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rol>(b =>
        {
            b.HasKey(x => x.IdRol);
            b.Property(x => x.NombreRol).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(b =>
        {
            b.HasKey(x => x.IdUsuario);
            b.Property(x => x.NombreUsuario).HasMaxLength(30);
            b.Property(x => x.NombreCompleto).HasMaxLength(100);
            b.Property(x => x.Correo).HasMaxLength(120);
            b.Property(x => x.ClaveHash).HasMaxLength(64);
            b.Property(x => x.Salt).HasMaxLength(50);

            b.HasOne<Rol>().WithMany().HasForeignKey(u => u.IdRol);
        });
    }
}
