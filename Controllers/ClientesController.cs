using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rumis_Salon_Spa.DB;
using Rumis_Salon_Spa.Models;
using Rumis_Salon_Spa.Models.ViewModels;
using System.Linq;

namespace Rumis_Salon_Spa.Controllers
{
    public class ClientesController : Controller
    {
        private readonly Conexion _context;

        public ClientesController(Conexion context)
        {
            _context = context;
        }

        public IActionResult Index(int? clienteId)
        {
            var clientes = _context.Clientes
                .Include(c => c.IdPersonaNavigation)
                .ToList();

            Clientes clienteSeleccionado = null;
            if (clienteId.HasValue)
            {
                clienteSeleccionado = _context.Clientes
                    .Include(c => c.IdPersonaNavigation)
                    .FirstOrDefault(c => c.IdCliente == clienteId.Value);
            }

            var model = new ClientesViewModel
            {
                Clientes = clientes,
                ClienteSeleccionado = clienteSeleccionado
            };

            return View("Clientes", model);
        }

        [Route("Clientes/Detalles/{id}/{nombre}")]
        public IActionResult Detalles(int id, string nombre)
        {
            var cliente = _context.Clientes
                .Include(c => c.IdPersonaNavigation)
                .FirstOrDefault(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ModalDetalles", cliente);
            }

            var clientes = _context.Clientes
                .Include(c => c.IdPersonaNavigation)
                .ToList();

            var model = new ClientesViewModel
            {
                Clientes = clientes,
                ClienteSeleccionado = cliente
            };

            return View("Clientes", model);
        }

        [Route("Clientes/Editar/{id}/{nombre}")]
        public IActionResult Editar(int id, string nombre)
        {
            var cliente = _context.Clientes
                .Include(c => c.IdPersonaNavigation)
                .FirstOrDefault(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Clientes cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            _context.Clientes.Update(cliente);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Clientes/Eliminar/{id}/{nombre}")]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id, string nombre)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
