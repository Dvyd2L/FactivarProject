using System;
using System.Collections.Generic;
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Users;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credenciale>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__tmp_ms_x__5B65BF97652754AA");

            entity.HasIndex(e => e.EnlaceCambioPass, "idx_unico")
                .IsUnique()
                .HasFilter("([EnlaceCambioPass] IS NOT NULL)");

            entity.Property(e => e.IdUsuario).ValueGeneratedNever();
            entity.Property(e => e.EnlaceCambioPass).HasMaxLength(50);
            entity.Property(e => e.FechaEnvioEnlace).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(500);

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Credenciale)
                .HasForeignKey<Credenciale>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Credencia__IdUsu__71D1E811");

            entity.HasOne(d => d.Roles_IdRolNavigation).WithMany(p => p.Credenciales)
                .HasForeignKey(d => d.Roles_IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        modelBuilder.Entity<DatosPersonale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC078AE81D96");

            entity.HasIndex(e => e.Email, "UQ__tmp_ms_x__A9D105343CE9FB58").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(25);
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Registro>(entity =>
        {
            entity.HasKey(e => e.IdRegistro).HasName("PK__Registro__FFA45A9994A82543");

            entity.Property(e => e.IdRegistro).ValueGeneratedNever();
            entity.Property(e => e.FechaAccion).HasColumnType("datetime");
            entity.Property(e => e.Ip).HasMaxLength(50);
            entity.Property(e => e.Observaciones).HasMaxLength(150);
            entity.Property(e => e.Operacion).HasMaxLength(50);
            entity.Property(e => e.Proceso).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__2A49584C38590C8A");

            entity.Property(e => e.IdRol).ValueGeneratedNever();
            entity.Property(e => e.Descripcion).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
