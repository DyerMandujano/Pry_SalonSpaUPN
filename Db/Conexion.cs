using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Models;

namespace Pry_Solu_SalonSPA.Db;

public partial class Conexion : DbContext
{
    public Conexion()
    {
    }

    public Conexion(DbContextOptions<Conexion> options)
        : base(options)
    {
    }

    //AUXILIARES
    public virtual DbSet<InventarioViewModel> InventarioViewModels { get; set; }

    
    //ENTITADES

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<CitaServicio> CitaServicios { get; set; }

    public virtual DbSet<Cita> Cita { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<DetalleCompra> DetalleCompras { get; set; }

    public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EmpleadoHorario> EmpleadoHorarios { get; set; }

    public virtual DbSet<Horario> Horarios { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }


    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedor { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<TipoComprobante> TipoComprobantes { get; set; }

    public virtual DbSet<TipoPago> TipoPagos { get; set; }

    public virtual DbSet<TipoServicio> TipoServicios { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {/*
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=DB_ROMIS;Trusted_Connection=True;TrustServerCertificate=True;");
        }*/
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("Categoria_pk");

            entity.Property(e => e.IdCategoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.NomCate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nom_Cate");
        });

        modelBuilder.Entity<CitaServicio>(entity =>
        {
            entity.HasKey(e => e.IdCitaServicio).HasName("Cita_Servicio_pk");

            entity.ToTable("Cita_Servicio");

            entity.Property(e => e.IdCitaServicio).HasColumnName("Id_CitaServicio");
            entity.Property(e => e.IdCita).HasColumnName("Id_Cita");
            entity.Property(e => e.IdServicio).HasColumnName("Id_Servicio");
            entity.Property(e => e.Estado).HasColumnName("Estado");

            entity.HasOne(d => d.IdCitaNavigation).WithMany(p => p.CitaServicios)
                .HasForeignKey(d => d.IdCita)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cita_Servicio_Cita");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.CitaServicios)
                .HasForeignKey(d => d.IdServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cita_Servicio_Servicio");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.IdCita).HasName("Cita_pk");

            entity.Property(e => e.IdCita).HasColumnName("Id_Cita");
            entity.Property(e => e.FechaCita)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_cita");
            entity.Property(e => e.Descripcion)
                 .HasMaxLength(100)
                 .IsUnicode(false)
                 .HasColumnName("Observacion");
            entity.Property(e => e.Estado).HasColumnName("Estado");
            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.IdEmpleadoHorario).HasColumnName("Id_Empleado_Horario");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cita_Cliente");

            entity.HasOne(d => d.IdEmpleadoHorarioNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.IdEmpleadoHorario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cita_Empleado_Horario");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("Cliente_pk");

            entity.ToTable("Cliente");

            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.IdPersona).HasColumnName("Id_Persona");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cliente_Persona");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("Compra_pk");

            entity.ToTable("Compra");

            entity.Property(e => e.IdCompra).HasColumnName("Id_Compra");
            entity.Property(e => e.FechaCompra)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_compra");
            entity.Property(e => e.IdProveedor).HasColumnName("Id_Proveedor");
            entity.Property(e => e.TipoDoc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Tipo_Doc");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Compra_Proveedor");
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCompra).HasName("Detalle_Compra_pk");

            entity.ToTable("Detalle_Compra");

            entity.Property(e => e.IdDetalleCompra).HasColumnName("Id_Detalle_Compra");
            entity.Property(e => e.IdCompra).HasColumnName("Id_Compra");
            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.PrecioCompra)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio_Compra");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Detalle_Compra_Compra");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Detalle_Compra_Producto");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.IdDetalleVenta).HasName("Detalle_Venta_pk");
            entity.ToTable("Detalle_Venta");
            entity.Property(e => e.IdDetalleVenta).HasColumnName("Id_Detalle_Venta");
            entity.Property(e => e.IdTipoCompro).HasColumnName("Id_TipoCompro");
            entity.Property(e => e.IdVenta).HasColumnName("Id_Venta");

            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");

            entity.Property(e => e.PrecioVenta)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio_Venta");

            entity.Property(e => e.IdCitaServicio).HasColumnName("Id_CitaServicio");
            entity.Property(e => e.Cantidad).HasColumnName("Cantidad");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("Detalle_Venta_Producto");

            entity.HasOne(d => d.Ventum).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => new { d.IdVenta, d.IdTipoCompro })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Detalle_Venta_Venta");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("Empleado_pk");

            entity.ToTable("Empleado");

            entity.Property(e => e.IdEmpleado).HasColumnName("Id_Empleado");
            entity.Property(e => e.FechaRetiro).HasColumnName("Fecha_retiro");
            entity.Property(e => e.IdPerfil).HasColumnName("Id_Perfil");
            entity.Property(e => e.IdPersona).HasColumnName("Id_Persona");
            entity.Property(e => e.Sueldo).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPerfilNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Empleado_Perfil");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Empleado_Persona");
        });

        modelBuilder.Entity<EmpleadoHorario>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoHorario).HasName("Empleado_Horario_pk");

            entity.ToTable("Empleado_Horario");

            entity.Property(e => e.IdEmpleadoHorario).HasColumnName("Id_Empleado_Horario");
            entity.Property(e => e.IdEmpleado).HasColumnName("Id_Empleado");
            entity.Property(e => e.IdHorario).HasColumnName("Id_Horario");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoHorarios)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Empleado_Horario_Empleado");

            entity.HasOne(d => d.IdHorarioNavigation).WithMany(p => p.EmpleadoHorarios)
                .HasForeignKey(d => d.IdHorario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Empleado_Horario_Horario");
        });

        modelBuilder.Entity<Horario>(entity =>
        {
            entity.HasKey(e => e.IdHorario).HasName("Horario_pk");

            entity.ToTable("Horario");

            entity.Property(e => e.IdHorario).HasColumnName("Id_Horario");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("time(0)")
                .HasColumnName("Hora_Inicio");
            entity.Property(e => e.HoraFin)
                .HasColumnType("time(0)")
                .HasColumnName("Hora_Fin");
            entity.Property(e => e.DiasSemana)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Dias_Semana");
            entity.Property(e => e.Estado)
                .HasColumnName("Estado");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario).HasName("Inventario_pk");

            entity.ToTable("Inventario");

            entity.Property(e => e.IdInventario).HasColumnName("Id_Inventario");
            entity.Property(e => e.FechaRegistro).HasColumnName("Fecha_registro");
            entity.Property(e => e.IdDetalleCompra).HasColumnName("Id_Detalle_Compra");
            entity.Property(e => e.IdDetalleVenta).HasColumnName("Id_Detalle_Venta");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Tipo_Movimiento");

            entity.HasOne(d => d.IdDetalleCompraNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdDetalleCompra)
                .HasConstraintName("Inventario_Detalle_Compra");

            entity.HasOne(d => d.IdDetalleVentaNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdDetalleVenta)
                .HasConstraintName("Inventario_Detalle_Venta");
        });



        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("Marca_pk");

            entity.ToTable("Marca");

            entity.Property(e => e.IdMarca).HasColumnName("Id_Marca");
            entity.Property(e => e.NomMarca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nom_Marca");
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.IdPerfil).HasName("Perfil_pk");

            entity.ToTable("Perfil");

            entity.Property(e => e.IdPerfil).HasColumnName("Id_Perfil");
            entity.Property(e => e.NombrePerfil)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_perfil");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("Persona_pk");

            entity.ToTable("Persona");

            entity.Property(e => e.IdPersona).HasColumnName("Id_persona");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dni)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FechaNacimiento).HasColumnName("Fecha_nacimiento");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Registro");
            entity.Property(e => e.Genero)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Nombres)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("Producto_pk");

            entity.ToTable("Producto");

            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasColumnName("Fecha_registro");
            entity.Property(e => e.IdCategoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.IdMarca).HasColumnName("Id_Marca");
            entity.Property(e => e.NomProd)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nom_Prod");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Producto_Categoria");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdMarca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Producto_Marca");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("Proveedor_pk");

            entity.ToTable("Proveedor");

            entity.Property(e => e.IdProveedor).HasColumnName("Id_Proveedor");

            entity.Property(e => e.NomProve)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nom_Prove");

            entity.Property(e => e.Ruc)
                .HasMaxLength(11)
                .IsUnicode(false);

            entity.Property(e => e.Telefono)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.TipoProveedor)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("Tipo_Proveedor");

            entity.Property(e => e.Estado)
                .HasColumnName("Estado");
        });


        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("Servicio_pk");

            entity.ToTable("Servicio");

            entity.Property(e => e.IdServicio).HasColumnName("Id_Servicio");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdTipoServicio).HasColumnName("Id_TipoServicio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdTipoServicioNavigation).WithMany(p => p.Servicios)
                .HasForeignKey(d => d.IdTipoServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Servicio_Tipo_Servicio");
        });

        modelBuilder.Entity<TipoComprobante>(entity =>
        {
            entity.HasKey(e => e.IdTipoCompro).HasName("Tipo_Comprobante_pk");

            entity.ToTable("Tipo_Comprobante");

            entity.Property(e => e.IdTipoCompro).HasColumnName("Id_TipoCompro");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NomCompro)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Nom_Compro");
        });

        modelBuilder.Entity<TipoPago>(entity =>
        {
            entity.HasKey(e => e.IdTipoPago).HasName("Tipo_Pago_pk");

            entity.ToTable("Tipo_Pago");

            entity.Property(e => e.IdTipoPago).HasColumnName("Id_Tipo_Pago");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoServicio>(entity =>
        {
            entity.HasKey(e => e.IdTipoServicio).HasName("Tipo_Servicio_pk");

            entity.ToTable("Tipo_Servicio");

            entity.Property(e => e.IdTipoServicio).HasColumnName("Id_TipoServicio");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => new { e.IdVenta, e.IdTipoCompro }).HasName("Venta_pk");

            entity.Property(e => e.IdVenta)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id_Venta");
            entity.Property(e => e.IdTipoCompro).HasColumnName("Id_TipoCompro");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_registro");
            entity.Property(e => e.IdCliente).HasColumnName("Id_cliente");
            entity.Property(e => e.IdEmpleado).HasColumnName("Id_empleado");
            entity.Property(e => e.IdTipoPago).HasColumnName("Id_Tipo_Pago");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Venta_Cliente");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Venta_Empleado");

            entity.HasOne(d => d.IdTipoComproNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdTipoCompro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Venta_Tipo_Comprobante");

            entity.HasOne(d => d.IdTipoPagoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdTipoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Venta_Tipo_Pago");
        });


        //AUXILIARES

        modelBuilder.Entity<InventarioViewModel>(eb =>
        {
            eb.HasNoKey();
            eb.ToView(null);
            eb.Property(v => v.Id_Inventario).HasColumnName("Id_Inventario");
            eb.Property(v => v.Nom_Prod).HasColumnName("Nom_Prod");
            eb.Property(v => v.Cantidad).HasColumnName("Cantidad");
            eb.Property(v => v.Precio).HasColumnName("Precio");
            eb.Property(v => v.Tipo_Movimiento).HasColumnName("Tipo_Movimiento");
            eb.Property(v => v.Fecha_Registro).HasColumnName("Fecha_registro");
            eb.Property(v => v.Stock_Actual).HasColumnName("Stock_Actual");
            eb.Property(v => v.Estado_Stock).HasColumnName("Estado_Stock");
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
