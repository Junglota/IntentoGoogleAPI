using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IntentoGoogleAPI.Models;

public partial class ContabilidadContext : DbContext
{
    public ContabilidadContext()
    {
    }

    public ContabilidadContext(DbContextOptions<ContabilidadContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Descripcion> Descripcions { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Tiendum> Tienda { get; set; }

    public virtual DbSet<Tipo> Tipos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:contabilidadsinrebu.database.windows.net,1433;Initial Catalog=Contabilidad;Persist Security Info=False;User ID=enmanuel;Password=Contabilidadsinrebu@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Descripcion>(entity =>
        {
            entity.HasKey(e => e.Descripcion1).HasName("PK_TipoMovimiento");

            entity.ToTable("Descripcion");

            entity.Property(e => e.Descripcion1)
                .HasMaxLength(50)
                .HasColumnName("Descripcion");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.IntId);

            entity.Property(e => e.Estado1)
                .HasMaxLength(50)
                .HasColumnName("Estado");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IntId);

            entity.ToTable("Inventario");

            entity.Property(e => e.IdProducto).HasMaxLength(50);

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Productos");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.IntId);

            entity.Property(e => e.Descripcion).HasMaxLength(50);
            entity.Property(e => e.Fecha).HasColumnType("date");
            entity.Property(e => e.IdProducto).HasMaxLength(50);
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.DescripcionNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.Descripcion)
                .HasConstraintName("FK_Movimientos_Descripcion");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Movimientos_Productos");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("FK_Movimientos_Tienda");

            entity.HasOne(d => d.UsuarioNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.Usuario)
                .HasConstraintName("FK_Movimientos_Usuarios");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__C1FF6E912D244F56");

            entity.Property(e => e.IdProducto).HasMaxLength(50);
            entity.Property(e => e.Categoria).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(255);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Tiendum>(entity =>
        {
            entity.HasKey(e => e.IntId).HasName("PK_Heladerias");

            entity.Property(e => e.Localidad).HasMaxLength(50);

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.Tienda)
                .HasForeignKey(d => d.IdPropietario)
                .HasConstraintName("FK_Tienda_Usuarios");
        });

        modelBuilder.Entity<Tipo>(entity =>
        {
            entity.HasKey(e => e.IntId);

            entity.ToTable("Tipo");

            entity.Property(e => e.Tipo1)
                .HasMaxLength(50)
                .HasColumnName("Tipo");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IntId);

            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.Correo).HasMaxLength(50);
            entity.Property(e => e.EToken)
                .HasMaxLength(75)
                .HasColumnName("eToken");
            entity.Property(e => e.ETokenValidUntil)
                .HasColumnType("datetime")
                .HasColumnName("eTokenValidUntil");
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Estado)
                .HasConstraintName("FK_Usuarios_Estados");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("FK_Usuarios_Tienda");

            entity.HasOne(d => d.UserTypeNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.UserType)
                .HasConstraintName("FK_Usuarios_Tipo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
