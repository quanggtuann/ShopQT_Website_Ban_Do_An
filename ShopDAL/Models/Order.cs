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
    public enum PaymentMethod
    {
        Cash,
        VNPay
    }
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int? AddressId { get; set; }
        public virtual Address? Address { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; } = false;
        public string? Note { get; set; }
        [MaxLength(500)]
        public string? CancelReason { get; set; }
        public string? CancelReasonDetail { get; set; }
        [MaxLength(50)]
        public string? CancelledBy { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ShippingAddress { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
            = new List<OrderDetail>();
    }
}
