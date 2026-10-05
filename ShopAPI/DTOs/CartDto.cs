namespace ShopAPI.DTOs
{
    public class AddToCartRequest
    {
        public int UserId { get; set; }
        public int FoodItemId { get; set; }
        public int Quantity { get; set; }
    }
    public class AddComboToCartRequest
    {
        public int UserId { get; set; }
        public int ComboId { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateCartItem
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
