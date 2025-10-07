using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Cita
{
    public int IdCita { get; set; }

    public int IdHorario { get; set; }

    public int IdCliente { get; set; }

    public int IdEmpleado { get; set; }

    public DateOnly FechaCita { get; set; }

    public string HoraCita { get; set; } = null!;

    public string Observacion { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<CitaServicio> CitaServicios { get; set; } = new List<CitaServicio>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Horario IdHorarioNavigation { get; set; } = null!;
}
