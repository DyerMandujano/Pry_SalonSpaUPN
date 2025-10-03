using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace Rumis_Salon_Spa.Views
{
    public class RedirectToDashboardModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirige a /Dashboard
            return RedirectToPage("/Dashboard");
        }
    }
}
