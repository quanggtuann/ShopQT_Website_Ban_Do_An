using ShopDAL.Models;

namespace ShopAPI.DTOs
{
    public class DiscountCampaignFilterViewModel
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public DiscountType? DiscountType { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class DiscountCampaignDto
    {
        public int DiscountCampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<DiscountCampaignProductDto> Products { get; set; } = new();
        public List<DiscountCampaignComboDto> Combos { get; set; } = new();
    }

    public class DiscountCampaignProductDto
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class DiscountCampaignComboDto
    {
        public int ComboId { get; set; }
        public string ComboName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class CreateDiscountCampaignRequest
    {
        public string CampaignName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.Percent;
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int> FoodItemIds { get; set; } = new();
        public List<int> ComboIds { get; set; } = new();
    }

    public class UpdateDiscountCampaignRequest : CreateDiscountCampaignRequest
    {
    }
}
