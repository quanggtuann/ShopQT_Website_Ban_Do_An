using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Services.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Controllers
{
    [Route("api/customer/account")]
    [ApiController]
    public class CustomerAccountController : ControllerBase
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IJwtTokenService _jwtTokenService;

        public CustomerAccountController(IAccountRepo accountRepo, IJwtTokenService jwtTokenService)
        {
            _accountRepo = accountRepo;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                user.Role = "customer";
                user.IsActive = true;
                _accountRepo.Register(user);
                return Ok(new { success = true, message = "Registration successful", userId = user.UserID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                _accountRepo.Login(request.Username, request.Password);
                var user = _accountRepo.Getnameuser(request.Username);
                if (user == null)
                    return BadRequest(new { success = false, message = "User not found" });

                var token = _jwtTokenService.GenerateToken(user);

                return Ok(new LoginResponseDto
                {
                    Success = true,
                    UserId = user.UserID,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetProfile(int id)
        {
            if (!CanAccessUser(id))
                return Forbid();

            var user = _accountRepo.Getnameuser(id.ToString());
            if (user == null)
                return NotFound();

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
                _accountRepo.Update(user);
                return Ok(new { success = true, message = "Profile updated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private bool CanAccessUser(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var currentUserId))
                return false;

            if (currentUserId == id)
                return true;

            return User.IsInRole("admin");
        }
    }
}
