using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Services.Customer
{
    public class CustomerFavoriteService : ICustomerFavoriteService
    {
        private readonly IFavoriteRepo _favoriteRepo;
        private readonly ICustomerDiscountPriceService _discountPriceService;

        public CustomerFavoriteService(
            IFavoriteRepo favoriteRepo,
            ICustomerDiscountPriceService discountPriceService)
        {
            _favoriteRepo = favoriteRepo;
            _discountPriceService = discountPriceService;
        }

        public List<FavoriteDto> GetByUserId(int userId)
        {
            var favorites = _favoriteRepo.GetByUserId(userId);
            var discounts = _discountPriceService.GetFoodDiscounts(favorites.Select(f => f.FoodId));

            return favorites
                .Select(f => MapToDto(f, discounts.GetValueOrDefault(f.FoodId)))
                .ToList();
        }

        public bool IsFavorite(int userId, int foodId)
        {
            return _favoriteRepo.GetByUserAndFood(userId, foodId) != null;
        }

        public ToggleFavoriteResultDto ToggleFood(int userId, int foodId)
        {
            var favorite = _favoriteRepo.GetByUserAndFood(userId, foodId);
            if (favorite != null)
            {
                _favoriteRepo.Remove(favorite);
                _favoriteRepo.Save();

                return new ToggleFavoriteResultDto
                {
                    IsFavorite = false,
                    Message = "Removed from favorites"
                };
            }

            if (!_favoriteRepo.FoodExists(foodId))
            {
                throw new KeyNotFoundException("Food not found");
            }

            favorite = new Favorite
            {
                UserId = userId,
                FoodId = foodId,
                CreateDate = DateTime.Now
            };

            _favoriteRepo.Add(favorite);
            _favoriteRepo.Save();

            favorite = _favoriteRepo.GetByUserAndFood(userId, foodId);
            return new ToggleFavoriteResultDto
            {
                IsFavorite = true,
                Message = "Added to favorites",
                Favorite = MapToDto(favorite, _discountPriceService.GetFoodDiscount(favorite.FoodId, favorite.FoodItem.Price))
            };
        }

        public void RemoveFood(int userId, int foodId)
        {
            var favorite = _favoriteRepo.GetByUserAndFood(userId, foodId);
            if (favorite == null)
            {
                return;
            }

            _favoriteRepo.Remove(favorite);
            _favoriteRepo.Save();
        }

        private static FavoriteDto MapToDto(Favorite favorite, DiscountPriceResult? discount)
        {
            var food = favorite.FoodItem;
            discount ??= new DiscountPriceResult
            {
                OriginalPrice = food.Price,
                FinalPrice = food.Price
            };

            return new FavoriteDto
            {
                Id = favorite.Id,
                UserId = favorite.UserId,
                FoodId = favorite.FoodId,
                CreateDate = favorite.CreateDate,
                Food = new FoodItemDto
                {
                    FoodItemId = food.FoodItemId,
                    Name = food.Name,
                    Description = food.Description ?? string.Empty,
                    Price = discount.FinalPrice,
                    OriginalPrice = discount.OriginalPrice,
                    FinalPrice = discount.FinalPrice,
                    HasDiscount = discount.HasDiscount,
                    DiscountCampaignName = discount.CampaignName,
                    DiscountType = discount.DiscountType,
                    DiscountValue = discount.DiscountValue,
                    IsAvailable = food.IsAvailable,
                    CreateDate = food.CreateDate,
                    ImagePath = food.ImagePath ?? string.Empty,
                    CategoryId = food.CategoryId,
                    Category = food.Category == null
                        ? null
                        : new CategoryDto
                        {
                            CategoryId = food.Category.CategoryId,
                            Name = food.Category.Name,
                            IsActive = food.Category.IsActive
                        }
                }
            };
        }
    }
}
