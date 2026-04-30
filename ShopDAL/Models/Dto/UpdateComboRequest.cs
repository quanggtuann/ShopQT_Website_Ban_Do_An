namespace ShopDAL.Models.Dto
{
    public class UpdateComboRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailabale { get; set; }
        public List<ComboFoodItemRequest>? FoodItems { get; set; }
        public bool RemoveImage { get; set; } = false; 
    }
}
