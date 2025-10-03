using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class CitaServicio
{
    public int IdCitaServicio { get; set; }

    public int IdServicio { get; set; }

    public int IdCita { get; set; }

    public string Observacion { get; set; } = null!;

    public int Estado { get; set; }

    public virtual Cita IdCitaNavigation { get; set; } = null!;

    public virtual Servicio IdServicioNavigation { get; set; } = null!;
}
