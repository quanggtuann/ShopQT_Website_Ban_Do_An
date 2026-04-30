using System.ComponentModel.DataAnnotations;

namespace ShopDAL.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<FoodItem>? FoodItems { get; set; }
    }
}
