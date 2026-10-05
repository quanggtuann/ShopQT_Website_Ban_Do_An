namespace ShopView.Areas.Admin.ViewModel
{
    public class AdminDashboardViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public AdminStatisticsViewModel Statistics { get; set; } = new();
        public string ImageBaseUrl { get; set; } = "https://localhost:7130/";
    }

    public class AdminStatisticsViewModel
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
        public List<OrderStatusStatisticViewModel> StatusStatistics { get; set; } = new();
        public List<DailyRevenueViewModel> DailyRevenue { get; set; } = new();
        public List<TopSellingItemViewModel> TopFoods { get; set; } = new();
        public List<TopSellingItemViewModel> TopCombos { get; set; } = new();
        public List<RecentOrderStatisticViewModel> RecentOrders { get; set; } = new();
    }

    public class OrderStatusStatisticViewModel
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DailyRevenueViewModel
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopSellingItemViewModel
    {
        public int? ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RecentOrderStatisticViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
