using Microsoft.AspNetCore.Mvc;
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
    public class TipoServicioController : Controller
    {
        private readonly Conexion _context;

        public TipoServicioController(Conexion context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string busqueda,
            int? estado,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var listaTipoServicios = new List<TipoServicio>();
            int totalRegistros = 0;
            int totalPaginas = 0;

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Listar_TipoServicio", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@NumeroPagina", pageNumber);
                command.Parameters.AddWithValue("@TamanoPagina", pageSize);
                command.Parameters.AddWithValue("@Descripcion", string.IsNullOrWhiteSpace(busqueda) ? (object)DBNull.Value : busqueda);
                command.Parameters.AddWithValue("@Estado", estado ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrdenarPor", "Id_TipoServicio");
                command.Parameters.AddWithValue("@OrdenDireccion", "ASC");

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var tipoServicio = new TipoServicio
                    {
                        IdTipoServicio = reader.GetInt32(reader.GetOrdinal("Id_TipoServicio")),
                        Descripcion = reader["Descripcion"]?.ToString() ?? "",
                        Estado = reader["Estado"] != DBNull.Value ? Convert.ToInt32(reader["Estado"]) : 0
                    };

                    if (totalRegistros == 0)
                    {
                        totalRegistros = reader["TotalRegistros"] != DBNull.Value
                            ? Convert.ToInt32(reader["TotalRegistros"])
                            : 0;
                        totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);
                    }

                    listaTipoServicios.Add(tipoServicio);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al listar tipos de servicio: {ex.Message}";
                return View(new List<TipoServicio>());
            }

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.Busqueda = busqueda;
            ViewBag.Estado = estado;

            return View(listaTipoServicios);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var tipoServicio = new TipoServicio();
            return View("_CrearTipoServicio", tipoServicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TipoServicio tipoServicio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using var connection = new SqlConnection(_context.Database.GetConnectionString());
                    await connection.OpenAsync();

                    using var command = new SqlCommand("sp_Crear_TipoServicio", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Descripcion", tipoServicio.Descripcion);
                    command.Parameters.AddWithValue("@Estado", 1);

                    await command.ExecuteNonQueryAsync();

                    TempData["Mensaje"] = "Tipo de servicio registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error al crear el tipo de servicio: {ex.Message}";
                }
            }

            return View("_CrearTipoServicio", tipoServicio);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(
            int id,
            string? busqueda,
            int? estado,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                if (id == 0)
                {
                    TempData["Mensaje"] = "ID de tipo de servicio inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var tipoServicio = await _context.TipoServicios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.IdTipoServicio == id);

                if (tipoServicio == null)
                {
                    TempData["Mensaje"] = $"No se encontró el tipo de servicio con ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.IdTipoServicio = tipoServicio.IdTipoServicio;
                ViewBag.ReturnBusqueda = busqueda;
                ViewBag.ReturnEstado = estado;
                ViewBag.ReturnPageNumber = pageNumber;
                ViewBag.ReturnPageSize = pageSize;

                return View("_EditarTipoServicio", tipoServicio);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cargar el tipo de servicio: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            TipoServicio tipoServicio,
            string? returnBusqueda,
            int? returnEstado,
            int returnPageNumber = 1,
            int returnPageSize = 10)
        {
            ModelState.Remove("Servicios");

            if (!ModelState.IsValid)
            {
                ViewBag.IdTipoServicio = id;
                ViewBag.ReturnBusqueda = returnBusqueda;
                ViewBag.ReturnEstado = returnEstado;
                ViewBag.ReturnPageNumber = returnPageNumber;
                ViewBag.ReturnPageSize = returnPageSize;

                return View("_EditarTipoServicio", tipoServicio);
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Editar_TipoServicio", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_TipoServicio", id);
                command.Parameters.AddWithValue("@Descripcion", tipoServicio.Descripcion);
                command.Parameters.AddWithValue("@Estado", tipoServicio.Estado);

                await command.ExecuteNonQueryAsync();

                TempData["Mensaje"] = "Tipo de servicio actualizado correctamente.";

                return RedirectToAction(nameof(Index), new
                {
                    busqueda = returnBusqueda,
                    estado = returnEstado,
                    pageNumber = returnPageNumber,
                    pageSize = returnPageSize
                });
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al actualizar el tipo de servicio: {ex.Message}";
                ViewBag.IdTipoServicio = id;
                ViewBag.ReturnBusqueda = returnBusqueda;
                ViewBag.ReturnEstado = returnEstado;
                ViewBag.ReturnPageNumber = returnPageNumber;
                ViewBag.ReturnPageSize = returnPageSize;

                return View("_EditarTipoServicio", tipoServicio);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            string busqueda,
            int? estado,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var tipoServicio = await _context.TipoServicios
                    .FirstOrDefaultAsync(t => t.IdTipoServicio == id);

                if (tipoServicio == null)
                {
                    TempData["Error"] = "Tipo de servicio no encontrado.";
                    return RedirectToAction(nameof(Index), new
                    {
                        busqueda,
                        estado,
                        pageNumber,
                        pageSize
                    });
                }

                int estadoAnterior = tipoServicio.Estado;
                int nuevoEstado = estadoAnterior == 1 ? 0 : 1;

                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Estado_TipoServicio", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_TipoServicio", id);
                command.Parameters.AddWithValue("@Estado", nuevoEstado);

                await command.ExecuteNonQueryAsync();

                string mensaje = nuevoEstado == 1
                    ? "Tipo de servicio activado correctamente."
                    : "Tipo de servicio inactivado correctamente.";

                if (estado.HasValue && estado.Value != nuevoEstado)
                {
                    mensaje += " (El tipo de servicio ya no aparece en esta vista debido a los filtros aplicados)";
                }

                TempData["Mensaje"] = mensaje;
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cambiar estado: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new
            {
                busqueda,
                estado,
                pageNumber,
                pageSize
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(
            int id,
            string busqueda,
            int? estado,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var tipoServicio = await _context.TipoServicios
                    .FirstOrDefaultAsync(t => t.IdTipoServicio == id);

                if (tipoServicio == null)
                {
                    TempData["Error"] = "Tipo de servicio no encontrado.";
                    return RedirectToAction(nameof(Index), new
                    {
                        busqueda,
                        estado,
                        pageNumber,
                        pageSize
                    });
                }

                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Eliminar_TipoServicio", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_TipoServicio", id);

                await command.ExecuteNonQueryAsync();

                TempData["Mensaje"] = "Tipo de servicio eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al eliminar: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new
            {
                busqueda,
                estado,
                pageNumber,
                pageSize
            });
        }
    }
}
