using Microsoft.AspNetCore.Mvc;
using Rumis_Salon_Spa.Models;

namespace Rumis_Salon_Spa.Controllers
{
    public class ComprasController : Controller
    {
        public IActionResult Index()
        {
            //new Cliente { Id = 1, Dni = "12345678", Nombres = "Juan", Apellidos = "Pérez", Correo = "juan@mail.com", Telefono = "987654321" },

            return View("Compras");
        }
    }
}
