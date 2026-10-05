using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.Services.IServices;

namespace ShopAPI.Controllers.Admin
{
    [Route("api/admin/statistics")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly IAdminStatisticsService _statisticsService;

        public StatisticsController(IAdminStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public IActionResult GetStatistics([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                return Ok(_statisticsService.GetStatistics(fromDate, toDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }
    }
}
