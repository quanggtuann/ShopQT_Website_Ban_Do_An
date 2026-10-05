using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopAPI.DTOs;
using ShopAPI.Services.IServices;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShopAPI.Services
{
    public class CategoryesService : ICategoryesService
    {
        private readonly IAdminCategoryRepo _categoryRepo;

        public CategoryesService(IAdminCategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public List<CategoryDto> GetAll(bool? activeOnly = null)
        {
            var categories = _categoryRepo.GetAll();

            if (activeOnly.HasValue && activeOnly.Value)
            {
                categories = categories.Where(c => c.IsActive).ToList();
            }           
            return categories.Select(MapToDto).ToList();        
        }

        public CategoryDto GetById(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
                throw new Exception("Category not found");

            return MapToDto(category);
        }

        public CategoryDto Create(CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Category name is required");

            if (CheckDuplicateName(request.Name))
                throw new Exception("Category name already exists");

            var category = new Category
            {
                Name = request.Name,
                IsActive = true
            };

            _categoryRepo.Add(category);
            return MapToDto(category);
        }

        public CategoryDto Update(int id, UpdateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Category name is required");

            var existingCategory = _categoryRepo.GetById(id);
            if (existingCategory == null)
                throw new Exception("Category not found");

            if (CheckDuplicateName(request.Name, id))
                throw new Exception("Category name already exists");

            existingCategory.Name = request.Name;
            _categoryRepo.Update(existingCategory);

            return MapToDto(existingCategory);
        }

        public void Deactivate(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
                throw new Exception("Category not found");

            _categoryRepo.Deactive(id);
        }

        public void Activate(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
                throw new Exception("Category not found");

            _categoryRepo.Activate(id);
        }

        public bool CheckDuplicateName(string name, int? excludeId = null)
        {
            var categories = _categoryRepo.GetAll();
            return categories.Any(c =>
                c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                (!excludeId.HasValue || c.CategoryId != excludeId.Value));
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                IsActive = category.IsActive
            };
        }
    }
}
