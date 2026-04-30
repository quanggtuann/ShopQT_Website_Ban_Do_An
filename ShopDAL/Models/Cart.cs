using System.ComponentModel.DataAnnotations;

namespace ShopDAL.Models
{
    public class Cart
    {
        public int CartID {  get; set; }
        [Required]
        public int UserID {  get; set; }
        public virtual User Users { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }

    }
}
