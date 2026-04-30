namespace ShopDAL.Models
{
    public class ComboFoodItem
    {
        public int ComboId { get; set; }
        public virtual Combo Combo { get; set; }
        public int FoodItemID {  get; set; }
        public virtual FoodItem FoodItem { get; set; }
        public int Quantity {  get; set; }
        
    }
}
