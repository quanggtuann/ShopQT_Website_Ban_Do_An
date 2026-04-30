using System.ComponentModel.DataAnnotations;

namespace ShopDAL.Models
{
    public class FoodItem
    {
        public int FoodItemId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Range(0,double.MaxValue)]
        public decimal Price {  get; set; }
        public bool IsAvailable {  get; set; }
        public DateTime CreateDate { get; set; }
        [MaxLength(5000)]
        public string? ImagePath { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<ComboFoodItem>? ComboFoodItem { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetail { get; set; }
        public virtual ICollection<CartItem>? CartItem { get; set; }
    }
}
