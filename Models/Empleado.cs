using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public int IdPersona { get; set; }

    public int IdPerfil { get; set; }

    public decimal Sueldo { get; set; }

    public DateOnly? FechaRetiro { get; set; }

    public virtual ICollection<EmpleadoHorario> EmpleadoHorarios { get; set; } = new List<EmpleadoHorario>();

    public virtual Perfil IdPerfilNavigation { get; set; } = null!;

    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
