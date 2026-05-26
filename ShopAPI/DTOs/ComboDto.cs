namespace ShopAPI.DTOs
{
    public class ComboDto
    {
        public int ComboId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsVaiLabel { get; set; }
        public DateTime CreateDate { get; set; }
        public string ImagePath { get; set; }


        public virtual ICollection<ComboFoodItemDto> FoodItems { get; set; }
    }
    public class ComboFoodItemDto
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; }    
        public int Quantity { get; set; }
    }
    public class CreateComboRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailabale { get; set; } = true;
        public string? ImagePath { get; set; }
        public List<ComboFoodItemRequest> FoodItems { get; set; }
    }

    public class ComboFoodItemRequest
    {
        public int FoodItemId { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateComboRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailabale { get; set; } = true;
        public string? ImagePath { get; set; }
        public List<ComboFoodItemRequest>? FoodItems { get; set; }
        public bool RemoveImage { get; set; } = false;
    }
}
