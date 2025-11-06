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
    public class ClientesController : Controller
    {
        private readonly Conexion _context;

        public ClientesController(Conexion context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(
            string busqueda,
            bool? estado,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var listaClientes = new List<Cliente>();
            int totalRegistros = 0;
            int totalPaginas = 0;

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Listar_Clientes", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Busqueda", string.IsNullOrWhiteSpace(busqueda) ? (object)DBNull.Value : busqueda);
                command.Parameters.AddWithValue("@Estado", estado ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FechaInicio", fechaInicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FechaFin", fechaFin ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PageNumber", pageNumber);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var cliente = new Cliente
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("Id_Cliente")),
                        IdPersona = reader.GetInt32(reader.GetOrdinal("Id_Persona")),
                        IdPersonaNavigation = new Persona
                        {
                            IdPersona = reader.GetInt32(reader.GetOrdinal("Id_Persona")),
                            Nombres = reader["Nombres"]?.ToString() ?? "",
                            Apellidos = reader["Apellidos"]?.ToString() ?? "",
                            Telefono = reader["Telefono"]?.ToString() ?? "",
                            Dni = reader["Dni"]?.ToString() ?? "",
                            FechaNacimiento = reader["Fecha_nacimiento"] != DBNull.Value
                                ? DateOnly.FromDateTime(Convert.ToDateTime(reader["Fecha_nacimiento"]))
                                : DateOnly.MinValue,
                            Genero = reader["Genero"]?.ToString() ?? "",
                            FechaRegistro = reader["Fecha_Registro"] != DBNull.Value
                                ? Convert.ToDateTime(reader["Fecha_Registro"])
                                : DateTime.MinValue,
                            Estado = reader["Estado"] != DBNull.Value
                                ? Convert.ToInt32(reader["Estado"])
                                : 0
                        }
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

                    listaClientes.Add(cliente);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al listar clientes: {ex.Message}";
                return View(new List<Cliente>());
            }

            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            ViewBag.Busqueda = busqueda;
            ViewBag.Estado = estado;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            return View(listaClientes);
        }

        // GET: Clientes/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            var persona = new Persona();

            ViewBag.Generos = new List<SelectListItem>
            {
                new SelectListItem { Text = "Masculino", Value = "Masculino" },
                new SelectListItem { Text = "Femenino", Value = "Femenino" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            return View("_CrearClientes", persona);
        }

        // POST: Clientes/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Persona persona)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using var connection = new SqlConnection(_context.Database.GetConnectionString());
                    await connection.OpenAsync();

                    using var command = new SqlCommand("sp_Crear_Cliente", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@NombresPersona", persona.Nombres);
                    command.Parameters.AddWithValue("@ApellidosPersona", persona.Apellidos);
                    command.Parameters.AddWithValue("@TelefonoPersona", persona.Telefono);
                    command.Parameters.AddWithValue("@DniPersona", persona.Dni);
                    command.Parameters.AddWithValue("@Fecha_nacimientoPersona", persona.FechaNacimiento);
                    command.Parameters.AddWithValue("@GeneroPersona", persona.Genero);
                    command.Parameters.AddWithValue("@Estado_Persona", 1);

                    await command.ExecuteNonQueryAsync();

                    TempData["Mensaje"] = "Cliente registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error al crear el cliente: {ex.Message}";
                }
            }

            ViewBag.Generos = new List<SelectListItem>
            {
                new SelectListItem { Text = "Masculino", Value = "Masculino" },
                new SelectListItem { Text = "Femenino", Value = "Femenino" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            return View("_CrearClientes", persona);
        }

        // GET: Clientes/Editar/5
        [HttpGet]
        public async Task<IActionResult> Editar(
            int id,
            string busqueda,
            bool? estado,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                if (id == 0)
                {
                    TempData["Mensaje"] = "ID de cliente inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var cliente = await _context.Clientes
                    .Include(c => c.IdPersonaNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdCliente == id);

                if (cliente == null)
                {
                    TempData["Mensaje"] = $"No se encontró el cliente con ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                if (cliente.IdPersonaNavigation == null)
                {
                    TempData["Mensaje"] = "Error: Datos de persona del cliente no encontrados.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.IdCliente = cliente.IdCliente;
                ViewBag.Generos = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Masculino", Value = "Masculino" },
                    new SelectListItem { Text = "Femenino", Value = "Femenino" },
                    new SelectListItem { Text = "Otro", Value = "Otro" }
                };

                // Mantener parámetros para regresar a la misma página
                ViewBag.ReturnBusqueda = busqueda;
                ViewBag.ReturnEstado = estado;
                ViewBag.ReturnFechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
                ViewBag.ReturnFechaFin = fechaFin?.ToString("yyyy-MM-dd");
                ViewBag.ReturnPageNumber = pageNumber;
                ViewBag.ReturnPageSize = pageSize;

                return View("_EditarClientes", cliente.IdPersonaNavigation);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cargar el cliente: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Clientes/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            Persona persona,
            string busqueda,
            bool? estado,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using var connection = new SqlConnection(_context.Database.GetConnectionString());
                    await connection.OpenAsync();

                    using var command = new SqlCommand("SP_Editar_Cliente", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Id_Cliente", id);
                    command.Parameters.AddWithValue("@NombresPersona", persona.Nombres);
                    command.Parameters.AddWithValue("@ApellidosPersona", persona.Apellidos);
                    command.Parameters.AddWithValue("@TelefonoPersona", persona.Telefono);
                    command.Parameters.AddWithValue("@DniPersona", persona.Dni);
                    command.Parameters.AddWithValue("@Fecha_nacimientoPersona", persona.FechaNacimiento);
                    command.Parameters.AddWithValue("@GeneroPersona", persona.Genero);
                    command.Parameters.AddWithValue("@Estado_Persona", persona.Estado);

                    await command.ExecuteNonQueryAsync();

                    TempData["Mensaje"] = "Cliente actualizado correctamente.";

                    return RedirectToAction(nameof(Index), new
                    {
                        busqueda,
                        estado,
                        fechaInicio,
                        fechaFin,
                        pageNumber,
                        pageSize
                    });
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error al actualizar el cliente: {ex.Message}";
                }
            }

            ViewBag.Generos = new List<SelectListItem>
            {
                new SelectListItem { Text = "Masculino", Value = "Masculino" },
                new SelectListItem { Text = "Femenino", Value = "Femenino" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            ViewBag.ReturnBusqueda = busqueda;
            ViewBag.ReturnEstado = estado;
            ViewBag.ReturnFechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.ReturnFechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewBag.ReturnPageNumber = pageNumber;
            ViewBag.ReturnPageSize = pageSize;

            return View("_EditarClientes", persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            string busqueda,
            bool? estado,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var cliente = await _context.Clientes
                    .Include(c => c.IdPersonaNavigation)
                    .FirstOrDefaultAsync(c => c.IdCliente == id);

                if (cliente == null)
                {
                    TempData["Error"] = "Cliente no encontrado.";
                    return RedirectToAction(nameof(Index), new
                    {
                        busqueda,
                        estado,
                        fechaInicio,
                        fechaFin,
                        pageNumber,
                        pageSize
                    });
                }

                int estadoAnterior = cliente.IdPersonaNavigation.Estado;
                int nuevoEstado = estadoAnterior == 1 ? 0 : 1;

                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Estado_Cliente", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Cliente", id);
                command.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);

                await command.ExecuteNonQueryAsync();

                // Mensaje mejorado que explica si el cliente desaparecerá
                string mensaje = nuevoEstado == 1
                    ? "Cliente activado correctamente."
                    : "Cliente inactivado correctamente.";

                // Si hay filtro de estado activo y el nuevo estado no coincide
                if (estado.HasValue && estado.Value != (nuevoEstado == 1))
                {
                    mensaje += " (El cliente ya no aparece en esta vista debido a los filtros aplicados)";
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
                fechaInicio,
                fechaFin,
                pageNumber,
                pageSize
            });
        }


    }
}
