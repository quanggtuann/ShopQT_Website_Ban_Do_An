using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.Services.IServices;
using ShopDAL.Models;

namespace ShopAPI.Controllers.Customer
{
    [Route("api/customer/foods")]
    [ApiController]
    public class CustomerFoodsController : ControllerBase
    {
        private readonly ICustomerFoodService _customerFoodService;

        public CustomerFoodsController(ICustomerFoodService customerFoodService)
        {
            _customerFoodService = customerFoodService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll([FromQuery] FoodItemFilterViewModel filter)
        {
            try
            {
                var result = _customerFoodService.GetAll(filter);
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
                var food = _customerFoodService.GetById(id);
                return Ok(food);
            }
            catch (Exception ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
        }
    }
}
