using Microsoft.AspNetCore.Mvc;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryesController : ControllerBase
    {
        private readonly IAdminCategoryRepo _categoryRepo;

        public CategoryesController(IAdminCategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] bool? activeOnly = null)
        {
            try
            {
                var categories = _categoryRepo.GetAll();
                
                if (activeOnly == true)
                {
                    categories = categories.Where(c => c.IsActive).ToList();
                }
                
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
            {
                return NotFound(new { ErrorMessage = "Category not found" });
            }
            return Ok(category);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Category category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    return BadRequest(new { ErrorMessage = "Category name is required" });
                }

                _categoryRepo.Add(category);
                return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Category category)
        {
            try
            {
                var existingCategory = _categoryRepo.GetById(id);
                if (existingCategory == null)
                {
                    return NotFound(new { ErrorMessage = "Category not found" });
                }

                category.CategoryId = id;
                _categoryRepo.Update(category);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            try
            {
                _categoryRepo.Deactive(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/activate")]
        public IActionResult Activate(int id)
        {
            try
            {
                _categoryRepo.Activate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
    }
}
