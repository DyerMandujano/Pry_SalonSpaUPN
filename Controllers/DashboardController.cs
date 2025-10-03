using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Rumis_Salon_Spa.Models;

namespace Rumis_Salon_Spa.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Clientes"] = 120;
            ViewData["Empleados"] = 25;
            ViewData["Inventario"] = 350;
            ViewData["Ventas"] = 12500;

            return View("Dashboard");
        }
    }
}
