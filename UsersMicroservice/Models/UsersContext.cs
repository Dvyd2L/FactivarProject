using Microsoft.EntityFrameworkCore;

namespace UsersMicroservice.Models;

public partial class UsersContext : DbContext
{
    public UsersContext()
    {
    }

    public UsersContext(DbContextOptions<UsersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Usuario>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC073D2FFAA5");

            _ = entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D105342BBD673E").IsUnique();

            _ = entity.HasIndex(e => e.EnlaceCambioPass, "idx_unico")
                .IsUnique()
                .HasFilter("([EnlaceCambioPass] IS NOT NULL)");

            _ = entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            _ = entity.Property(e => e.Email).HasMaxLength(100);
            _ = entity.Property(e => e.EnlaceCambioPass).HasMaxLength(50);
            _ = entity.Property(e => e.FechaEnvioEnlace).HasColumnType("datetime");
            _ = entity.Property(e => e.Password).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
