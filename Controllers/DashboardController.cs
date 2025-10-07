using Microsoft.AspNetCore.Mvc;

namespace Pry_Solu_SalonSPA.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
