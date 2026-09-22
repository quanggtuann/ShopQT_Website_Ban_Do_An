using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Services.IServices;

namespace ShopAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountCampaignsController : ControllerBase
    {
        private readonly IDiscountCampaignService _discountCampaignService;

        public DiscountCampaignsController(IDiscountCampaignService discountCampaignService)
        {
            _discountCampaignService = discountCampaignService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] DiscountCampaignFilterViewModel filter)
        {
            try
            {
                return Ok(_discountCampaignService.GetAll(filter));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(_discountCampaignService.GetById(id));
            }
            catch (Exception ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateDiscountCampaignRequest request)
        {
            try
            {
                var result = _discountCampaignService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = result.DiscountCampaignId }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateDiscountCampaignRequest request)
        {
            try
            {
                return Ok(_discountCampaignService.Update(id, request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/activate")]
        public IActionResult Activate(int id)
        {
            try
            {
                _discountCampaignService.Activate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            try
            {
                _discountCampaignService.Deactivate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
    }
}
