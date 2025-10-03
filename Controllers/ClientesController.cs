using Microsoft.AspNetCore.Mvc;
using Rumis_Salon_Spa.Models;

namespace Rumis_Salon_Spa.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            var clientes = new List<Clientes>
        {
            new Clientes { Id=1, Dni="12345678", Nombres="Juan", Apellidos="Pérez", Correo="juan@mail.com", Telefono="987654321" },
            new Clientes { Id=2, Dni="87654321", Nombres="María", Apellidos="López", Correo="maria@mail.com", Telefono="912345678" }
        };
            return View("Clientes", clientes);
        }
    }
}
