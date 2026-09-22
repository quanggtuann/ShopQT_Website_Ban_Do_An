using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.Services.Customer.IServices;
using System.Security.Claims;

namespace ShopAPI.Controllers.Customer
{
    [Route("api/customer/favorites")]
    [ApiController]
    [Authorize(Roles = "customer")]
    public class CustomerFavoritesController : ControllerBase
    {
        private readonly ICustomerFavoriteService _favoriteService;

        public CustomerFavoritesController(ICustomerFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public IActionResult GetMyFavorites()
        {
            try
            {
                return Ok(_favoriteService.GetByUserId(GetUserIdFromToken()));
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("food/{foodId}/status")]
        public IActionResult IsFavorite(int foodId)
        {
            return Ok(new { isFavorite = _favoriteService.IsFavorite(GetUserIdFromToken(), foodId) });
        }

        [HttpPost("food/{foodId}/toggle")]
        public IActionResult ToggleFood(int foodId)
        {
            try
            {
                return Ok(_favoriteService.ToggleFood(GetUserIdFromToken(), foodId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("food/{foodId}")]
        public IActionResult RemoveFood(int foodId)
        {
            try
            {
                _favoriteService.RemoveFood(GetUserIdFromToken(), foodId);
                return Ok(new { success = true, message = "Removed from favorites" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            return userId;
        }
    }
}
