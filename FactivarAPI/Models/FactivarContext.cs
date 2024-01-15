using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FactivarAPI.Models;

public partial class FactivarContext : DbContext
{
    public FactivarContext()
    {
    }

    public FactivarContext(DbContextOptions<FactivarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Factivar;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Cif).HasName("PK__Clientes__C1FFD867156CDEC7");

            entity.Property(e => e.Cif)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Direccion).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.NumeroFactura).HasName("PK__Factura__CF12F9A7E18A1D45");

            entity.ToTable("Factura");

            entity.Property(e => e.NumeroFactura).ValueGeneratedNever();
            entity.Property(e => e.Articulos).IsUnicode(false);
            entity.Property(e => e.ClienteId)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DescripcionOperacion).HasMaxLength(500);
            entity.Property(e => e.Importe).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(9, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.Total).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Factura__Cliente__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
