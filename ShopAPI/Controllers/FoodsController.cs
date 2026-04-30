using Microsoft.AspNetCore.Mvc;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;
using ShopDAL.Models.Dto;
namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly IAdminFoodRepo _foodRepo;
        private readonly IAdminCategoryRepo _categoryRepo;
        public FoodsController(IAdminFoodRepo foodRepo, IAdminCategoryRepo categoryRepo)
        {
            _foodRepo = foodRepo;
            _categoryRepo = categoryRepo;
        }
        [HttpGet("all")]
        public IActionResult GetAllForDropdown()
        {
            try
            {
                var foods = _foodRepo.GetAll()
                    .Where(f => f.IsAvailable)
                    .Select(f => new FoodItemDto
                    {
                        FoodItemId = f.FoodItemId,
                        Name = f.Name,
                        Description = f.Description,
                        Price = f.Price,
                        IsAvailable = f.IsAvailable,
                        CreateDate = f.CreateDate,
                        ImagePath = f.ImagePath,
                        CategoryId = f.CategoryId
                    })
                    .ToList();
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
                var query = _foodRepo.GetAll().AsQueryable();      
                if (filter.categoryID.HasValue)
                {
                    query = query.Where(f => f.CategoryId == filter.categoryID);
                }
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    query = query.Where(f => f.Name.Contains(filter.Keyword) || f.Description.Contains(filter.Keyword));
                }
                if (filter.PriceFrom.HasValue)
                {
                    query = query.Where(f => f.Price >= filter.PriceFrom.Value);
                }
                if (filter.PriceTo.HasValue)
                {
                    query = query.Where(f => f.Price <= filter.PriceTo.Value);
                }
                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    query = filter.SortBy.ToLower() switch
                    {
                        "price" => filter.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(f => f.Price)
                            : query.OrderBy(f => f.Price),
                        _ => filter.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(n => n.Name)
                            : query.OrderBy(n => n.Name),
                    };
                }
                var totalItem = query.Count();
                var foodItems = query
                    .Skip((filter.page - 1) * filter.pageSize)
                    .Take(filter.pageSize)
                    .Select(f => new FoodItemDto
                    {
                        FoodItemId = f.FoodItemId,
                        Name = f.Name,
                        Description = f.Description,
                        Price = f.Price,
                        IsAvailable = f.IsAvailable,
                        CreateDate = f.CreateDate,
                        ImagePath = f.ImagePath,
                        CategoryId = f.CategoryId,
                        Category = f.Category == null ? null : new CategoryDto { CategoryId = f.Category.CategoryId, Name = f.Category.Name }
                    })
                    .ToList();
                return Ok(new
                {
                    TotalItems = totalItem,
                    CurrentPage = filter.page,
                    TotalPages = (int)Math.Ceiling(totalItem / (double)filter.pageSize),
                    Data = foodItems
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var food = _foodRepo.GetById(id);
            if (food == null)
            {
                return NotFound(new { ErrorMessage = "Food not found" });
            }
            return Ok(food);
        }
        [HttpPost]
        public IActionResult Create([FromForm] FoodItem food, IFormFile? imageFile)
        {
            // Kiểm tra trùng tên
            var existingFood = _foodRepo.GetAll().FirstOrDefault(f => f.Name.ToLower() == food.Name.ToLower());
            if (existingFood != null)
            {
                return NotFound(new { ErrorMessage = $"Food with name '{food.Name}' already exists." });
            }

            food.CreateDate = DateTime.Now;
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(imageFile.FileName);
                    var fileName = $"{food.Name}{fileExtension}";
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
                    food.ImagePath = $"img/foods/{fileName}";
                }
                _foodRepo.Add(food);
                return CreatedAtAction(nameof(GetById), new { id = food.FoodItemId }, food);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] FoodItem food, IFormFile formFile)
        {
            var currentFood = _foodRepo.GetById(id);
            if (currentFood == null)
            {
                return NotFound(new { ErrorMessage = "Food not found" });
            }
            food.FoodItemId = id;  
            food.CreateDate = DateTime.Now;
            try
            {
                if (formFile != null && formFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(formFile.FileName);
                    var newFileName = $"{food.Name}{fileExtension}";
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/foods", newFileName);
                    if (!string.IsNullOrEmpty(currentFood.ImagePath))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", currentFood.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    using (var fileStream = new FileStream(savePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    food.ImagePath = $"img/foods/{newFileName}";
                }
                else
                {
                    food.ImagePath = currentFood.ImagePath;
                }
                _foodRepo.Update(food);
                return Ok(food);
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
                _foodRepo.Deactivate(id);
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
                _foodRepo.AcTivate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
    }
}

