using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;
using System.Security.Claims;

namespace ShopAPI.Controllers.Customer
{
    [Route("api/customer/cart")]
    [ApiController]
    [Authorize]
    public class CustomerCartsController : ControllerBase
    {
        private readonly ICustomerCartService _customerCartService;

        public CustomerCartsController(ICustomerCartService customerCartService)
        {
            _customerCartService = customerCartService;
        }
        [HttpGet]
        public ActionResult GetMyCart()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var cart = _customerCartService.GetByUserId(userId);
                if (cart == null)
                {
                    return Ok(new Cart
                    {
                        UserID = userId,
                        CartItems = new List<CartItem>()
                    });
                }

                cart.CartItems ??= new List<CartItem>();
                return Ok(cart);
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
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid token");
            }

            return userId;
        }
        [HttpPost("add-food")]
        public IActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                request.UserId = GetUserIdFromToken();
                _customerCartService.AddToCart(request);
                return Ok(new
                {
                    success = true,
                    message = "Add to cart success"
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
        [HttpPost("add-combo")]
        public IActionResult AddComboToCart([FromBody] AddComboToCartRequest request)
        {
            try
            {
                request.UserId = GetUserIdFromToken();
                _customerCartService.AddComboToCart(request);
                return Ok(new
                {
                    success = true,
                    message = "Add to cart success"
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
        [HttpDelete("{CartItemId}")]
        public IActionResult Delete(int CartItemId)
        {
            try
            {
                _customerCartService.RemoveCartItem(CartItemId);
                return Ok(new
                {
                    success = true,
                    message = "Remove success"
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
        [HttpPut("update")]
        public IActionResult UpdateCartItem([FromBody] UpdateCartItem updateCartItem)
        {
            try
            {
                _customerCartService.Update(updateCartItem);
                return Ok(new
                {
                    success = true,
                    message = "Update success"
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
    }
}
