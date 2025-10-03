using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public int IdTipoCompro { get; set; }

    public int IdEmpleado { get; set; }

    public int IdCliente { get; set; }

    public int IdTipoPago { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<DetalleVentas> DetalleVenta { get; set; } = new List<DetalleVentas>();

    public virtual Clientes IdClienteNavigation { get; set; } = null!;

    public virtual Empleados IdEmpleadoNavigation { get; set; } = null!;

    public virtual TipoComprobante IdTipoComproNavigation { get; set; } = null!;

    public virtual TipoPago IdTipoPagoNavigation { get; set; } = null!;
}
