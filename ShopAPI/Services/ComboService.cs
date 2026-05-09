using Microsoft.EntityFrameworkCore;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;
using ShopAPI.DTOs;
using ShopAPI.Services.IServices;

namespace ShopAPI.Services
{
    public class ComboService : IComboService
    {
        private readonly IAdminComboRepo _comboRepo;

        public ComboService(IAdminComboRepo comboRepo)
        {
            _comboRepo = comboRepo;
        }

        public PagedResult<ComboDto> GetAll(ComboFilterViewmodel filter)
        {
            var query = _comboRepo.Getall()
                .Include(c => c.ComboFoodItem)
                .ThenInclude(cf => cf.FoodItem)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.KeyWord))
            {
                query = query.Where(c => c.Name.Contains(filter.KeyWord) ||
                                         c.Description.Contains(filter.KeyWord));
            }

            if (filter.FromPrice.HasValue)
            {
                query = query.Where(c => c.Price >= filter.FromPrice.Value);
            }

            if (filter.ToPrice.HasValue)
            {
                query = query.Where(c => c.Price <= filter.ToPrice.Value);
            }

            if (!string.IsNullOrEmpty(filter.ShortBy))
            {
                query = filter.ShortBy.ToLower() switch
                {
                    "price" => filter.ShortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(f => f.Price)
                        : query.OrderBy(f => f.Price),
                    _ => filter.ShortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(n => n.Name)
                        : query.OrderBy(n => n.Name),
                };
            }

            var totalItem = query.Count();
            var combos = query
                .Skip((filter.page - 1) * filter.pageSize)
                .Take(filter.pageSize)
                .Select(f => new ComboDto
                {
                    ComboId = f.ComboId,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    IsVaiLabel = f.IsAvailabale,
                    CreateDate = f.CreateDate,
                    ImagePath = f.ImagePath,
                    FoodItems = f.ComboFoodItem.Select(cf => new ComboFoodItemDto
                    {
                        FoodItemId = cf.FoodItemID,
                        FoodName = cf.FoodItem.Name,
                        Quantity = cf.Quantity
                    }).ToList()
                })
                .ToList();

            return new PagedResult<ComboDto>
            {
                TotalItems = totalItem,
                CurrentPage = filter.page,
                TotalPages = (int)Math.Ceiling(totalItem / (double)filter.pageSize),
                Data = combos
            };
        }

        public ComboDto GetById(int id)
        {
            var combo = _comboRepo.GetById(id);
            if (combo == null) throw new Exception("Combo Not found");

            return new ComboDto
            {
                ComboId = combo.ComboId,
                Name = combo.Name,
                Description = combo.Description,
                Price = combo.Price,
                IsVaiLabel = combo.IsAvailabale,
                CreateDate = combo.CreateDate,
                ImagePath = combo.ImagePath,
                FoodItems = combo.ComboFoodItem?.Select(cf => new ComboFoodItemDto
                {
                    FoodItemId = cf.FoodItemID,
                    FoodName = cf.FoodItem?.Name,
                    Quantity = cf.Quantity
                }).ToList() ?? new List<ComboFoodItemDto>()
            };
        }

        public Combo GetComboById(int id)
        {
            var combo = _comboRepo.GetById(id);
            if (combo == null) throw new Exception("Combo Not found");
            return combo;
        }

        public ComboDto Create(CreateComboRequest request)
        {
            var combo = new Combo
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsAvailabale = request.IsAvailabale,
                ImagePath = request.ImagePath,
                CreateDate = DateTime.Now,
                ComboFoodItem = request.FoodItems?.Select(f => new ComboFoodItem
                {
                    FoodItemID = f.FoodItemId,
                    Quantity = f.Quantity
                }).ToList()
            };

            _comboRepo.Add(combo);
            return MapToDto(combo);
        }

        public ComboDto Update(int id, UpdateComboRequest request)
        {
            var currentCombo = _comboRepo.GetById(id);
            if (currentCombo == null) throw new Exception("Combo Not found");

            var combo = new Combo
            {
                ComboId = id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsAvailabale = request.IsAvailabale,
                ImagePath = request.ImagePath,
                CreateDate = currentCombo.CreateDate,
                ComboFoodItem = new List<ComboFoodItem>()
            };

            if (request.FoodItems != null)
            {
                combo.ComboFoodItem = request.FoodItems
                    .Where(f => f.FoodItemId > 0)
                    .Select(f => new ComboFoodItem
                    {
                        FoodItemID = f.FoodItemId,
                        Quantity = f.Quantity,
                    }).ToList();
            }

            _comboRepo.Update(combo);
            return MapToDto(combo);
        }

        private ComboDto MapToDto(Combo combo)
        {
            return new ComboDto
            {
                ComboId = combo.ComboId,
                Name = combo.Name,
                Description = combo.Description,
                Price = combo.Price,
                IsVaiLabel = combo.IsAvailabale,
                CreateDate = combo.CreateDate,
                ImagePath = combo.ImagePath,
                FoodItems = combo.ComboFoodItem?.Select(cf => new ComboFoodItemDto
                {
                    FoodItemId = cf.FoodItemID,
                    FoodName = cf.FoodItem?.Name,
                    Quantity = cf.Quantity
                }).ToList() ?? new List<ComboFoodItemDto>()
            };
        }

        public void Deactivate(int id)
        {
            _comboRepo.Deactivate(id);
        }

        public void Activate(int id)
        {
            _comboRepo.Activate(id);
        }

        public bool CheckDuplicateName(string name, int? excludeId = null)
        {
            var existing = _comboRepo.Getall().FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
            if (existing == null) return false;
            if (excludeId.HasValue && existing.ComboId == excludeId.Value) return false;
            return true;
        }
    }
}
