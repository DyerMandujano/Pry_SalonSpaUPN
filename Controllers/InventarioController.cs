using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public async Task<IActionResult> Index(
            string orden = "ASC",
            string nomProd = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string tipoMovimiento = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var listaInventario = new List<InventarioViewModel>();
            int totalRegistros = 0;
            int totalPaginas = 0;

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Listar_Inventario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Orden", orden ?? "ASC");
                command.Parameters.AddWithValue("@Nom_Prod", string.IsNullOrWhiteSpace(nomProd) ? (object)DBNull.Value : nomProd);
                command.Parameters.AddWithValue("@FechaInicio", fechaInicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FechaFin", fechaFin ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Tipo_Movimiento", string.IsNullOrWhiteSpace(tipoMovimiento) ? (object)DBNull.Value : tipoMovimiento);
                command.Parameters.AddWithValue("@PageNumber", pageNumber);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var inventario = new InventarioViewModel
                    {
                        Id_Inventario = reader.GetInt32(reader.GetOrdinal("Id_Inventario")),
                        Nom_Prod = reader["Nom_Prod"]?.ToString(),
                        Cantidad = reader["Cantidad"] != DBNull.Value ? Convert.ToInt32(reader["Cantidad"]) : 0,
                        Precio = reader["Precio"] != DBNull.Value ? Convert.ToDecimal(reader["Precio"]) : 0m,
                        Tipo_Movimiento = reader["Tipo_Movimiento"]?.ToString(),
                        Fecha_Registro = reader["Fecha_registro"] != DBNull.Value
                            ? Convert.ToDateTime(reader["Fecha_registro"])
                            : null,
                        Stock_Actual = reader["Stock_Actual"] != DBNull.Value ? Convert.ToInt32(reader["Stock_Actual"]) : 0,
                        Estado_Stock = reader["Estado_Stock"]?.ToString()
                    };

                    if (totalRegistros == 0)
                    {
                        totalRegistros = reader["TotalRegistros"] != DBNull.Value
                            ? Convert.ToInt32(reader["TotalRegistros"])
                            : 0;
                        totalPaginas = reader["TotalPaginas"] != DBNull.Value
                            ? Convert.ToInt32(reader["TotalPaginas"])
                            : 0;
                    }

                    listaInventario.Add(inventario);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al listar inventario: {ex.Message}";
                return View(new List<InventarioViewModel>());
            }

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            ViewBag.Orden = orden;
            ViewBag.NomProd = nomProd;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewBag.TipoMovimiento = tipoMovimiento;

            return View(listaInventario);
        }
    }
}
