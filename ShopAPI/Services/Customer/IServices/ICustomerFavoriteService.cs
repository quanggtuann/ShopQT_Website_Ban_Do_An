using ShopAPI.DTOs;

namespace ShopAPI.Services.Customer.IServices
{
    public interface ICustomerFavoriteService
    {
        List<FavoriteDto> GetByUserId(int userId);
        bool IsFavorite(int userId, int foodId);
        ToggleFavoriteResultDto ToggleFood(int userId, int foodId);
        void RemoveFood(int userId, int foodId);
    }
}
