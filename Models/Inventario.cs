using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int? IdDetalleCompra { get; set; }

    public int? IdDetalleVenta { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public DateOnly FechaRegistro { get; set; }

    public virtual DetalleCompra? IdDetalleCompraNavigation { get; set; }

    public virtual DetalleVenta? IdDetalleVentaNavigation { get; set; }
}
