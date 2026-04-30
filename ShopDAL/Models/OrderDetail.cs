namespace ShopDAL.Models
{
    public class OrderDetail
    {
        public int OrderDetailID {  get; set; } 
        public int Quantity {  get; set; }
        public decimal? Price {  get; set; }
        public int OrderID {  get; set; }
        public virtual Order Order { get; set; }
        public int ? FoodItemID { get; set; }
        public virtual FoodItem FoodItem { get; set; }
        public int ComboID {  get; set; }
        public virtual Combo Combo { get; set; }
    }
}
