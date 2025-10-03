using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class Persona
{
    public int IdPersona { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Genero { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Clientes> Clientes { get; set; } = new List<Clientes>();

    public virtual ICollection<Empleados> Empleados { get; set; } = new List<Empleados>();
}
