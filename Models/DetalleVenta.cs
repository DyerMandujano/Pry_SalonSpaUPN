using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class DetalleVenta
{
    public int IdDetalleVenta { get; set; }

    public int IdTipoCompro { get; set; }

    public int IdVenta { get; set; }

    public int IdItem { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public virtual Item IdItemNavigation { get; set; } = null!;

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();

    public virtual Venta Ventum { get; set; } = null!;
}
