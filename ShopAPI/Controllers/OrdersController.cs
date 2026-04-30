using Microsoft.AspNetCore.Mvc;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { message = "Orders API" });
        }
    }
}
