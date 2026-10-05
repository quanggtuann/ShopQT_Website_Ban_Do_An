using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using System.Security.Claims;

namespace ShopAPI.Controllers.Customer
{
    [Route("api/customer/orders")]
    [ApiController]
    [Authorize]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly ICustomerOrderService _customerOrderService;

        public CustomerOrdersController(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        [HttpGet]
        public IActionResult GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 6)
        {
            try
            {
                var userId = GetCurrentUserId();
                return Ok(_customerOrderService.GetOrders(userId, page, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{orderId}")]
        public IActionResult GetOrderDetail(int orderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = _customerOrderService.GetOrderDetail(userId, orderId);
                if (order == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Order not found"
                    });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = _customerOrderService.CreateOrderFromCart(userId, request);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPatch("{orderId}/cancel")]
        public IActionResult CancelOrder(int orderId, [FromBody] CancelOrderRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                _customerOrderService.CancelOrder(userId, orderId, request);
                return Ok(new
                {
                    success = true,
                    message = "Order cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }

            return userId;
        }
    }
}
