using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Compra
{
    public int IdCompra { get; set; }

    public int IdProveedor { get; set; }

    public string TipoDoc { get; set; } = null!;

    public DateTime FechaCompra { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;
}
