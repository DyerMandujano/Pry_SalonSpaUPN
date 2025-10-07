using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Item
{
    public int IdItem { get; set; }

    public string TipoItem { get; set; } = null!;

    public int? IdProducto { get; set; }

    public int? IdServicio { get; set; }

    public virtual ICollection<DetalleVentas> DetalleVenta { get; set; } = new List<DetalleVentas>();

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Servicio? IdServicioNavigation { get; set; }
}
