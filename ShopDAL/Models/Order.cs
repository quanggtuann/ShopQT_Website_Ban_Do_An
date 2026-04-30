using System.ComponentModel.DataAnnotations;

namespace ShopDAL.Models
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipping,
        Completed,
        Cancelled

    }
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? TotalAmount { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
