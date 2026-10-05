namespace ShopAPI.DTOs
{
    public class AdminStatisticsDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalFoods { get; set; }
        public int TotalCombos { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal CompletedRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }
        public List<OrderStatusStatisticDto> StatusStatistics { get; set; } = new();
        public List<DailyRevenueDto> DailyRevenue { get; set; } = new();
        public List<TopSellingItemDto> TopFoods { get; set; } = new();
        public List<TopSellingItemDto> TopCombos { get; set; } = new();
        public List<RecentOrderStatisticDto> RecentOrders { get; set; } = new();
    }

    public class OrderStatusStatisticDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopSellingItemDto
    {
        public int? ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RecentOrderStatisticDto
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
