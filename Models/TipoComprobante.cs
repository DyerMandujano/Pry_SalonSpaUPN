using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class TipoComprobante
{
    public int IdTipoCompro { get; set; }

    public string NomCompro { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
