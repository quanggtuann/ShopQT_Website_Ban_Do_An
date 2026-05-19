using ShopAPI.DTOs;

namespace ShopView.ViewModels
{
    public class ComboViewModels
    {
        public ComboFilterViewModels Filter { get; set; } = new();
        public PagedResult<ComboDto> PagedResult { get; set; }=new();
        public List<FoodItemDto> FoodItems { get; set; } = new();
        public string ImageBaseUrl { get; set; }= string.Empty;

    }
    public class ComboFilterViewModels
    {
        public string? Keyword { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public int? categoryID { get; set; }
        public string? SortBy { get; set; } = "name";
        public string? SortOrder { get; set; } = "asc";
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 5;
    }
}
