using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Cita
{
    public int IdCita { get; set; }

    public int IdCliente { get; set; }

    public int IdEmpleadoHorario { get; set; }

    public DateTime FechaCita { get; set; }

    public string Descripcion { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<CitaServicio> CitaServicios { get; set; } = new List<CitaServicio>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual EmpleadoHorario IdEmpleadoHorarioNavigation { get; set; } = null!;
}
