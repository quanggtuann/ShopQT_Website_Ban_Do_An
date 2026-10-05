using ShopAPI.DTOs;
using ShopDAL.Models;

namespace ShopAPI.Services.IServices
{
    public interface ICustomerFoodService
    {
        PagedResult<FoodItemDto> GetAll(FoodItemFilterViewModel filter);
        FoodItemDto GetById(int id);
    }
}
