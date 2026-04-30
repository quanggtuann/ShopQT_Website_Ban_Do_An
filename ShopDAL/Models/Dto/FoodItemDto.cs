namespace ShopDAL.Models.Dto
{
    public class FoodItemDto
    {
        public int FoodItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreateDate { get; set; }
        public string ImagePath { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
