using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Db;
using Pry_Solu_SalonSPA.Models;

namespace Pry_Solu_SalonSPA.Controllers
{
    public class InventarioController : Controller
    {
        private readonly Conexion _context;

        public InventarioController(Conexion context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string orden = "ASC", string nomProd = null, 
                            DateTime? fechaInicio = null, DateTime? fechaFin = null, string tipoMovimiento = null)
        {
            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@Orden", orden ?? (object)DBNull.Value),
                    new SqlParameter("@Nom_Prod", (object?)nomProd ?? DBNull.Value),
                    new SqlParameter("@FechaInicio", (object?)fechaInicio ?? DBNull.Value),
                    new SqlParameter("@FechaFin", (object?)fechaFin ?? DBNull.Value),
                    new SqlParameter("@Tipo_Movimiento", (object?)tipoMovimiento ?? DBNull.Value)
                };

                var lista = await _context.Set<InventarioViewModel>()
                    .FromSqlRaw("EXEC sp_Listar_Inventario @Orden, @Nom_Prod, @FechaInicio, @FechaFin, @Tipo_Movimiento", parametros)
                    .ToListAsync();

                return View(lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al listar inventario: {ex.Message}";
                return View(new List<InventarioViewModel>());
            }
        }

    }
}
