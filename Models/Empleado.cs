using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public int IdPersona { get; set; }

    public int IdPerfil { get; set; }

    public decimal Sueldo { get; set; }

    public DateOnly FechaIngreso { get; set; }

    public DateOnly? FechaRetiro { get; set; }

    public int Estado { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual Perfil IdPerfilNavigation { get; set; } = null!;

    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
