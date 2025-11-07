using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pry_Solu_SalonSPA.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    [Required(ErrorMessage = "El nombre del proveedor es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    public string NomProve { get; set; } = null!;

    [Required(ErrorMessage = "El RUC es requerido")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "El RUC debe tener 11 dígitos")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "El RUC debe contener solo números")]
    public string Ruc { get; set; } = null!;

    [StringLength(9, MinimumLength = 9, ErrorMessage = "El teléfono debe tener 9 dígitos")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe contener solo números")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El correo no es válido")]
    [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
    public string? Correo { get; set; }

    [StringLength(40, ErrorMessage = "El tipo de proveedor no puede exceder 40 caracteres")]
    public string? TipoProveedor { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    [Range(0, 1, ErrorMessage = "El estado debe ser 0 o 1")]
    public int Estado { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}