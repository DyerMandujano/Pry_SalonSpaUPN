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

        public async Task<IActionResult> Index(string filtro, int? idDetalle)
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

            ViewBag.IdDetalle = idDetalle;
            ViewBag.FiltroActual = filtro;

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

            ViewBag.TotalCitas = citas.Count();
            ViewBag.Completadas = citas.Count(c => c.CitaServicios.Any(cs => cs.Estado == 1));
            ViewBag.Pendientes = citas.Count(c => c.CitaServicios.Any(cs => cs.Estado == 2));
            ViewBag.Canceladas = citas.Count(c => c.CitaServicios.Any(cs => cs.Estado == 3));

            return View(citas);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            ViewData["Clientes"] = new SelectList(
                _context.Clientes.Include(c => c.IdPersonaNavigation),
                "IdCliente",
                "IdPersonaNavigation.Nombres"
            );

            ViewData["Empleados"] = new SelectList(
                _context.EmpleadoHorarios
                    .Include(eh => eh.IdEmpleadoNavigation)
                    .ThenInclude(e => e.IdPersonaNavigation),
                "IdEmpleadoHorario",
                "IdEmpleadoNavigation.IdPersonaNavigation.Nombres"
            );

            ViewData["Servicios"] = new SelectList(
                _context.Servicios,
                "IdServicio",
                "Nombre"
            );

            return View("_CrearCitas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            int IdCliente,
            int IdEmpleadoHorario,
            DateTime FechaCita,
            int IdServicio,
            string ObsServicio,
            int Estado_Servicio
        )
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

            ViewData["Clientes"] = new SelectList(
                _context.Clientes.Include(c => c.IdPersonaNavigation),
                "IdCliente",
                "IdPersonaNavigation.Nombres"
            );

            ViewData["Empleados"] = new SelectList(
                _context.EmpleadoHorarios
                    .Include(eh => eh.IdEmpleadoNavigation)
                    .ThenInclude(e => e.IdPersonaNavigation),
                "IdEmpleadoHorario",
                "IdEmpleadoNavigation.IdPersonaNavigation.Nombres"
            );

            ViewData["Servicios"] = new SelectList(
                _context.Servicios,
                "IdServicio",
                "Nombre"
            );

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

            ViewData["Clientes"] = new SelectList(
                _context.Clientes.Include(c => c.IdPersonaNavigation),
                "IdCliente",
                "IdPersonaNavigation.Nombres",
                cita.IdCliente
            );

            ViewData["Empleados"] = new SelectList(
                _context.EmpleadoHorarios
                    .Include(eh => eh.IdEmpleadoNavigation)
                    .ThenInclude(e => e.IdPersonaNavigation),
                "IdEmpleadoHorario",
                "IdEmpleadoNavigation.IdPersonaNavigation.Nombres",
                cita.IdEmpleadoHorario
            );

            ViewData["Servicios"] = new SelectList(
                _context.Servicios,
                "IdServicio",
                "Nombre",
                cita.CitaServicios.FirstOrDefault()?.IdServicio
            );

            // 👇 Esto mantiene el filtro actual (hoy, semana o general)
            ViewBag.FiltroActual = filtro;

            return View("_EditarCitas", cita);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int IdCita,
            int IdCliente,
            int IdEmpleadoHorario,
            DateTime FechaCita,
            int IdServicio,
            string ObsServicio,
            int Estado_Servicio
        )
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_ModificarCita", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                // Asignar parámetros
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

            ViewData["Clientes"] = new SelectList(
                _context.Clientes.Include(c => c.IdPersonaNavigation),
                "IdCliente",
                "IdPersonaNavigation.Nombres",
                IdCliente
            );

            ViewData["Empleados"] = new SelectList(
                _context.EmpleadoHorarios
                    .Include(eh => eh.IdEmpleadoNavigation)
                    .ThenInclude(e => e.IdPersonaNavigation),
                "IdEmpleadoHorario",
                "IdEmpleadoNavigation.IdPersonaNavigation.Nombres",
                IdEmpleadoHorario
            );

            ViewData["Servicios"] = new SelectList(
                _context.Servicios,
                "IdServicio",
                "Nombre",
                IdServicio
            );

            return View("_EditarCitas");
        }
    }
}
