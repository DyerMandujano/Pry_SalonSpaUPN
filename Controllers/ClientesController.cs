using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rumis_Salon_Spa.DB;
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

        public IActionResult Index()
        {
            var clientes = _context.Clientes
                .Include(c => c.IdPersonaNavigation)
                .ToList();

            return View("Clientes",clientes);
        }
    }
}
