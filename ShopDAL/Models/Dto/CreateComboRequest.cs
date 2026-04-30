namespace ShopDAL.Models.Dto
{
    public class CreateComboRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailabale { get; set; }
        public List<ComboFoodItemRequest> FoodItems { get; set; }
    }

    public class ComboFoodItemRequest
    {
        public int FoodItemId { get; set; }
        public int Quantity { get; set; }
    }
}
