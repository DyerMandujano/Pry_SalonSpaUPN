using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class Perfil
{
    public int IdPerfil { get; set; }

    public string NombrePerfil { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Empleados> Empleados { get; set; } = new List<Empleados>();
}
