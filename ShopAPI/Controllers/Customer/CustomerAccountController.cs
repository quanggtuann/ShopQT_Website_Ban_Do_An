using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;

namespace ShopAPI.Controllers
{
    [Route("api/customer/account")]
    [ApiController]
    public class CustomerAccountController : ControllerBase
    {
        private readonly ICustomerAccountService _accountService;

        public CustomerAccountController(ICustomerAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                var userId = _accountService.Register(user);

                return Ok(new
                {
                    success = true,
                    message = "Registration successful",
                    userId
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

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = _accountService.Login(request);

                return Ok(result);
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

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetProfile(int id)
        {
            if (!CanAccessUser(id))
                return Forbid();

            try
            {
                var user = _accountService.GetProfile(id);

                return Ok(new
                {
                    user.UserID,
                    user.Username,
                    user.Email,
                    user.PhoneNumber,
                    user.DateorBirth,
                    user.Role,
                    user.IsActive
                });
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateProfile(int id, [FromBody] User user)
        {
            if (id != user.UserID)
                return BadRequest("ID mismatch");

            if (!CanAccessUser(id))
                return Forbid();

            try
            {
                _accountService.UpdateProfile(user);

                return Ok(new
                {
                    success = true,
                    message = "Profile updated"
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

        private bool CanAccessUser(int id)
        {
            var userIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            )?.Value;

            if (!int.TryParse(userIdClaim, out var currentUserId))
                return false;

            if (currentUserId == id)
                return true;

            return User.IsInRole("admin");
        }
    }
}