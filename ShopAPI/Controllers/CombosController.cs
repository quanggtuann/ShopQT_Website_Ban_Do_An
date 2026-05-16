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
    public class CombosController : ControllerBase
    {
        private readonly IComboService _comboService;

        public CombosController(IComboService comboService)
        {
            _comboService = comboService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] ComboFilterViewmodel filter)
        {
            try
            {
                var result = _comboService.GetAll(filter);
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
                var result = _comboService.GetById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromForm] CreateComboRequest request, IFormFile? imageFile)
        {
            if (_comboService.CheckDuplicateName(request.Name))
            {
                return BadRequest(new { ErrorMessage = $"Combo with name '{request.Name}' already exists." });
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                request.ImagePath = SaveImage(imageFile, request.Name);
            }

            try
            {
                var result = _comboService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = result.ComboId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] UpdateComboRequest request, IFormFile? imageFile)
        {
            var currentCombo = _comboService.GetComboById(id);
            if (currentCombo == null)
            {
                return NotFound(new { ErrorMessage = "Combo Not found" });
            }

            if (_comboService.CheckDuplicateName(request.Name, id))
            {
                return BadRequest(new { ErrorMessage = $"Combo with name '{request.Name}' already exists." });
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(currentCombo.ImagePath))
                {
                    DeleteImage(currentCombo.ImagePath);
                }
                request.ImagePath = SaveImage(imageFile, request.Name);
            }
            else if (request.RemoveImage)
            {
                if (!string.IsNullOrEmpty(currentCombo.ImagePath))
                {
                    DeleteImage(currentCombo.ImagePath);
                }
                request.ImagePath = null;
            }
            else
            {
                request.ImagePath = currentCombo.ImagePath;
            }

            try
            {
                var result = _comboService.Update(id, request);
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
                _comboService.Deactivate(id);
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
                _comboService.Activate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        private string SaveImage(IFormFile imageFile, string comboName)
        {
            var fileExtension = Path.GetExtension(imageFile.FileName);
            var fileName = $"{comboName}{fileExtension}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/combos");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return $"img/combos/{fileName}";
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
