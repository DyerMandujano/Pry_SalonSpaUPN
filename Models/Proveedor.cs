using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string NomProve { get; set; } = null!;

    public string Ruc { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
