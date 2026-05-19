using ShopAPI.DTOs;
using ShopAPI.Services.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Services
{
    public class CustomerFoodService : ICustomerFoodService
    {
        private readonly IFoodRepo _foodRepo;

        public CustomerFoodService(IFoodRepo foodRepo)
        {
            _foodRepo = foodRepo;
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

            if (filter.PriceFrom.HasValue)
            {
                query = query.Where(f => f.Price >= filter.PriceFrom.Value);
            }

            if (filter.PriceTo.HasValue)
            {
                query = query.Where(f => f.Price <= filter.PriceTo.Value);
            }

            var sortBy = (filter.SortBy ?? "name").ToLower();
            var sortOrder = (filter.SortOrder ?? "asc").ToLower();
            query = sortBy switch
            {
                "price" => sortOrder == "desc"
                    ? query.OrderByDescending(f => f.Price)
                    : query.OrderBy(f => f.Price),
                _ => sortOrder == "desc"
                    ? query.OrderByDescending(f => f.Name)
                    : query.OrderBy(f => f.Name)
            };

            var page = filter.page <= 0 ? 1 : filter.page;
            var pageSize = filter.pageSize <= 0 ? 6 : filter.pageSize;
            var totalItems = query.Count();
            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FoodItemDto
                {
                    FoodItemId = f.FoodItemId,
                    Name = f.Name,
                    Description = f.Description ?? string.Empty,
                    Price = f.Price,
                    IsAvailable = f.IsAvailable,
                    CreateDate = f.CreateDate,
                    ImagePath = f.ImagePath ?? string.Empty,
                    CategoryId = f.CategoryId,
                    Category = f.Category == null
                        ? null
                        : new CategoryDto
                        {
                            CategoryId = f.Category.CategoryId,
                            Name = f.Category.Name,
                            IsActive = f.Category.IsActive
                        }
                })
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

            return new FoodItemDto
            {
                FoodItemId = item.FoodItemId,
                Name = item.Name,
                Description = item.Description ?? string.Empty,
                Price = item.Price,
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
