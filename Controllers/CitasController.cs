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
    public class CitasController : Controller
    {
        private readonly Conexion _context;

        public CitasController(Conexion context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string dni, DateTime? fechaInicio, DateTime? fechaFin,
                                               int? estadoCita, int? estadoServicio,
                                               int pageNumber = 1, int pageSize = 10)
        {
            var listaCitas = new List<Cita>();

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Listar_Citas", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@DNI", string.IsNullOrWhiteSpace(dni) ? (object)DBNull.Value : dni);
                command.Parameters.AddWithValue("@FechaInicio", fechaInicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FechaFin", fechaFin ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EstadoCita", estadoCita ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EstadoServicio", estadoServicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PageNumber", pageNumber);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var cita = new Cita
                    {
                        IdCita = reader.GetInt32(reader.GetOrdinal("Id_Cita")),
                        FechaCita = reader.GetDateTime(reader.GetOrdinal("Fecha_Cita")),
                        Estado = reader["Estado_Cita"] != DBNull.Value ? Convert.ToInt32(reader["Estado_Cita"]) : 0,
                        Descripcion = reader["Observacion_Servicio"]?.ToString(),
                        IdClienteNavigation = new Cliente
                        {
                            IdPersonaNavigation = new Persona
                            {
                                Nombres = reader["Cliente"]?.ToString()
                            }
                        },
                        IdEmpleadoHorarioNavigation = new EmpleadoHorario
                        {
                            IdEmpleadoNavigation = new Empleado
                            {
                                IdPersonaNavigation = new Persona
                                {
                                    Nombres = reader["Empleado"]?.ToString()
                                }
                            }
                        }
                    };

                    var citaServicio = new CitaServicio
                    {
                        IdServicioNavigation = new Servicio
                        {
                            Nombre = reader["Servicio"]?.ToString(),
                            Precio = reader["Precio"] != DBNull.Value ? reader.GetDecimal(reader.GetOrdinal("Precio")) : 0m
                        },
                        Estado = reader["Estado_Servicio"] != DBNull.Value ? Convert.ToInt32(reader["Estado_Servicio"]) : 0
                    };

                    cita.CitaServicios.Add(citaServicio);
                    listaCitas.Add(cita);
                }

                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    ViewBag.TotalCitas = Convert.ToInt32(reader["TotalCitas"]);
                    ViewBag.Pendientes = Convert.ToInt32(reader["Pendientes"]);
                    ViewBag.Completadas = Convert.ToInt32(reader["Completadas"]);
                    ViewBag.Reprogramadas = Convert.ToInt32(reader["Reprogramadas"]);
                    ViewBag.Canceladas = Convert.ToInt32(reader["Canceladas"]);
                    ViewBag.ServiciosActivos = Convert.ToInt32(reader["ServiciosActivos"]);
                    ViewBag.ServiciosInactivos = Convert.ToInt32(reader["ServiciosInactivos"]);
                    ViewBag.TotalPaginas = Convert.ToInt32(reader["TotalPaginas"]);
                }
                else
                {
                    ViewBag.TotalCitas = 0;
                    ViewBag.Pendientes = 0;
                    ViewBag.Completadas = 0;
                    ViewBag.Reprogramadas = 0;
                    ViewBag.Canceladas = 0;
                    ViewBag.ServiciosActivos = 0;
                    ViewBag.ServiciosInactivos = 0;
                    ViewBag.TotalPaginas = 0;
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al listar citas: {ex.Message}";
                return RedirectToAction("Index");
            }

            ViewBag.DniActual = dni;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewBag.EstadoCitaSeleccionado = estadoCita;
            ViewBag.EstadoServicioSeleccionado = estadoServicio;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            return View(listaCitas);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            CargarListasDesplegables();
            ViewBag.ClienteEncontrado = null;
            ViewBag.DniBuscado = null;
            ViewBag.Error = null;
            return View("_CrearCitas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int IdCliente, int IdEmpleadoHorario, DateTime FechaCita, int IdServicio,
                                                string ObsServicio, int Estado_Cita = 1, int Estado_Cita_Servicio = 2)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Crear_Cita", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Cliente", IdCliente);
                command.Parameters.AddWithValue("@Id_Empleado_Horario", IdEmpleadoHorario);
                command.Parameters.AddWithValue("@Fecha_Cita", FechaCita);
                command.Parameters.AddWithValue("@Id_Servicio", IdServicio);
                command.Parameters.AddWithValue("@Obs_Servicio", string.IsNullOrWhiteSpace(ObsServicio) ? (object)DBNull.Value : ObsServicio);
                command.Parameters.AddWithValue("@Estado_Cita", Estado_Cita);
                command.Parameters.AddWithValue("@Estado_Cita_Servicio", Estado_Cita_Servicio);

                using var reader = await command.ExecuteReaderAsync();
                int? nuevaCitaId = null;
                if (await reader.ReadAsync())
                {
                    if (reader["Id_Cita_Creada"] != DBNull.Value)
                        nuevaCitaId = Convert.ToInt32(reader["Id_Cita_Creada"]);
                }

                TempData["Mensaje"] = "Cita creada correctamente.";
                if (nuevaCitaId.HasValue)
                    return RedirectToAction(nameof(Index), new { id = nuevaCitaId.Value });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al crear la cita: {ex.Message}";
            }

            CargarListasDesplegables(IdCliente, IdEmpleadoHorario, IdServicio);
            return View("_CrearCitas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarClientePorDniPost(string dni)
        {
            CargarListasDesplegables();

            if (string.IsNullOrWhiteSpace(dni))
            {
                ViewBag.Error = "Debe ingresar un DNI válido.";
                return View("_CrearCitas");
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Buscar_Cliente_Por_DNI", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@DNI", dni.Trim());

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    ViewBag.ClienteEncontrado = new
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("Id_Cliente")),
                        Nombres = reader["Nombres"]?.ToString(),
                        Apellidos = reader["Apellidos"]?.ToString(),
                        DNI = reader["DNI"]?.ToString()
                    };
                    ViewBag.DniBuscado = dni;
                }
                else
                {
                    ViewBag.Error = "No se encontró ningún cliente con el DNI ingresado.";
                    ViewBag.DniBuscado = dni;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al buscar cliente: {ex.Message}";
            }

            return View("_CrearCitas");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id, string dni, DateTime? fechaInicio, DateTime? fechaFin,
                                                int? estadoCita, int? estadoServicio,
                                                int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (id == 0)
                {
                    TempData["Mensaje"] = "ID de cita inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var cita = await _context.Cita
                    .Include(c => c.IdClienteNavigation)
                        .ThenInclude(cl => cl.IdPersonaNavigation)
                    .Include(c => c.IdEmpleadoHorarioNavigation)
                        .ThenInclude(eh => eh.IdEmpleadoNavigation)
                            .ThenInclude(e => e.IdPersonaNavigation)
                    .Include(c => c.CitaServicios)
                        .ThenInclude(cs => cs.IdServicioNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdCita == id);

                if (cita == null)
                {
                    TempData["Mensaje"] = $"No se encontró la cita con ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                if (cita.IdClienteNavigation == null)
                {
                    TempData["Mensaje"] = "Error: Cliente no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                if (cita.IdClienteNavigation.IdPersonaNavigation == null)
                {
                    TempData["Mensaje"] = "Error: Datos de persona del cliente no encontrados.";
                    return RedirectToAction(nameof(Index));
                }

                var citaServicio = cita.CitaServicios?.FirstOrDefault();

                ViewBag.IdCita = cita.IdCita;
                ViewBag.ClienteEncontrado = new
                {
                    IdCliente = cita.IdClienteNavigation.IdCliente,
                    DNI = cita.IdClienteNavigation.IdPersonaNavigation.Dni ?? "",
                    Nombres = cita.IdClienteNavigation.IdPersonaNavigation.Nombres ?? "",
                    Apellidos = cita.IdClienteNavigation.IdPersonaNavigation.Apellidos ?? ""
                };
                ViewBag.DniBuscado = cita.IdClienteNavigation.IdPersonaNavigation.Dni ?? "";
                ViewBag.IdEmpleadoHorario = cita.IdEmpleadoHorario;
                ViewBag.FechaCita = cita.FechaCita.ToString("yyyy-MM-ddTHH:mm");
                ViewBag.IdServicio = citaServicio?.IdServicio ?? 0;
                ViewBag.Observacion = cita.Descripcion ?? "";
                ViewBag.EstadoCita = cita.Estado;
                ViewBag.EstadoCitaServicio = citaServicio?.Estado ?? 1;

                ViewBag.ReturnDni = dni;
                ViewBag.ReturnFechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
                ViewBag.ReturnFechaFin = fechaFin?.ToString("yyyy-MM-dd");
                ViewBag.ReturnEstadoCita = estadoCita;
                ViewBag.ReturnEstadoServicio = estadoServicio;
                ViewBag.ReturnPageNumber = pageNumber;
                ViewBag.ReturnPageSize = pageSize;

                CargarListasDesplegables();

                return View("_EditarCitas");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cargar la cita: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int Id_Cita, int IdCliente, int IdEmpleadoHorario, DateTime FechaCita,
                                                int IdServicio, string ObsServicio, int Estado_Cita, int Estado_Cita_Servicio,
                                                string dni, DateTime? fechaInicio, DateTime? fechaFin,
                                                int? estadoCita, int? estadoServicio,
                                                int pageNumber = 1, int pageSize = 10)
        {
            if (IdCliente == 0)
            {
                ViewBag.Error = "Debe seleccionar un cliente.";
                CargarListasDesplegables();
                return View("_EditarCitas");
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Editar_Cita", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Cita", Id_Cita);
                command.Parameters.AddWithValue("@Id_Cliente", IdCliente);
                command.Parameters.AddWithValue("@Id_Empleado_Horario", IdEmpleadoHorario);
                command.Parameters.AddWithValue("@Fecha_Cita", FechaCita);
                command.Parameters.AddWithValue("@Id_Servicio", IdServicio);
                command.Parameters.AddWithValue("@Obs_Servicio", string.IsNullOrWhiteSpace(ObsServicio) ? (object)DBNull.Value : ObsServicio);
                command.Parameters.AddWithValue("@Estado_Cita", Estado_Cita);
                command.Parameters.AddWithValue("@Estado_Cita_Servicio", Estado_Cita_Servicio);

                await command.ExecuteNonQueryAsync();

                TempData["Mensaje"] = "Cita actualizada correctamente.";

                return RedirectToAction(nameof(Index), new
                {
                    dni = dni,
                    fechaInicio = fechaInicio,
                    fechaFin = fechaFin,
                    estadoCita = estadoCita,
                    estadoServicio = estadoServicio,
                    pageNumber = pageNumber,
                    pageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al actualizar la cita: {ex.Message}";
                CargarListasDesplegables();
                return View("_EditarCitas");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string dni, DateTime? fechaInicio, DateTime? fechaFin,
                                                        int? estadoCita, int? estadoServicio,
                                                        int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Estado_Cita", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Cita", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    int resultado = reader.GetInt32(reader.GetOrdinal("Resultado"));
                    string mensaje = reader["Mensaje"]?.ToString();

                    if (resultado == 1)
                    {
                        TempData["Mensaje"] = mensaje;
                    }
                    else
                    {
                        TempData["Mensaje"] = $"Error: {mensaje}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al cambiar estado: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new
            {
                dni = dni,
                fechaInicio = fechaInicio,
                fechaFin = fechaFin,
                estadoCita = estadoCita,
                estadoServicio = estadoServicio,
                pageNumber = pageNumber,
                pageSize = pageSize
            });
        }

        private void CargarListasDesplegables(int? idCliente = null, int? idEmpleadoHorario = null, int? idServicio = null)
        {
            ViewData["Clientes"] = new SelectList(
                _context.Clientes.Include(c => c.IdPersonaNavigation).AsNoTracking().ToList(),
                "IdCliente",
                "IdPersonaNavigation.Nombres",
                idCliente
            );

            ViewData["Empleados"] = _context.EmpleadoHorarios
                .Include(eh => eh.IdEmpleadoNavigation)
                .ThenInclude(e => e.IdPersonaNavigation)
                .AsNoTracking()
                .Select(eh => new SelectListItem
                {
                    Value = eh.IdEmpleadoHorario.ToString(),
                    Text = eh.IdEmpleadoNavigation.IdPersonaNavigation.Nombres + " " +
                           eh.IdEmpleadoNavigation.IdPersonaNavigation.Apellidos
                })
                .ToList();

            ViewData["Servicios"] = new SelectList(
                _context.Servicios.AsNoTracking().ToList(),
                "IdServicio",
                "Nombre",
                idServicio
            );
        }
    }
}
