using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int? IdDetalleCompra { get; set; }

    public int? IdDetalleVenta { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int Cantidad { get; set; }

    public DateOnly FechaRegistro { get; set; }

    public virtual DetalleCompra? IdDetalleCompraNavigation { get; set; }

    public virtual DetalleVentas? IdDetalleVentaNavigation { get; set; }
}
