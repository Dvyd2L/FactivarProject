using Microsoft.EntityFrameworkCore;

namespace AuthMS.Models;

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

    public virtual DbSet<Registro> Registros { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Credenciale>(entity =>
        {
            _ = entity.HasKey(e => e.IdUsuario).HasName("PK__tmp_ms_x__5B65BF97652754AA");

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
                .HasConstraintName("FK__Credencia__IdUsu__71D1E811");

            _ = entity.HasOne(d => d.Roles_IdRolNavigation).WithMany(p => p.Credenciales)
                .HasForeignKey(d => d.Roles_IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
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

        _ = modelBuilder.Entity<Registro>(entity =>
        {
            _ = entity.HasKey(e => e.IdRegistro).HasName("PK__Registro__FFA45A9994A82543");

            _ = entity.Property(e => e.IdRegistro).ValueGeneratedNever();
            _ = entity.Property(e => e.FechaAccion).HasColumnType("datetime");
            _ = entity.Property(e => e.Ip).HasMaxLength(50);
            _ = entity.Property(e => e.Observaciones).HasMaxLength(150);
            _ = entity.Property(e => e.Operacion).HasMaxLength(50);
            _ = entity.Property(e => e.Proceso).HasMaxLength(50);
        });

        _ = modelBuilder.Entity<Role>(entity =>
        {
            _ = entity.HasKey(e => e.IdRol).HasName("PK__Roles__2A49584C38590C8A");

            _ = entity.Property(e => e.IdRol).ValueGeneratedNever();
            _ = entity.Property(e => e.Descripcion).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
