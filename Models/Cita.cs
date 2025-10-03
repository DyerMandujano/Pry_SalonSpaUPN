using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

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

    public virtual Clientes IdClienteNavigation { get; set; } = null!;

    public virtual Empleados IdEmpleadoNavigation { get; set; } = null!;

    public virtual Turno IdHorarioNavigation { get; set; } = null!;
}
