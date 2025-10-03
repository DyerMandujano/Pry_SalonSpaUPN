using Microsoft.AspNetCore.Mvc;
using Rumis_Salon_Spa.Models;

namespace Rumis_Salon_Spa.Controllers
{
    public class TurnosController : Controller
    {
        public IActionResult Index()
        {

            return View("Turnos");
        }
    }
}
