namespace ShopDAL.Models
{
    //tim kiem loc mon an theo gia ,key,category
    public class FoodItemFilterViewModel
    {
        public string? Keyword { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public int? categoryID { get; set; }
        public string? SortBy { get; set; } = "name";
        public string? SortOrder { get; set; } = "asc";
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 5;
        public List<FoodItem>? FoodItems { get; set; }
        public List<Category>? categories { get; set; }
    }
}
