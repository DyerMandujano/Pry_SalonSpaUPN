using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string NomCate { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
