using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopAPI.Services.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Services
{
    public class CustomerFoodService : ICustomerFoodService
    {
        private readonly IFoodRepo _foodRepo;
        private readonly ICustomerDiscountPriceService _discountPriceService;

        public CustomerFoodService(IFoodRepo foodRepo, ICustomerDiscountPriceService discountPriceService)
        {
            _foodRepo = foodRepo;
            _discountPriceService = discountPriceService;
        }

        public PagedResult<FoodItemDto> GetAll(FoodItemFilterViewModel filter)
        {
            var query = _foodRepo.Getall()
                .Where(f => f.IsAvailable)
                .AsQueryable();

            if (filter.categoryID.HasValue)
            {
                query = query.Where(f => f.CategoryId == filter.categoryID.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(f =>
                    f.Name.Contains(filter.Keyword) ||
                    (f.Description != null && f.Description.Contains(filter.Keyword)));
            }

            var page = filter.page <= 0 ? 1 : filter.page;
            var pageSize = filter.pageSize <= 0 ? 6 : filter.pageSize;
            var foodItems = query.ToList();
            var discounts = _discountPriceService.GetFoodDiscounts(foodItems.Select(f => f.FoodItemId));
            var dataQuery = foodItems
                .Select(f => MapToDto(f, discounts.GetValueOrDefault(f.FoodItemId)))
                .AsQueryable();

            if (filter.PriceFrom.HasValue)
            {
                dataQuery = dataQuery.Where(f => f.Price >= filter.PriceFrom.Value);
            }

            if (filter.PriceTo.HasValue)
            {
                dataQuery = dataQuery.Where(f => f.Price <= filter.PriceTo.Value);
            }

            var sortBy = (filter.SortBy ?? "name").ToLower();
            var sortOrder = (filter.SortOrder ?? "asc").ToLower();
            dataQuery = sortBy switch
            {
                "price" => sortOrder == "desc"
                    ? dataQuery.OrderByDescending(f => f.Price)
                    : dataQuery.OrderBy(f => f.Price),
                _ => sortOrder == "desc"
                    ? dataQuery.OrderByDescending(f => f.Name)
                    : dataQuery.OrderBy(f => f.Name)
            };

            var totalItems = dataQuery.Count();
            var data = dataQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<FoodItemDto>
            {
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Data = data
            };
        }

        public FoodItemDto GetById(int id)
        {
            var item = _foodRepo.GetById(id);
            if (item == null || !item.IsAvailable)
            {
                throw new Exception("Food not found");
            }

            var discount = _discountPriceService.GetFoodDiscount(item.FoodItemId, item.Price);
            return MapToDto(item, discount);
        }

        private static FoodItemDto MapToDto(FoodItem item, DiscountPriceResult? discount)
        {
            discount ??= new DiscountPriceResult
            {
                OriginalPrice = item.Price,
                FinalPrice = item.Price
            };

            return new FoodItemDto
            {
                FoodItemId = item.FoodItemId,
                Name = item.Name,
                Description = item.Description ?? string.Empty,
                Price = discount.FinalPrice,
                OriginalPrice = discount.OriginalPrice,
                FinalPrice = discount.FinalPrice,
                HasDiscount = discount.HasDiscount,
                DiscountCampaignName = discount.CampaignName,
                DiscountType = discount.DiscountType,
                DiscountValue = discount.DiscountValue,
                IsAvailable = item.IsAvailable,
                CreateDate = item.CreateDate,
                ImagePath = item.ImagePath ?? string.Empty,
                CategoryId = item.CategoryId,
                Category = item.Category == null
                    ? null
                    : new CategoryDto
                    {
                        CategoryId = item.Category.CategoryId,
                        Name = item.Category.Name,
                        IsActive = item.Category.IsActive
                    }
            };
        }
    }
}
