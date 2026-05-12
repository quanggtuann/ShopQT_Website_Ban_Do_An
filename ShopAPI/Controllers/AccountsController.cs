using Microsoft.AspNetCore.Mvc;
using ShopAPI.Services.IServices;
using ShopDAL.Models;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetAccounts([FromQuery] AccountFilterViewModel filter)
        {
            try
            {
                var result = _accountService.GetAllPaged(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAccount(int id)
        {
            try
            {
                var account = _accountService.GetAccount(id);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { ErrorMessage = ex.Message })
                    : StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateAccount([FromBody] User user)
        {
            try
            {
                var result = _accountService.CreateAccount(user);
                return CreatedAtAction(nameof(GetAccount), new { id = result.UserID }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id, [FromBody] User user)
        {
            try
            {
                var result = _accountService.UpdateAccount(id, user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(new { ErrorMessage = ex.Message });
                if (ex.Message.Contains("ID mismatch"))
                    return BadRequest(new { ErrorMessage = ex.Message });
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/deactivate")]
        public IActionResult DeactivateAccount(int id)
        {
            try
            {
                _accountService.DeactivateAccount(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { ErrorMessage = ex.Message })
                    : StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/activate")]
        public IActionResult ActivateAccount(int id)
        {
            try
            {
                _accountService.ActivateAccount(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { ErrorMessage = ex.Message })
                    : StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
    }
}
