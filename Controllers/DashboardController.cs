using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Rumis_Salon_Spa.Models;

namespace Rumis_Salon_Spa.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public DashboardController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Acción principal -> /Dashboard/Index
        public async Task<IActionResult> Index()
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Data", "dashboard.json");

            DashboardModel datos = new();

            if (System.IO.File.Exists(filePath))
            {
                var json = await System.IO.File.ReadAllTextAsync(filePath);
                datos = JsonSerializer.Deserialize<DashboardModel>(json) ?? new DashboardModel();
            }

            // Enviamos el modelo a la Vista
            return View(datos);
        }
    }
}
