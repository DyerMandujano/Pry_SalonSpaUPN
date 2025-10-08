using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Horario
{
    public int IdHorario { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int Estado { get; set; }

    public virtual ICollection<EmpleadoHorario> EmpleadoHorarios { get; set; } = new List<EmpleadoHorario>();
}
