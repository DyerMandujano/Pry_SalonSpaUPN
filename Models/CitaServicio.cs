using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class CitaServicio
{
    public int IdCitaServicio { get; set; }

    public int IdServicio { get; set; }

    public int IdCita { get; set; }

    public int Estado { get; set; }

    public virtual Cita IdCitaNavigation { get; set; } = null!;

    public virtual Servicio IdServicioNavigation { get; set; } = null!;
}
