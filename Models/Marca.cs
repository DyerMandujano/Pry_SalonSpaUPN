using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string NomMarca { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
