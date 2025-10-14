using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Db;
using Pry_Solu_SalonSPA.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Pry_Solu_SalonSPA.Controllers
{
    public class CitasController : Controller
    {
        private readonly Conexion _context;

        public CitasController(Conexion context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filtro, string busqueda, int? idDetalle)
        {
            var hoy = DateTime.Today;

            var citas = await _context.Cita
                .Include(c => c.IdClienteNavigation)
                    .ThenInclude(cl => cl.IdPersonaNavigation)
                .Include(c => c.IdEmpleadoHorarioNavigation)
                    .ThenInclude(eh => eh.IdEmpleadoNavigation)
                        .ThenInclude(e => e.IdPersonaNavigation)
                .Include(c => c.CitaServicios)
                    .ThenInclude(cs => cs.IdServicioNavigation)
                .ToListAsync();

            if (!string.IsNullOrEmpty(busqueda))
            {
                citas = citas.Where(c =>
                    c.IdClienteNavigation.IdPersonaNavigation.Dni.Contains(busqueda)
                ).ToList();
            }

            switch (filtro)
            {
                case "hoy":
                    citas = citas.Where(c => c.FechaCita.Date == hoy).ToList();
                    ViewBag.TituloFiltro = "Citas de Hoy";
                    break;

                case "semana":
                    DateTime inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime finSemana = inicioSemana.AddDays(6);
                    citas = citas.Where(c => c.FechaCita.Date >= inicioSemana && c.FechaCita.Date <= finSemana).ToList();
                    ViewBag.TituloFiltro = "Citas de la Semana";
                    break;

                default:
                    ViewBag.TituloFiltro = "Todas las Citas";
                    filtro = "general";
                    break;
            }

            ViewBag.IdDetalle = idDetalle;
            ViewBag.FiltroActual = filtro;
            ViewBag.TotalCitas = citas.Count();
            ViewBag.Completadas = citas.Count(c => c.CitaServicios.Any(cs => cs.Estado == 1));
            ViewBag.Pendientes = citas.Count(c => c.CitaServicios.Any(cs => cs.Estado == 2));
            ViewBag.Canceladas = citas.Count(c => c.CitaServicios.Any(cs => cs.Estado == 3));

            return View(citas);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            CargarListasDesplegables();

            return View("_CrearCitas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int IdCliente, int IdEmpleadoHorario, DateTime FechaCita, int IdServicio,
                                                string ObsServicio, int Estado_Servicio)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_CrearCita", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Cliente", IdCliente);
                command.Parameters.AddWithValue("@Id_Empleado_Horario", IdEmpleadoHorario);
                command.Parameters.AddWithValue("@Fecha_Cita", FechaCita);
                command.Parameters.AddWithValue("@Id_Servicio", IdServicio);
                command.Parameters.AddWithValue("@Obs_Servicio", (object?)ObsServicio ?? DBNull.Value);
                command.Parameters.AddWithValue("@Estado_Servicio", Estado_Servicio);

                await command.ExecuteNonQueryAsync();

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
        public async Task<IActionResult> BuscarClientePorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                ViewBag.Error = "Debe ingresar un DNI.";
                CargarListasDesplegables();
                return View("_CrearCitas");
            }

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_BuscarClientePorDNI", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@DNI", dni);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    ViewBag.ClienteEncontrado = new
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("Id_Cliente")),
                        Nombre = reader["Nombres"].ToString() + " " + reader["Apellidos"].ToString()
                    };
                }
                else
                {
                    ViewBag.Error = "No se encontró un cliente con ese DNI.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error en la búsqueda: " + ex.Message;
            }

            CargarListasDesplegables();
            return View("_CrearCitas");
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id, string filtro)
        {
            var cita = await _context.Cita
                .Include(c => c.CitaServicios)
                .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
                return NotFound();

            var servicio = cita.CitaServicios.FirstOrDefault()?.IdServicio;

            CargarListasDesplegables(cita.IdCliente, cita.IdEmpleadoHorario, servicio);

            ViewBag.FiltroActual = filtro;
            return View("_EditarCitas", cita);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int IdCita, int IdCliente, int IdEmpleadoHorario, DateTime FechaCita,
                                                    int IdServicio, string ObsServicio, int Estado_Servicio)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_ModificarCita", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id_Cita", IdCita);
                command.Parameters.AddWithValue("@Id_Cliente", IdCliente);
                command.Parameters.AddWithValue("@Id_Empleado_Horario", IdEmpleadoHorario);
                command.Parameters.AddWithValue("@Fecha_Cita", FechaCita);
                command.Parameters.AddWithValue("@Id_Servicio", IdServicio);
                command.Parameters.AddWithValue("@Obs_Servicio", string.IsNullOrEmpty(ObsServicio) ? "" : ObsServicio);
                command.Parameters.AddWithValue("@Estado_Servicio", Estado_Servicio);

                await command.ExecuteNonQueryAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al modificar la cita: {ex.Message}";
            }

            CargarListasDesplegables(IdCliente, IdEmpleadoHorario, IdServicio);

            return View("_EditarCitas");
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Buscar(string busqueda)
        {
            if (string.IsNullOrEmpty(busqueda))
            {
                TempData["Mensaje"] = "Ingrese un DNI válido.";
                return RedirectToAction("Index");
            }

            var listaCitas = new List<Cita>();

            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_BuscarCitaPorDNI", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@DNI", busqueda);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var cita = new Cita
                    {
                        IdCita = reader.GetInt32(reader.GetOrdinal("Id_Cita")),
                        FechaCita = reader.GetDateTime(reader.GetOrdinal("Fecha_Cita")),
                        IdClienteNavigation = new Cliente
                        {
                            IdPersonaNavigation = new Persona
                            {
                                Nombres = reader["Cliente"].ToString()
                            }
                        },
                        IdEmpleadoHorarioNavigation = new EmpleadoHorario
                        {
                            IdEmpleadoNavigation = new Empleado
                            {
                                IdPersonaNavigation = new Persona
                                {
                                    Nombres = reader["Empleado"].ToString()
                                }
                            }
                        }
                    };

                    var citaServicio = new CitaServicio
                    {
                        IdServicioNavigation = new Servicio
                        {
                            Nombre = reader["Servicio"].ToString(),
                            Precio = reader.GetDecimal(reader.GetOrdinal("Precio"))
                        },
                        Observacion = reader["Observacion_Servicio"].ToString(),
                        Estado = Convert.ToInt32(reader["Estado_Servicio"])
                    };

                    cita.CitaServicios.Add(citaServicio);
                    listaCitas.Add(cita);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al buscar la cita: {ex.Message}";
                return RedirectToAction("Index");
            }

            if (!listaCitas.Any())
            {
                TempData["Mensaje"] = "No se encontraron citas para el DNI ingresado.";
            }

            return View("Index", listaCitas);
        }



        private void CargarListasDesplegables(int? idCliente = null, int? idEmpleadoHorario = null, int? idServicio = null)
        {
            ViewData["Clientes"] = new SelectList(
                _context.Clientes.Include(c => c.IdPersonaNavigation),
                "IdCliente",
                "IdPersonaNavigation.Nombres",
                idCliente
            );

            ViewData["Empleados"] = _context.EmpleadoHorarios
                .Include(eh => eh.IdEmpleadoNavigation)
                .ThenInclude(e => e.IdPersonaNavigation)
                .Select(eh => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = eh.IdEmpleadoHorario.ToString(),
                    Text = eh.IdEmpleadoNavigation.IdPersonaNavigation.Nombres + " " +
                           eh.IdEmpleadoNavigation.IdPersonaNavigation.Apellidos
                })
                .ToList();

            ViewData["Servicios"] = new SelectList(
                _context.Servicios,
                "IdServicio",
                "Nombre",
                idServicio
            );
        }

    }
}
