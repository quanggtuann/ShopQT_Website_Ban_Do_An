using Microsoft.AspNetCore.Mvc;
using ShopDAL.Areas.Models;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;
namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAdminAccountRepo _adminAccountRepo;
        public AccountsController(IAdminAccountRepo adminAccountRepo)
        {
            _adminAccountRepo = adminAccountRepo;
        }
        [HttpGet]
        public IActionResult GetAccounts(
            [FromQuery] string? keyword = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? role = null,
            [FromQuery] string? sortBy = "id",
            [FromQuery] string? sortOrder = "asc")
        {
            var accounts = _adminAccountRepo.GetFiltered(keyword, isActive, role, sortBy, sortOrder);
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public IActionResult GetAccount(int id)
        {
            var account = _adminAccountRepo.GetById(id);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        [HttpPost]
        public IActionResult CreateAccount([FromBody] User user)
        {
            try
            {
                _adminAccountRepo.Add(user);
                return CreatedAtAction(nameof(GetAccount), new { id = user.UserID }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id, [FromBody] User user)
        {
            if (id != user.UserID)
                return BadRequest(new { message = "ID mismatch" });

            try
            {
                _adminAccountRepo.Update(user);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id}/deactivate")]
        public IActionResult DeactivateAccount(int id)
        {
            _adminAccountRepo.Deactive(id);
            return NoContent();
        }
        [HttpPost("{id}/activate")]
        public IActionResult ActivateAccount(int id)
        {
            _adminAccountRepo.ActiveUser(id);
            return NoContent();
        }
    }

}



