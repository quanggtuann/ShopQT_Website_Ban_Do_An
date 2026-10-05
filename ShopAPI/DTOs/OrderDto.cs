namespace ShopAPI.DTOs
{
    public class CreateOrderRequest
    {
        public int AddressId { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? Note { get; set; }
    }

    public class CreateOrderResponse
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class CancelOrderRequest
    {
        public string CancelReason { get; set; } = string.Empty;
        public string? CancelReasonDetail { get; set; }
    }

    public class CustomerOrderDto
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
        public List<CustomerOrderDetailDto> Items { get; set; } = new();
    }

    public class CustomerOrderDetailDto
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

    public class AdminOrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
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
        public List<AdminOrderDetailDto> Items { get; set; } = new();
    }

    public class AdminOrderDetailDto
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
}
