using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class Turno
{
    public int IdHorario { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public string HoraInicio { get; set; } = null!;

    public string HoraFin { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();
}
