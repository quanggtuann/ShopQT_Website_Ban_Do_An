using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Services.IServices;
using ShopDAL.Models;

namespace ShopAPI.Controllers.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] OrderFilterViewModel filter)
        {
            try
            {
                return Ok(_orderService.GetAll(filter));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(_orderService.GetById(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/next-status")]
        public IActionResult UpdateNextStatus(int id)
        {
            try
            {
                _orderService.UpdateNextStatus(id);
                return Ok(new { success = true, message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/cancel")]
        public IActionResult Cancel(int id, [FromBody] CancelOrderRequest request)
        {
            try
            {
                _orderService.CancelOrder(id, request);
                return Ok(new { success = true, message = "Order cancelled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, errorMessage = ex.Message });
            }
        }
    }
}
