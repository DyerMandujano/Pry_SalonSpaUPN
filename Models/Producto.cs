using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public int IdCategoria { get; set; }

    public int IdMarca { get; set; }

    public string NomProd { get; set; } = null!;

    public int Stock { get; set; }

    public decimal Precio { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaRegistro { get; set; }

    public int Estado { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();
    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new HashSet<DetalleVenta>();

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

}
