using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Services.Customer
{
    public class CustomerComboService : ICustomerComboService
    {
        private readonly IComboRepo _comboRepo;
        private readonly ICustomerDiscountPriceService _discountPriceService;

        public CustomerComboService(IComboRepo comboRepo, ICustomerDiscountPriceService discountPriceService)
        {
            _comboRepo = comboRepo;
            _discountPriceService = discountPriceService;
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

            var page = comboFilterViewmodel.page <= 0 ? 1 : comboFilterViewmodel.page;
            var pageSize = comboFilterViewmodel.pageSize <= 0 ? 6 : comboFilterViewmodel.pageSize;
            var combos = query.ToList();
            var discounts = _discountPriceService.GetComboDiscounts(combos.Select(c => c.ComboId));
            var dataQuery = combos
                .Select(c =>
                {
                    var discount = discounts.GetValueOrDefault(c.ComboId);
                    return new ComboDto
                    {
                        ComboId = c.ComboId,
                        Name = c.Name,
                        Description = c.Description ?? string.Empty,
                        Price = discount?.FinalPrice ?? c.Price,
                        OriginalPrice = discount?.OriginalPrice ?? c.Price,
                        FinalPrice = discount?.FinalPrice ?? c.Price,
                        HasDiscount = discount?.HasDiscount ?? false,
                        DiscountCampaignName = discount?.CampaignName,
                        DiscountType = discount?.DiscountType,
                        DiscountValue = discount?.DiscountValue,
                        IsVaiLabel = c.IsAvailabale,
                        CreateDate = c.CreateDate,
                        ImagePath = c.ImagePath ?? string.Empty,
                        FoodItems = c.ComboFoodItem?.Select(cf => new ComboFoodItemDto
                        {
                            FoodItemId = cf.FoodItemID,
                            FoodName = cf.FoodItem.Name,
                            Quantity = cf.Quantity,
                        }).ToList() ?? new List<ComboFoodItemDto>()
                    };
                })
                .AsQueryable();

            if (comboFilterViewmodel.FromPrice.HasValue)
            {
                dataQuery = dataQuery.Where(c => c.Price >= comboFilterViewmodel.FromPrice.Value);
            }

            if (comboFilterViewmodel.ToPrice.HasValue)
            {
                dataQuery = dataQuery.Where(c => c.Price <= comboFilterViewmodel.ToPrice.Value);
            }

            var sortBy = (comboFilterViewmodel.ShortBy ?? "name").ToLower();
            var sortOrder = (comboFilterViewmodel.ShortOrder ?? "asc").ToLower();
            dataQuery = sortBy switch
            {
                "price" => sortOrder == "desc"
                    ? dataQuery.OrderByDescending(c => c.Price)
                    : dataQuery.OrderBy(c => c.Price),
                _ => sortOrder == "desc"
                    ? dataQuery.OrderByDescending(c => c.Name)
                    : dataQuery.OrderBy(c => c.Name),
            };

            var totalItems = dataQuery.Count();
            var data = dataQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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

            var discount = _discountPriceService.GetComboDiscount(item.ComboId, item.Price);
            return new ComboDto
            {
                ComboId = item.ComboId,
                Name = item.Name,
                Description = item.Description ?? string.Empty,
                Price = discount.FinalPrice,
                OriginalPrice = discount.OriginalPrice,
                FinalPrice = discount.FinalPrice,
                HasDiscount = discount.HasDiscount,
                DiscountCampaignName = discount.CampaignName,
                DiscountType = discount.DiscountType,
                DiscountValue = discount.DiscountValue,
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
