using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.Services;
using ShopAPI.Services.IServices;
using ShopDAL.Models;
using ShopAPI.DTOs;

namespace ShopAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodsController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet("all")]
        public IActionResult GetAllForDropdown()
        {
            try
            {
                var foods = _foodService.GetAllForDropdown();
                return Ok(foods);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] FoodItemFilterViewModel filter)
        {
            try
            {
                var result = _foodService.GetAll(filter);
                return Ok(result);
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
                var food = _foodService.GetById(id);
                return Ok(food);
            }
            catch (Exception ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromForm] FoodItem food, IFormFile? imageFile)
        {
            if (_foodService.CheckDuplicateName(food.Name))
            {
                return BadRequest(new { ErrorMessage = $"Food with name '{food.Name}' already exists." });
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                food.ImagePath = SaveImage(imageFile, food.Name);
            }

            try
            {
                var result = _foodService.Create(food);
                return CreatedAtAction(nameof(GetById), new { id = result.FoodItemId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] FoodItem food, IFormFile? imageFile)
        {
            var currentFood = _foodService.GetById(id);
            if (currentFood == null)
            {
                return NotFound(new { ErrorMessage = "Food not found" });
            }

            if (_foodService.CheckDuplicateName(food.Name, id))
            {
                return BadRequest(new { ErrorMessage = $"Food with name '{food.Name}' already exists." });
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(currentFood.ImagePath))
                {
                    DeleteImage(currentFood.ImagePath);
                }
                food.ImagePath = SaveImage(imageFile, food.Name);
            }
            else
            {
                food.ImagePath = currentFood.ImagePath;
            }

            try
            {
                var result = _foodService.Update(id, food);
                return Ok(result);
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
                _foodService.Deactivate(id);
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
                _foodService.Activate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        private string SaveImage(IFormFile imageFile, string foodName)
        {
            var fileExtension = Path.GetExtension(imageFile.FileName);
            var fileName = $"{foodName}{fileExtension}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/foods");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return $"img/foods/{fileName}";
        }
        private void DeleteImage(string imagePath)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}

