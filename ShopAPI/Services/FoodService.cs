using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;
using ShopAPI.DTOs;
using ShopAPI.Services.IServices;

namespace ShopAPI.Services
{
    public class FoodService : IFoodService
    {
        private readonly IAdminFoodRepo _foodRepo;

        public FoodService(IAdminFoodRepo foodRepo)
        {
            _foodRepo = foodRepo;
        }

        public List<FoodItemDto> GetAllForDropdown()
        {
            return _foodRepo.GetAll()
                .Where(f => f.IsAvailable)
                .Select(f => new FoodItemDto
                {
                    FoodItemId = f.FoodItemId,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    IsAvailable = f.IsAvailable,
                    CreateDate = f.CreateDate,
                    ImagePath = f.ImagePath,
                    CategoryId = f.CategoryId
                })
                .ToList();
        }

        public PagedResult<FoodItemDto> GetAll(FoodItemFilterViewModel filter)
        {
            var query = _foodRepo.GetAll().AsQueryable();

            if (filter.categoryID.HasValue)
            {
                query = query.Where(f => f.CategoryId == filter.categoryID);
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(f => f.Name.Contains(filter.Keyword) || f.Description.Contains(filter.Keyword));
            }

            if (filter.PriceFrom.HasValue)
            {
                query = query.Where(f => f.Price >= filter.PriceFrom.Value);
            }

            if (filter.PriceTo.HasValue)
            {
                query = query.Where(f => f.Price <= filter.PriceTo.Value);
            }

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                query = filter.SortBy.ToLower() switch
                {
                    "price" => filter.SortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(f => f.Price)
                        : query.OrderBy(f => f.Price),
                    _ => filter.SortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(n => n.Name)
                        : query.OrderBy(n => n.Name),
                };
            }

            var totalItem = query.Count();
            var foodItems = query
                .Skip((filter.page - 1) * filter.pageSize)
                .Take(filter.pageSize)
                .Select(f => new FoodItemDto
                {
                    FoodItemId = f.FoodItemId,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    IsAvailable = f.IsAvailable,
                    CreateDate = f.CreateDate,
                    ImagePath = f.ImagePath,
                    CategoryId = f.CategoryId,
                    Category = f.Category == null ? null : new CategoryDto { CategoryId = f.Category.CategoryId, Name = f.Category.Name }
                })
                .ToList();

            return new PagedResult<FoodItemDto>
            {
                TotalItems = totalItem,
                CurrentPage = filter.page,
                TotalPages = (int)Math.Ceiling(totalItem / (double)filter.pageSize),
                Data = foodItems
            };
        }

        public FoodItemDto GetById(int id)
        {
            var food = _foodRepo.GetById(id);
            if (food == null) throw new Exception("Food not found");
            return MapToDto(food);
        }

        public FoodItemDto Create(FoodItem food)
        {
            food.CreateDate = DateTime.Now;
            _foodRepo.Add(food);
            return MapToDto(food);
        }

        public FoodItemDto Update(int id, FoodItem food)
        {
            var existing = _foodRepo.GetById(id);
            if (existing == null) throw new Exception("Food not found");

            food.FoodItemId = id;
            food.CreateDate = existing.CreateDate;
            _foodRepo.Update(food);
            return MapToDto(food);
        }

        private FoodItemDto MapToDto(FoodItem food)
        {
            return new FoodItemDto
            {
                FoodItemId = food.FoodItemId,
                Name = food.Name,
                Description = food.Description,
                Price = food.Price,
                IsAvailable = food.IsAvailable,
                CreateDate = food.CreateDate,
                ImagePath = food.ImagePath,
                CategoryId = food.CategoryId
            };
        }

        public void Deactivate(int id)
        {
            _foodRepo.Deactivate(id);
        }

        public void Activate(int id)
        {
            _foodRepo.AcTivate(id);
        }

        public bool CheckDuplicateName(string name, int? excludeId = null)
        {
            var existing = _foodRepo.GetAll().FirstOrDefault(f => f.Name.ToLower() == name.ToLower());
            if (existing == null) return false;
            if (excludeId.HasValue && existing.FoodItemId == excludeId.Value) return false;
            return true;
        }
    }
}
