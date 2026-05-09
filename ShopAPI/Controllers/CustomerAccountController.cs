using Microsoft.AspNetCore.Mvc;
using ShopDAL.Models;
using ShopAPI.DTOs;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Controllers
{
    [Route("api/customer/account")]
    [ApiController]
    public class CustomerAccountController : ControllerBase
    {
        private readonly IAccountRepo _accountRepo;

        public CustomerAccountController(IAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                user.Role = "customer";
                user.IsActive = true;
                var result = _accountRepo.Register(user);
                return Ok(new { success = true, message = "Registration successful", userId = user.UserID });
            }
            catch (Exception ex)
            {
              return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = _accountRepo.Login(request.Username, request.Password);
                var user = _accountRepo.Getnameuser(request.Username);
                return Ok(new
                {
                    success = true,
                    userId = user.UserID,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProfile(int id)
        {
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

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(int id, [FromBody] User user)
        {
            if (id != user.UserID)
                return BadRequest("ID mismatch");

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
    }
}
