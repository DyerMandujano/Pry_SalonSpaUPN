using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class TipoServicio
{
    public int IdTipoServicio { get; set; }

    public string Descripcion { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
}
