using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;

namespace ShopAPI.Controllers.Customer
{
    [Route("api/customer/combos")]
    [ApiController]
    public class CustomerComboController : ControllerBase
    {
        private readonly ICustomerComboService _comboService;

        public CustomerComboController(ICustomerComboService comboService)
        {
            _comboService = comboService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll([FromQuery] ComboFilterViewmodel filter)
        {
            try
            {
                var result = _comboService.Getall(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var combo = _comboService.GetById(id);
                return Ok(combo);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { ErrorMessage = ex.Message });

                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
    }
}
