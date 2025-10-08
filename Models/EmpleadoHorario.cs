using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class EmpleadoHorario
{
    public int IdEmpleadoHorario { get; set; }

    public int IdEmpleado { get; set; }

    public int IdHorario { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Horario IdHorarioNavigation { get; set; } = null!;
}
