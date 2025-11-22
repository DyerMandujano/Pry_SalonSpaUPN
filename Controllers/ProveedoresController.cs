using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Db;
using Pry_Solu_SalonSPA.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Pry_Solu_SalonSPA.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly Conexion _context;

        public ProveedoresController(Conexion context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string busqueda,
            int? estado,
            string tipoProveedor,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var listaProveedores = new List<Proveedor>();
            int totalRegistros = 0;
            int totalPaginas = 0;

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Listar_Proveedores", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Busqueda", string.IsNullOrWhiteSpace(busqueda) ? (object)DBNull.Value : busqueda);
                command.Parameters.AddWithValue("@Estado", estado ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TipoProveedor", string.IsNullOrWhiteSpace(tipoProveedor) ? (object)DBNull.Value : tipoProveedor);
                command.Parameters.AddWithValue("@PageNumber", pageNumber);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var proveedor = new Proveedor
                    {
                        IdProveedor = reader.GetInt32(reader.GetOrdinal("Id_Proveedor")),
                        NomProve = reader["Nom_Prove"]?.ToString() ?? "",
                        Ruc = reader["Ruc"]?.ToString() ?? "",
                        Telefono = reader["Telefono"]?.ToString() ?? "",
                        Correo = reader["Correo"]?.ToString() ?? "",
                        TipoProveedor = reader["Tipo_Proveedor"]?.ToString() ?? "",
                        Estado = reader["Estado"] != DBNull.Value
                            ? Convert.ToInt32(reader["Estado"])
                            : 0
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

                    listaProveedores.Add(proveedor);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al listar proveedores: {ex.Message}";
                return View(new List<Proveedor>());
            }

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            ViewBag.Busqueda = busqueda;
            ViewBag.Estado = estado;
            ViewBag.TipoProveedor = tipoProveedor;

            CargarCombos(estado, tipoProveedor);

            return View(listaProveedores);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            CargarCombos();
            return View("_CrearProveedor");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Proveedor model)
        {
            if (string.IsNullOrWhiteSpace(model.TipoProveedor))
            {
                ModelState.Remove("TipoProveedor");
                model.TipoProveedor = null;
            }

            if (!ModelState.IsValid)
            {
                CargarCombos(model.Estado);
                return View("_CrearProveedor", model);
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Crear_Proveedor", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Nom_Prove", model.NomProve);
                command.Parameters.AddWithValue("@Ruc", model.Ruc);
                command.Parameters.AddWithValue("@Telefono", model.Telefono ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Correo", model.Correo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Tipo_Proveedor", model.TipoProveedor ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Estado", model.Estado);

                await command.ExecuteNonQueryAsync();

                TempData["Mensaje"] = "Proveedor registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al crear el proveedor: {ex.Message}";
                CargarCombos(model.Estado);
                return View("_CrearProveedor", model);
            }
        }




        [HttpGet]
        public async Task<IActionResult> Editar(
        int id,
        string? busqueda,
        int? estado,
        string? tipoProveedor,
        int pageNumber = 1,
        int pageSize = 10)
        {
            try
            {
                if (id == 0)
                {
                    TempData["Mensaje"] = "ID de proveedor inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var proveedor = await _context.Proveedor
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.IdProveedor == id);

                if (proveedor == null)
                {
                    TempData["Mensaje"] = $"No se encontró el proveedor con ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ReturnBusqueda = busqueda;
                ViewBag.ReturnEstado = estado;
                ViewBag.ReturnTipoProveedor = tipoProveedor;
                ViewBag.ReturnPageNumber = pageNumber;
                ViewBag.ReturnPageSize = pageSize;

                return View("_EditarProveedor", proveedor);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cargar el proveedor: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            Proveedor model,
            string? returnBusqueda,
            int? returnEstado,
            string? returnTipoProveedor,
            int returnPageNumber = 1,
            int returnPageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(model.TipoProveedor))
            {
                model.TipoProveedor = null;
            }

            if (string.IsNullOrWhiteSpace(model.Telefono))
            {
                model.Telefono = null;
            }

            if (string.IsNullOrWhiteSpace(model.Correo))
            {
                model.Correo = null;
            }

            ModelState.Remove("Compras");

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnBusqueda = returnBusqueda;
                ViewBag.ReturnEstado = returnEstado;
                ViewBag.ReturnTipoProveedor = returnTipoProveedor;
                ViewBag.ReturnPageNumber = returnPageNumber;
                ViewBag.ReturnPageSize = returnPageSize;

                return View("_EditarProveedor", model);
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Editar_Proveedor", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Proveedor", model.IdProveedor);
                command.Parameters.AddWithValue("@Nom_Prove", model.NomProve);
                command.Parameters.AddWithValue("@Ruc", model.Ruc);
                command.Parameters.AddWithValue("@Telefono", (object?)model.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("@Correo", (object?)model.Correo ?? DBNull.Value);
                command.Parameters.AddWithValue("@Tipo_Proveedor", (object?)model.TipoProveedor ?? DBNull.Value);
                command.Parameters.AddWithValue("@Estado", model.Estado);

                await command.ExecuteNonQueryAsync();

                TempData["Mensaje"] = "Proveedor actualizado correctamente.";

                return RedirectToAction(nameof(Index), new
                {
                    busqueda = returnBusqueda,
                    estado = returnEstado,
                    tipoProveedor = returnTipoProveedor,
                    pageNumber = returnPageNumber,
                    pageSize = returnPageSize
                });
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al actualizar el proveedor: {ex.Message}";

                ViewBag.ReturnBusqueda = returnBusqueda;
                ViewBag.ReturnEstado = returnEstado;
                ViewBag.ReturnTipoProveedor = returnTipoProveedor;
                ViewBag.ReturnPageNumber = returnPageNumber;
                ViewBag.ReturnPageSize = returnPageSize;

                return View("_EditarProveedor", model);
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            string busqueda,
            int? estado,
            string tipoProveedor,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Estado_Proveedor", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IdProveedor", id);

                await command.ExecuteNonQueryAsync();

                TempData["Mensaje"] = "Estado del proveedor cambiado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cambiar estado: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new
            {
                busqueda,
                estado = (int?)null,
                tipoProveedor,
                pageNumber,
                pageSize
            });
        }

        private void CargarCombos(int? estadoSeleccionado = null, string? tipoProveedorSeleccionado = null)
        {
            var estadosFiltro = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Estado: Todos" },
                new SelectListItem { Value = "1", Text = "Activo" },
                new SelectListItem { Value = "0", Text = "Inactivo" }
            };

            ViewBag.EstadosFiltro = new SelectList(estadosFiltro, "Value", "Text");
            ViewBag.TipoProveedorActual = tipoProveedorSeleccionado;
        }

    }
}
