using ShopDAL.Models;
using ShopAPI.DTOs;

namespace ShopAPI.Services.IServices
{
    public interface IFoodService
    {
        List<FoodItemDto> GetAllForDropdown();
        PagedResult<FoodItemDto> GetAll(FoodItemFilterViewModel filter);
        FoodItemDto GetById(int id);
        FoodItemDto Create(FoodItem food);
        FoodItemDto Update(int id, FoodItem food);
        void Deactivate(int id);
        void Activate(int id);
        bool CheckDuplicateName(string name, int? excludeId = null);
    }

}
