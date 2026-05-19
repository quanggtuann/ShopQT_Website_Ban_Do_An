using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Services.Customer
{
    public class CustomerComboSevice : ICustomerComboSevice
    {
        private readonly IComboRepo _comboRepo;

        public CustomerComboSevice(IComboRepo comboRepo)
        {
            _comboRepo = comboRepo;
        }

        public PagedResult<ComboDto> Getall(ComboFilterViewmodel comboFilterViewmodel)
        {
            var query = _comboRepo.GetAllCombos()
                .Where(c => c.IsAvailabale);

            if (!string.IsNullOrWhiteSpace(comboFilterViewmodel.KeyWord))
            {
                query = query.Where(c => c.Name.Contains(comboFilterViewmodel.KeyWord) ||
                    (c.Description != null && c.Description.Contains(comboFilterViewmodel.KeyWord)));
            }

            if (comboFilterViewmodel.FromPrice.HasValue)
            {
                query = query.Where(c => c.Price >= comboFilterViewmodel.FromPrice.Value);
            }

            if (comboFilterViewmodel.ToPrice.HasValue)
            {
                query = query.Where(c => c.Price <= comboFilterViewmodel.ToPrice.Value);
            }

            var sortBy = (comboFilterViewmodel.ShortBy ?? "name").ToLower();
            var sortOrder = (comboFilterViewmodel.ShortOrder ?? "asc").ToLower();
            query = sortBy switch
            {
                "price" => sortOrder == "desc"
                    ? query.OrderByDescending(c => c.Price)
                    : query.OrderBy(c => c.Price),
                _ => sortOrder == "desc"
                    ? query.OrderByDescending(c => c.Name)
                    : query.OrderBy(c => c.Name),
            };

            var page = comboFilterViewmodel.page <= 0 ? 1 : comboFilterViewmodel.page;
            var pageSize = comboFilterViewmodel.pageSize <= 0 ? 6 : comboFilterViewmodel.pageSize;
            var totalItems = query.Count();
            var combos = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var data = combos.Select(c => new ComboDto
            {
                ComboId = c.ComboId,
                Name = c.Name,
                Description = c.Description ?? string.Empty,
                Price = c.Price,
                IsVaiLabel = c.IsAvailabale,
                CreateDate = c.CreateDate,
                ImagePath = c.ImagePath ?? string.Empty,
                FoodItems = new List<ComboFoodItemDto>()
            }).ToList();

            return new PagedResult<ComboDto>
            {
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Data = data,
            };
        }

        public ComboDto GetById(int id)
        {
            var item = _comboRepo.GetById(id);
            if (item == null || !item.IsAvailabale)
            {
                throw new Exception("Combo not found");
            }

            return new ComboDto
            {
                ComboId = item.ComboId,
                Name = item.Name,
                Description = item.Description ?? string.Empty,
                Price = item.Price,
                IsVaiLabel = item.IsAvailabale,
                CreateDate = item.CreateDate,
                ImagePath = item.ImagePath ?? string.Empty,
                FoodItems = item.ComboFoodItem.Select(cf => new ComboFoodItemDto
                {
                    FoodItemId = cf.FoodItemID,
                    FoodName = cf.FoodItem.Name,
                    Quantity = cf.Quantity,
                }).ToList(),
            };
        }
    }
}
