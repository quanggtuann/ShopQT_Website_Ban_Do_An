using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;
using ShopDAL.Models.Dto;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombosController : Controller
    {
        private readonly IAdminComboRepo _adminComboRepo;
        private readonly IAdminFoodRepo _foodRepo;
        public CombosController(IAdminComboRepo adminComboRepo, IAdminFoodRepo foodRepo)
        {
            _adminComboRepo = adminComboRepo;
            _foodRepo = foodRepo;
        }
        [HttpGet]
        public IActionResult Getall([FromQuery] ComboFilterViewmodel Filter)
        {
            try
            {
                var query = _adminComboRepo.Getall()
                     .Include(c => c.ComboFoodItem)       
                      .ThenInclude(cf => cf.FoodItem)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(Filter.KeyWord))
                {
                    query = query.Where(c => c.Name.Contains(Filter.KeyWord) ||
                    c.Description.Contains(Filter.KeyWord)
                    );
                }
                if (Filter.FromPrice.HasValue)
                {
                    query = query.Where(c => c.Price >= Filter.FromPrice.Value);
                }
                if (Filter.ToPrice.HasValue)
                {
                    query = query.Where(c => c.Price <= Filter.ToPrice.Value);
                }
                if (!string.IsNullOrEmpty(Filter.ShortBy))
                {
                    query = Filter.ShortBy.ToLower() switch
                    {
                        "price" => Filter.ShortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(f => f.Price)
                            : query.OrderBy(f => f.Price),
                        _ => Filter.ShortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(n => n.Name)
                            : query.OrderBy(n => n.Name),
                    };
                }
                var totalItem = query.Count();
                var totalItems = query.
                    Skip((Filter.page - 1) * Filter.pageSize)
                    .Take(Filter.pageSize)
                    .Select(f => new ComboDto
                    {
                        ComboId = f.ComboId,
                        Name = f.Name,
                        Description = f.Description,
                        Price = f.Price,
                        IsVaiLabel = f.IsAvailabale,
                        CreateDate = f.CreateDate,
                        ImagePath = f.ImagePath,
                        FoodItems = f.ComboFoodItem.Select(cf => new ComboFoodItemDto
                        {
                            FoodItemId = cf.FoodItemID,
                            FoodName = cf.FoodItem.Name,
                            Quantity = cf.Quantity
                        }).ToList()
                    }).ToList();
                return Ok(new
                {
                    TotalItems = totalItem,
                    CurrentPage = Filter.page,
                    ToTalPage = (int)Math.Ceiling(totalItem / (double)Filter.pageSize),
                    Data = totalItems
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
            var combo = _adminComboRepo.GetById(id);
            if (combo == null)
            {
                return NotFound(new { ErrorMessage = "Combo Not found" });
            }
            var result = new ComboDto
            {
                ComboId = combo.ComboId,
                Name = combo.Name,
                Description = combo.Description,
                Price = combo.Price,
                IsVaiLabel = combo.IsAvailabale,
                CreateDate = combo.CreateDate,
                ImagePath = combo.ImagePath,
                FoodItems = combo.ComboFoodItem?.Select(cf => new ComboFoodItemDto
                {
                    FoodItemId = cf.FoodItemID,
                    FoodName = cf.FoodItem?.Name,
                    Quantity = cf.Quantity
                }).ToList() ?? new List<ComboFoodItemDto>()
            };

            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateCombo([FromForm] CreateComboRequest request, IFormFile? ImageFile)
        {
            var combo = new Combo
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsAvailabale = request.IsAvailabale,
                CreateDate = DateTime.Now,
                ComboFoodItem = request.FoodItems?.Select(f => new ComboFoodItem
                {
                    FoodItemID = f.FoodItemId,
                    Quantity = f.Quantity
                }).ToList()
            };

            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(ImageFile.FileName);
                    var fileName = $"{combo.Name}{fileExtension}";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/combos");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }
                    combo.ImagePath = $"img/combos/{fileName}";
                }
                _adminComboRepo.Add(combo);
                var result = new ComboDto
                {
                    ComboId = combo.ComboId,
                    Name = combo.Name,
                    Description = combo.Description,
                    Price = combo.Price,
                    IsVaiLabel = combo.IsAvailabale,
                    CreateDate = combo.CreateDate,
                    ImagePath = combo.ImagePath
                };
                return CreatedAtAction(nameof(GetById), new { id = combo.ComboId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public IActionResult UpdateCombo([FromForm] UpdateComboRequest request, IFormFile? ImageFile, int id)
        {
            var currentCombo = _adminComboRepo.GetById(id);
            if (currentCombo == null)
            {
                return NotFound(new { ErrorMessage = "Combo Not found" });
            }
            var combo = new Combo
            {
                ComboId = id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsAvailabale = request.IsAvailabale,
                CreateDate = currentCombo.CreateDate,
                ComboFoodItem = new List<ComboFoodItem>()
            };
            if(request.FoodItems!=null)
            {
                combo.ComboFoodItem = request.FoodItems
                    .Where(f=>f.FoodItemId>0)
                    .Select(f => new ComboFoodItem
                {
                    FoodItemID = f.FoodItemId,
                    Quantity = f.Quantity,
                }).ToList();
            }
            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var FileExtention = Path.GetExtension(ImageFile.FileName);
                    var NewFileName = $"{request.Name}.{FileExtention}";
                    var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/food", NewFileName);
                    if (!String.IsNullOrEmpty(currentCombo.ImagePath))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", currentCombo.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    using (var filestream = new FileStream(SavePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(filestream);
                    }
                    combo.ImagePath = $"img/combos/{NewFileName}";
                }
                else if(request.RemoveImage){
                    combo.ImagePath=null ;
                }
                else
                {
                    combo.ImagePath = currentCombo.ImagePath;
                }
                _adminComboRepo.Update(combo);
                var result = new ComboDto
                {
                    ComboId = combo.ComboId,
                    Name = combo.Name,
                    Description = combo.Description,
                    Price = combo.Price,
                    IsVaiLabel = combo.IsAvailabale,
                    CreateDate = combo.CreateDate,
                    ImagePath = combo.ImagePath,
                };
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
                _adminComboRepo.Deactivate(id);
                return NoContent();
            }
            catch(Exception ex) 
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
        [HttpPatch("{id}/Activate")]
        public IActionResult Activate(int id)
        {
            try
            {
                _adminComboRepo.Activate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }
    }
}
