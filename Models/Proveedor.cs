using System;
using System.Collections.Generic;

namespace Rumis_Salon_Spa.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string NomProve { get; set; } = null!;

    public string Ruc { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public int Telefono { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<Compras> Compras { get; set; } = new List<Compras>();
}
