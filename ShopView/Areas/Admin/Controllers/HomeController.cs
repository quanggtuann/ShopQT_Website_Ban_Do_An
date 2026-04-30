using Microsoft.AspNetCore.Mvc;

namespace ShopView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "admin")
            {
                ViewData["ErrorMessage"] = "You do not have admin permission to access this page.";
                return View("Error", "Shared");
            }

            return View();
        }
    }
}
