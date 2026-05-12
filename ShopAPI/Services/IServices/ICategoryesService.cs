using ShopAPI.DTOs;

namespace ShopAPI.Services.IServices
{
    public interface ICategoryesService
    {
        List<CategoryDto> GetAll(bool? activeOnly = null);
        CategoryDto GetById(int id);
        CategoryDto Create(CreateCategoryRequest request);
        CategoryDto Update(int id, UpdateCategoryRequest request);
        void Deactivate(int id);
        void Activate(int id);
        bool CheckDuplicateName(string name, int? excludeId = null);
    }
}
