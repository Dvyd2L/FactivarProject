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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Cliente>(entity =>
        {
            _ = entity.HasKey(e => e.Cif).HasName("PK__Clientes__C1FFD867156CDEC7");

            _ = entity.Property(e => e.Cif)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            _ = entity.Property(e => e.Direccion).HasMaxLength(50);
            _ = entity.Property(e => e.Email).HasMaxLength(50);
            _ = entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        _ = modelBuilder.Entity<Factura>(entity =>
        {
            _ = entity.HasKey(e => e.FacturaId).HasName("PK__tmp_ms_x__5C0248059183DD5B");

            _ = entity.ToTable("Factura");

            _ = entity.Property(e => e.FacturaId).HasColumnName("FacturaID");
            _ = entity.Property(e => e.Articulos).IsUnicode(false);
            _ = entity.Property(e => e.ClienteId)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            _ = entity.Property(e => e.DescripcionOperacion).HasMaxLength(500);
            _ = entity.Property(e => e.ProveedorId)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();

            _ = entity.HasOne(d => d.Cliente).WithMany(p => p.FacturaClientes)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Factura__Cliente__4BAC3F29");

            _ = entity.HasOne(d => d.Proveedor).WithMany(p => p.FacturaProveedors)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Factura__Proveed__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
