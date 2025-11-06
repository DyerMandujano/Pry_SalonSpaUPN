using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class DetalleCompra
{
    public int IdDetalleCompra { get; set; }

    public int IdCompra { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioCompra { get; set; }

    public virtual Compra IdCompraNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual ICollection<Inventario> Inventarios { get; set; } = new HashSet<Inventario>();
}
