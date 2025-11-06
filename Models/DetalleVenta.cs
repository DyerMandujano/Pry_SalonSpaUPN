using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models
{
    // MODIFICACIÓN: clase actualizada para reflejar la nueva DDL (ya no usamos Item)
    public partial class DetalleVenta
    {
        public int IdDetalleVenta { get; set; }
        public int IdTipoCompro { get; set; }
        public int IdVenta { get; set; }
        public int? IdProducto { get; set; }
        public int? IdCitaServicio { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public virtual Producto IdProductoNavigation { get; set; }
        public virtual Venta Ventum { get; set; }
        public virtual ICollection<Inventario> Inventarios { get; set; } = new HashSet<Inventario>();

    }
}