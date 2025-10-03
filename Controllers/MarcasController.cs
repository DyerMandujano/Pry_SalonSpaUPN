using Microsoft.AspNetCore.Mvc;
using Rumis_Salon_Spa;

namespace Rumis_Salon_Spa.Controllers
{
    public class MarcasController : Controller
    {
        public IActionResult Index()
        {

            return View("Marcas");
        }
    }
}
