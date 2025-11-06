using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Servicio
{
    public int IdServicio { get; set; }

    public int IdTipoServicio { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Precio { get; set; }

    public int Duracion { get; set; }

    public virtual ICollection<CitaServicio> CitaServicios { get; set; } = new List<CitaServicio>();

    public virtual TipoServicio IdTipoServicioNavigation { get; set; } = null!;

}
