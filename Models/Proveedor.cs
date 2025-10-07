using System;
using System.Collections.Generic;

namespace Pry_Solu_SalonSPA.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string NomProve { get; set; } = null!;

    public string Ruc { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public int Telefono { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
