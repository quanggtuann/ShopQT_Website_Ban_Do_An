namespace ShopView.ViewModels
{
    public class CartViewModel
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new();
    }

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int? FoodItemID { get; set; }
        public FoodLiteViewModel? FoodItem { get; set; }
        public int? ComboID { get; set; }
        public ComboLiteViewModel? Combo { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
    }

    public class FoodLiteViewModel
    {
        public string? ImagePath { get; set; }
        public string? Name { get; set; }
    }

    public class ComboLiteViewModel
    {
        public string? ImagePath { get; set; }
        public string? Name { get; set; }
    }

    public class AddFoodToCartApiRequest
    {
        public int FoodItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddComboToCartApiRequest
    {
        public int ComboId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemApiRequest
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
