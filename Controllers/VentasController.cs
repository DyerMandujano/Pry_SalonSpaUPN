using Microsoft.AspNetCore.Mvc;
using Rumis_Salon_Spa.Models;

namespace Rumis_Salon_Spa.Controllers
{
    public class VentasController : Controller
    {
        public IActionResult Index()
        {

            return View("Ventas");
        }
    }
}
