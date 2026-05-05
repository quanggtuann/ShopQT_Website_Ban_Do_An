using System.ComponentModel.DataAnnotations;

namespace ShopView.Areas.Admin.Models
{
    // For Combo Index
    public class ComboIndexViewModel
    {
        public ComboFilterViewModel Filter { get; set; }
        public List<ComboDto> Combos { get; set; } = new();
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public string ImageBaseUrl { get; set; }
    }

    public class ComboFilterViewModel
    {
        public string? KeyWord { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
        public string? ShortBy { get; set; }
        public string? ShortOrder { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }

    public class ComboDto
    {
        public int ComboId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public bool IsVaiLabel { get; set; }
        public List<ComboFoodItemDto> FoodItems { get; set; } = new();
    }

    public class ComboFoodItemDto
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
    }

    // For Create/Edit
    public class ComboCreateViewModel
    {
        public CreateComboRequest Combo { get; set; }
        public List<FoodOptionDto> AvailableFoods { get; set; } = new();
        public string ImageBaseUrl { get; set; }
    }

    public class CreateComboRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public List<ComboFoodItemRequest> FoodItems { get; set; } = new();
    }

    public class ComboFoodItemRequest
    {
        public int FoodItemId { get; set; }
        public int Quantity { get; set; }
    }

    // Response from API
    public class ComboListResponse
    {
        public List<ComboDto> Data { get; set; }
        public int ToTalPage { get; set; }
        public int CurrentPage { get; set; }
    }

    // FoodItem DTO for dropdown
    public class FoodOptionDto
    {
        public int FoodItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
    }
}
