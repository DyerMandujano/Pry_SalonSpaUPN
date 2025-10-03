using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class Marcas
{
    public int IdMarca { get; set; }

    public string NomMarca { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
