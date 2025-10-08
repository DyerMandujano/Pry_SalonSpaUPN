using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public int IdTipoCompro { get; set; }

    public int IdEmpleado { get; set; }

    public int IdCliente { get; set; }

    public int IdTipoPago { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual TipoComprobante IdTipoComproNavigation { get; set; } = null!;

    public virtual TipoPago IdTipoPagoNavigation { get; set; } = null!;
}
