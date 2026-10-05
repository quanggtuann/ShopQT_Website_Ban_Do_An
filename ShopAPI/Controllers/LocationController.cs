using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.Services;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;
        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var data = await _locationService.GetAll();
            return Ok(data);
        }
    }
}
