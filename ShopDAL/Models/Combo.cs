using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace ShopDAL.Models
{
    public class Combo
    {
        public int ComboId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, int.MaxValue)]
        public decimal Price {  get; set; }
        public DateTime CreateDate { get; set; }
        [MaxLength(3000)]
        public string? ImagePath { get; set; }
        public bool IsAvailabale {  get; set; }
        public virtual ICollection<ComboFoodItem> ComboFoodItem { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<CartItem> CartItem { get; set; }

    }
}
