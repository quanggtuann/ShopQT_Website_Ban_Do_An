using ShopAPI.DTOs;
using ShopAPI.Services.IServices;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;

namespace ShopAPI.Services
{
    public class AdminStatisticsService : IAdminStatisticsService
    {
        private readonly IAdminStatisticsRepo _statisticsRepo;

        public AdminStatisticsService(IAdminStatisticsRepo statisticsRepo)
        {
            _statisticsRepo = statisticsRepo;
        }

        public AdminStatisticsDto GetStatistics(DateTime? fromDate, DateTime? toDate)
        {
            var startDate = (fromDate ?? DateTime.Today.AddDays(-29)).Date;
            var endDate = (toDate ?? DateTime.Today).Date;

            if (endDate < startDate)
            {
                throw new InvalidOperationException("To date must be greater than or equal to from date.");
            }

            var allOrders = _statisticsRepo.GetOrders().ToList();
            var filteredOrders = allOrders
                .Where(order => order.OrderTime.Date >= startDate && order.OrderTime.Date <= endDate)
                .ToList();

            var grossRevenueOrders = filteredOrders
                .Where(order => order.Status != OrderStatus.Cancelled)
                .ToList();

            var revenueOrders = filteredOrders
                .Where(order => order.Status == OrderStatus.Completed)
                .ToList();

            var completedOrders = filteredOrders
                .Where(order => order.Status == OrderStatus.Completed)
                .ToList();

            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);

            return new AdminStatisticsDto
            {
                FromDate = startDate,
                ToDate = endDate,
                TotalCustomers = _statisticsRepo.GetUsers().Count(user => user.Role == "customer"),
                TotalFoods = _statisticsRepo.GetFoods().Count(),
                TotalCombos = _statisticsRepo.GetCombos().Count(),
                TotalOrders = filteredOrders.Count,
                PendingOrders = filteredOrders.Count(order => order.Status == OrderStatus.Pending),
                ConfirmedOrders = filteredOrders.Count(order => order.Status == OrderStatus.Confirmed),
                ShippingOrders = filteredOrders.Count(order => order.Status == OrderStatus.Shipping),
                CompletedOrders = filteredOrders.Count(order => order.Status == OrderStatus.Completed),
                CancelledOrders = filteredOrders.Count(order => order.Status == OrderStatus.Cancelled),
                GrossRevenue = grossRevenueOrders.Sum(order => order.TotalAmount),
                CompletedRevenue = completedOrders.Sum(order => order.TotalAmount),
                TodayRevenue = allOrders
                    .Where(order => order.Status == OrderStatus.Completed
                        && (order.CompletedAt ?? order.OrderTime).Date == today)
                    .Sum(order => order.TotalAmount),
                MonthRevenue = allOrders
                    .Where(order => order.Status == OrderStatus.Completed
                        && (order.CompletedAt ?? order.OrderTime).Date >= monthStart
                        && (order.CompletedAt ?? order.OrderTime).Date <= today)
                    .Sum(order => order.TotalAmount),
                YearRevenue = allOrders
                    .Where(order => order.Status == OrderStatus.Completed
                        && (order.CompletedAt ?? order.OrderTime).Date >= yearStart
                        && (order.CompletedAt ?? order.OrderTime).Date <= today)
                    .Sum(order => order.TotalAmount),
                StatusStatistics = BuildStatusStatistics(filteredOrders),
                DailyRevenue = BuildDailyRevenue(revenueOrders, startDate, endDate),
                TopFoods = BuildTopFoods(revenueOrders),
                TopCombos = BuildTopCombos(revenueOrders),
                RecentOrders = BuildRecentOrders(filteredOrders)
            };
        }

        private static List<OrderStatusStatisticDto> BuildStatusStatistics(List<Order> orders)
        {
            return Enum.GetValues<OrderStatus>()
                .Select(status => new OrderStatusStatisticDto
                {
                    Status = status.ToString(),
                    Count = orders.Count(order => order.Status == status),
                    Revenue = orders
                        .Where(order => order.Status == status && order.Status != OrderStatus.Cancelled)
                        .Sum(order => order.TotalAmount)
                })
                .ToList();
        }

        private static List<DailyRevenueDto> BuildDailyRevenue(List<Order> orders, DateTime startDate, DateTime endDate)
        {
            var days = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset))
                .ToList();

            return days.Select(day => new DailyRevenueDto
            {
                Date = day,
                OrderCount = orders.Count(order => order.OrderTime.Date == day),
                Revenue = orders
                    .Where(order => (order.CompletedAt ?? order.OrderTime).Date == day)
                    .Sum(order => order.TotalAmount)
            }).ToList();
        }

        private static List<TopSellingItemDto> BuildTopFoods(List<Order> orders)
        {
            return orders
                .SelectMany(order => order.OrderDetail)
                .Where(detail => detail.FoodItemID.HasValue)
                .GroupBy(detail => new
                {
                    detail.FoodItemID,
                    Name = detail.FoodItem?.Name ?? "Food item",
                    detail.FoodItem?.ImagePath
                })
                .Select(group => new TopSellingItemDto
                {
                    ItemId = group.Key.FoodItemID,
                    Name = group.Key.Name,
                    ImagePath = group.Key.ImagePath,
                    ItemType = "Food",
                    QuantitySold = group.Sum(detail => detail.Quantity),
                    Revenue = group.Sum(detail => detail.Price * detail.Quantity)
                })
                .OrderByDescending(item => item.QuantitySold)
                .ThenByDescending(item => item.Revenue)
                .Take(5)
                .ToList();
        }

        private static List<TopSellingItemDto> BuildTopCombos(List<Order> orders)
        {
            return orders
                .SelectMany(order => order.OrderDetail)
                .Where(detail => detail.ComboID.HasValue)
                .GroupBy(detail => new
                {
                    detail.ComboID,
                    Name = detail.Combo?.Name ?? "Combo",
                    detail.Combo?.ImagePath
                })
                .Select(group => new TopSellingItemDto
                {
                    ItemId = group.Key.ComboID,
                    Name = group.Key.Name,
                    ImagePath = group.Key.ImagePath,
                    ItemType = "Combo",
                    QuantitySold = group.Sum(detail => detail.Quantity),
                    Revenue = group.Sum(detail => detail.Price * detail.Quantity)
                })
                .OrderByDescending(item => item.QuantitySold)
                .ThenByDescending(item => item.Revenue)
                .Take(5)
                .ToList();
        }

        private static List<RecentOrderStatisticDto> BuildRecentOrders(List<Order> orders)
        {
            return orders
                .OrderByDescending(order => order.OrderTime)
                .Take(8)
                .Select(order => new RecentOrderStatisticDto
                {
                    OrderId = order.OrderId,
                    OrderTime = order.OrderTime,
                    CustomerName = order.User?.Username ?? $"User #{order.UserId}",
                    Status = order.Status.ToString(),
                    ItemCount = order.OrderDetail.Sum(detail => detail.Quantity),
                    TotalAmount = order.TotalAmount
                })
                .ToList();
        }
    }
}
