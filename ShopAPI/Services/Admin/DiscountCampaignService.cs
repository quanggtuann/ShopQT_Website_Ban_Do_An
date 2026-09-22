using ShopAPI.DTOs;
using ShopAPI.Services.IServices;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;

namespace ShopAPI.Services
{
    public class DiscountCampaignService : IDiscountCampaignService
    {
        private readonly IAdminDiscountCampaignRepo _discountCampaignRepo;

        public DiscountCampaignService(IAdminDiscountCampaignRepo discountCampaignRepo)
        {
            _discountCampaignRepo = discountCampaignRepo;
        }

        public PagedResult<DiscountCampaignDto> GetAll(DiscountCampaignFilterViewModel filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;
            var query = _discountCampaignRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(c =>
                    c.CampaignName.Contains(filter.Keyword) ||
                    c.Description.Contains(filter.Keyword));
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == filter.IsActive.Value);
            }

            if (filter.DiscountType.HasValue)
            {
                query = query.Where(c => c.DiscountType == filter.DiscountType.Value);
            }

            query = filter.SortBy?.ToLower() switch
            {
                "startdate" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.StartDate)
                    : query.OrderBy(c => c.StartDate),
                "enddate" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.EndDate)
                    : query.OrderBy(c => c.EndDate),
                "discountvalue" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.DiscountValue)
                    : query.OrderBy(c => c.DiscountValue),
                _ => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.CreatedAt)
                    : query.OrderBy(c => c.CampaignName)
            };

            var totalItems = query.Count();
            var campaigns = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            return new PagedResult<DiscountCampaignDto>
            {
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Data = campaigns
            };
        }

        public DiscountCampaignDto GetById(int id)
        {
            var campaign = _discountCampaignRepo.GetById(id);
            if (campaign == null)
            {
                throw new Exception("Discount campaign not found");
            }

            return MapToDto(campaign);
        }

        public DiscountCampaignDto Create(CreateDiscountCampaignRequest request)
        {
            ValidateRequest(request);

            if (CheckDuplicateName(request.CampaignName))
            {
                throw new ArgumentException($"Discount campaign '{request.CampaignName}' already exists.");
            }

            var campaign = BuildCampaign(request);
            campaign.CreatedAt = DateTime.Now;

            _discountCampaignRepo.Add(campaign);
            return MapToDto(campaign);
        }

        public DiscountCampaignDto Update(int id, UpdateDiscountCampaignRequest request)
        {
            ValidateRequest(request);

            if (CheckDuplicateName(request.CampaignName, id))
            {
                throw new ArgumentException($"Discount campaign '{request.CampaignName}' already exists.");
            }

            var existing = _discountCampaignRepo.GetById(id);
            if (existing == null)
            {
                throw new Exception("Discount campaign not found");
            }

            var campaign = BuildCampaign(request);
            campaign.DiscountCampaignId = id;
            campaign.CreatedAt = existing.CreatedAt;
            campaign.CreatedBy = existing.CreatedBy;
            campaign.UpdatedAt = DateTime.Now;

            _discountCampaignRepo.Update(campaign);
            return GetById(id);
        }

        public void Activate(int id)
        {
            _discountCampaignRepo.Activate(id);
        }

        public void Deactivate(int id)
        {
            _discountCampaignRepo.Deactivate(id);
        }

        public bool CheckDuplicateName(string campaignName, int? excludeId = null)
        {
            return _discountCampaignRepo.ExistsByName(campaignName, excludeId);
        }

        private static DiscountCampaign BuildCampaign(CreateDiscountCampaignRequest request)
        {
            var now = DateTime.Now;

            return new DiscountCampaign
            {
                CampaignName = request.CampaignName.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive,
                CampaignProducts = request.FoodItemIds
                    .Distinct()
                    .Select(foodItemId => new DiscountCampaignProduct
                    {
                        FoodItemId = foodItemId,
                        CreatedAt = now
                    })
                    .ToList(),
                CampaignCombos = request.ComboIds
                    .Distinct()
                    .Select(comboId => new DiscountCampaignCombo
                    {
                        ComboId = comboId,
                        CreatedAt = now
                    })
                    .ToList()
            };
        }

        private static void ValidateRequest(CreateDiscountCampaignRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CampaignName))
            {
                throw new ArgumentException("Campaign name is required.");
            }

            if (request.DiscountValue <= 0)
            {
                throw new ArgumentException("Discount value must be greater than 0.");
            }

            if (request.DiscountType == DiscountType.Percent && request.DiscountValue > 100)
            {
                throw new ArgumentException("Percent discount cannot be greater than 100.");
            }

            if (request.EndDate < request.StartDate)
            {
                throw new ArgumentException("End date must be greater than or equal to start date.");
            }

            if (!request.FoodItemIds.Any() && !request.ComboIds.Any())
            {
                throw new ArgumentException("Please select at least one food or combo.");
            }
        }

        private static DiscountCampaignDto MapToDto(DiscountCampaign campaign)
        {
            return new DiscountCampaignDto
            {
                DiscountCampaignId = campaign.DiscountCampaignId,
                CampaignName = campaign.CampaignName,
                Description = campaign.Description,
                DiscountType = campaign.DiscountType,
                DiscountValue = campaign.DiscountValue,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                IsActive = campaign.IsActive,
                CreatedAt = campaign.CreatedAt,
                UpdatedAt = campaign.UpdatedAt,
                Products = campaign.CampaignProducts.Select(cp => new DiscountCampaignProductDto
                {
                    FoodItemId = cp.FoodItemId,
                    FoodName = cp.FoodItem?.Name ?? string.Empty,
                    Price = cp.FoodItem?.Price ?? 0
                }).ToList(),
                Combos = campaign.CampaignCombos.Select(cc => new DiscountCampaignComboDto
                {
                    ComboId = cc.ComboId,
                    ComboName = cc.Combo?.Name ?? string.Empty,
                    Price = cc.Combo?.Price ?? 0
                }).ToList()
            };
        }
    }
}
