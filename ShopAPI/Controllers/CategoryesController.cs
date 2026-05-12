using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Services.IServices;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryesController : ControllerBase
    {
        private readonly ICategoryesService _categoryService;

        public CategoryesController(ICategoryesService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] bool? activeOnly = null)
        {
            try
            {
                var categories = _categoryService.GetAll(activeOnly);
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
            try
            {
                var category = _categoryService.GetById(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { ErrorMessage = ex.Message })
                    : StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var result = _categoryService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = result.CategoryId }, result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists") || ex.Message.Contains("required"))
                    return BadRequest(new { ErrorMessage = ex.Message });
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var result = _categoryService.Update(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(new { ErrorMessage = ex.Message });
                if (ex.Message.Contains("already exists") || ex.Message.Contains("required"))
                    return BadRequest(new { ErrorMessage = ex.Message });
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            try
            {
                _categoryService.Deactivate(id);
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
        public IActionResult Activate(int id)
        {
            try
            {
                _categoryService.Activate(id);
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
