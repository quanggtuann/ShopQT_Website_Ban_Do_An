using ShopDAL.Models.Dto;

namespace ShopView.Models
{
    public class ComboCreateViewModel
    {
        public CreateComboRequest Combo { get; set; } = new();
        public List<FoodItemDto> AvailableFoods { get; set; } = new();
        public string ImageBaseUrl { get; set; } = "https://localhost:7130/";
    }
}