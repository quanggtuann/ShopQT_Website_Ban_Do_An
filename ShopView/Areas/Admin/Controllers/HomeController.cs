using Microsoft.AspNetCore.Mvc;

namespace ShopView.Areas.Admin.Controllers
{
    public class HomeController : AdminControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
