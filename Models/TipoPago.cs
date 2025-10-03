using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class TipoPago
{
    public int IdTipoPago { get; set; }

    public string Descripcion { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
