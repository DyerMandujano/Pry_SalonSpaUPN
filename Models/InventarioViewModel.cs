using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Pry_Solu_SalonSPA.Models
{
    [Keyless]
    public class InventarioViewModel
    {
        public int Id_Inventario { get; set; }
        public string? Nom_Prod { get; set; }
        public int? Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public string? Tipo_Movimiento { get; set; }
        public DateTime? Fecha_Registro { get; set; }
        public int? Stock_Actual { get; set; }
        public string? Estado_Stock { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}
