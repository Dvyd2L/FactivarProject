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

    public virtual DbSet<Credenciale> Credenciales { get; set; }

    public virtual DbSet<DatosPersonale> DatosPersonales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Credenciale>(entity =>
        {
            _ = entity.HasKey(e => e.IdUsuario).HasName("PK__Credenci__5B65BF9724CEB331");

            _ = entity.HasIndex(e => e.EnlaceCambioPass, "idx_unico")
                .IsUnique()
                .HasFilter("([EnlaceCambioPass] IS NOT NULL)");

            _ = entity.Property(e => e.IdUsuario).ValueGeneratedNever();
            _ = entity.Property(e => e.EnlaceCambioPass).HasMaxLength(50);
            _ = entity.Property(e => e.FechaEnvioEnlace).HasColumnType("datetime");
            _ = entity.Property(e => e.Password).HasMaxLength(500);

            _ = entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Credenciale)
                .HasForeignKey<Credenciale>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Credencia__IdUsu__5EBF139D");
        });

        _ = modelBuilder.Entity<DatosPersonale>(entity =>
        {
            _ = entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC078AE81D96");

            _ = entity.HasIndex(e => e.Email, "UQ__tmp_ms_x__A9D105343CE9FB58").IsUnique();

            _ = entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            _ = entity.Property(e => e.Apellidos).HasMaxLength(100);
            _ = entity.Property(e => e.Email).HasMaxLength(100);
            _ = entity.Property(e => e.Nombre).HasMaxLength(25);
            _ = entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
