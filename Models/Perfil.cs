using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Perfil
{
    public int IdPerfil { get; set; }

    public string NombrePerfil { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
