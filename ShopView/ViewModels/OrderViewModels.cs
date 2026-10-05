using System.ComponentModel.DataAnnotations;

namespace ShopView.ViewModels
{
    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; } = new();
        public List<AddressViewModel> Addresses { get; set; } = new();

        [Required]
        public int AddressId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "Cash";

        public string? Note { get; set; }

        public decimal GrandTotal => Cart.CartItems.Sum(item => (item.Price ?? 0) * item.Quantity);
    }

    public class CreateOrderApiRequest
    {
        public int AddressId { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? Note { get; set; }
    }

    public class CreateOrderApiResponse
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class CustomerOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int? AddressId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public string? Note { get; set; }
        public string? CancelReason { get; set; }
        public string? CancelReasonDetail { get; set; }
        public string? CancelledBy { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public List<CustomerOrderDetailViewModel> Items { get; set; } = new();
        public bool CanCancel => string.Equals(Status, "Pending", StringComparison.OrdinalIgnoreCase);
    }

    public class CancelOrderApiRequest
    {
        public string CancelReason { get; set; } = string.Empty;
        public string? CancelReasonDetail { get; set; }
    }

    public class CustomerOrderDetailViewModel
    {
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public int? FoodItemId { get; set; }
        public int? ComboId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string ItemType { get; set; } = string.Empty;
    }

    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
