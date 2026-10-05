namespace ShopView.Areas.Admin.ViewModel
{
    public class AdminOrderIndexViewModel
    {
        public AdminOrderFilterViewModel Filter { get; set; } = new();
        public PagedResponse<AdminOrderDto> PagedResult { get; set; } = new();
        public List<string> StatusOptions { get; set; } = new()
        {
            "Pending",
            "Confirmed",
            "Shipping",
            "Completed",
            "Cancelled"
        };
    }

    public class AdminOrderFilterViewModel
    {
        public int? OrderId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AdminOrderDetailViewModel
    {
        public AdminOrderDto Order { get; set; } = new();
        public string ImageBaseUrl { get; set; } = "https://localhost:7130/";
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

        public bool CanCancel =>
            string.Equals(Status, "Pending", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Status, "Confirmed", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Status, "Shipping", StringComparison.OrdinalIgnoreCase);

        public bool CanMoveNext =>
            !string.Equals(Status, "Completed", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(Status, "Cancelled", StringComparison.OrdinalIgnoreCase);

        public string NextStatusText => Status.ToLowerInvariant() switch
        {
            "pending" => "Confirm order",
            "confirmed" => "Start shipping",
            "shipping" => "Complete order",
            _ => "Update status"
        };
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
