using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pry_Solu_SalonSPA.Db;
using Pry_Solu_SalonSPA.Models;
using System.Collections.Generic;
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

        // ===========================
        // INDEX: Lista de clientes + modal activo
        // ===========================
        public IActionResult Index(string filtro, int? editarId, int? detalleId)
        {
            var clientes = _context.Clientes
                .Include(c => c.IdPersonaNavigation)
                .Where(c =>
                    string.IsNullOrEmpty(filtro) ||
                    c.IdPersonaNavigation.Nombres.Contains(filtro) ||
                    c.IdPersonaNavigation.Dni.Contains(filtro))
                .ToList();

            return View(clientes);
        }

        // ===========================
        // CREAR: GET y POST
        // ===========================
        public IActionResult Crear()
        {
            var persona = new Persona();

            return View("_CrearClientes", persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Persona persona)
        {
            if (ModelState.IsValid)
            {
                persona.Estado = 1;

                var parametros = new[]
                {
                    new SqlParameter("@NombresPersona", persona.Nombres),
                    new SqlParameter("@ApellidosPersona", persona.Apellidos),
                    new SqlParameter("@TelefonoPersona", persona.Telefono),
                    new SqlParameter("@DniPersona", persona.Dni),
                    new SqlParameter("@Fecha_nacimientoPersona", persona.FechaNacimiento),
                    new SqlParameter("@GeneroPersona", persona.Genero),
                    new SqlParameter("@Estado_Persona", persona.Estado)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_Registrar_Cliente @NombresPersona, @ApellidosPersona, @TelefonoPersona, @DniPersona, @Fecha_nacimientoPersona, @GeneroPersona, @Estado_Persona",
                    parametros);

                TempData["Mensaje"] = "Cliente registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(persona);
        }

        // ===========================
        // EDITAR: GET y POST
        // ===========================
        public async Task<IActionResult> Editar(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.IdPersonaNavigation)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            ViewBag.Generos = new List<SelectListItem>
            {
                new SelectListItem { Text = "Masculino", Value = "Masculino" },
                new SelectListItem { Text = "Femenino", Value = "Femenino" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            return View("_EditarClientes", cliente.IdPersonaNavigation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Persona persona)
        {
            if (ModelState.IsValid)
            {
                var parametros = new[]
                {
                    new SqlParameter("@Id_Cliente", id),
                    new SqlParameter("@NombresPersona", persona.Nombres),
                    new SqlParameter("@ApellidosPersona", persona.Apellidos),
                    new SqlParameter("@TelefonoPersona", persona.Telefono),
                    new SqlParameter("@DniPersona", persona.Dni),
                    new SqlParameter("@Fecha_nacimientoPersona", persona.FechaNacimiento),
                    new SqlParameter("@GeneroPersona", persona.Genero),
                    new SqlParameter("@Estado_Persona", persona.Estado)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_Modificar_Cliente @Id_Cliente, @NombresPersona, @ApellidosPersona, @TelefonoPersona, @DniPersona, @Fecha_nacimientoPersona, @GeneroPersona, @Estado_Persona",
                    parametros);

                TempData["Mensaje"] = "Cliente actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View("Editar", persona);
        }

        // ===========================
        // ELIMINAR: cambiar estado
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Obtener estado actual
            var cliente = await _context.Clientes
                                .Include(c => c.IdPersonaNavigation)
                                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                TempData["Error"] = "Cliente no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            int nuevoEstado = cliente.IdPersonaNavigation.Estado == 1 ? 0 : 1;

            var parametros = new[]
            {
            new SqlParameter("@Id_Cliente", id),
            new SqlParameter("@NuevoEstado", nuevoEstado)
        };

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_Eliminar_Cliente @Id_Cliente, @NuevoEstado", parametros);

            TempData["Mensaje"] = nuevoEstado == 1 ? "Cliente activado correctamente." : "Cliente inactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // BUSCAR: por DNI
        // ===========================
        [HttpGet]
        public IActionResult Buscar(string dni)
        {
            var clientes = _context.Clientes
                .Include(c => c.IdPersonaNavigation)
                .ToList();

            if (!string.IsNullOrEmpty(dni))
            {
                clientes = clientes
                    .Where(c => c.IdPersonaNavigation.Dni.Contains(dni))
                    .ToList();
            }

            if (!clientes.Any())
                TempData["Error"] = "No se encontraron clientes con ese DNI.";

            return View("Index", clientes);
        }

    }
}
