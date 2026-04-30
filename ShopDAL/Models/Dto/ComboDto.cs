namespace ShopDAL.Models.Dto
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
}
