using ShopAPI.DTOs;
using ShopAPI.Services.IServices;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Models;

namespace ShopAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAdminOrderRepo _orderRepo;

        public OrderService(IAdminOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public PagedResult<AdminOrderDto> GetAll(OrderFilterViewModel filter)
        {
            filter.Page = filter.Page <= 0 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var query = _orderRepo.GetAll().AsQueryable();

            if (filter.OrderId.HasValue)
            {
                query = query.Where(order => order.OrderId == filter.OrderId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(order => order.TotalAmount >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(order => order.TotalAmount <= filter.MaxPrice.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(order => order.OrderTime.Date >= filter.FromDate.Value.Date);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(order => order.OrderTime.Date <= filter.ToDate.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status)
                && Enum.TryParse<OrderStatus>(filter.Status, true, out var status))
            {
                query = query.Where(order => order.Status == status);
            }

            query = ApplySort(query, filter.Shortby, filter.ShotOrder);

            var totalItems = query.Count();
            var orders = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(ToDto)
                .ToList();

            return new PagedResult<AdminOrderDto>
            {
                Data = orders,
                TotalItems = totalItems,
                CurrentPage = filter.Page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize)
            };
        }

        public AdminOrderDto GetById(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            return ToDto(order);
        }

        public void CancelOrder(int orderId, CancelOrderRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CancelReason))
            {
                throw new InvalidOperationException("Cancellation reason is required");
            }

            _orderRepo.CancelOrder(orderId, request.CancelReason.Trim(), "Admin");
        }

        public void UpdateNextStatus(int orderId)
        {
            _orderRepo.UpdateNextStatus(orderId);
        }

        private static IQueryable<Order> ApplySort(IQueryable<Order> query, string? sortBy, string? sortOrder)
        {
            var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            return (sortBy ?? "OrderId").ToLowerInvariant() switch
            {
                "total" or "totalamount" => isDescending
                    ? query.OrderByDescending(order => order.TotalAmount)
                    : query.OrderBy(order => order.TotalAmount),
                "date" or "ordertime" => isDescending
                    ? query.OrderByDescending(order => order.OrderTime)
                    : query.OrderBy(order => order.OrderTime),
                "status" => isDescending
                    ? query.OrderByDescending(order => order.Status)
                    : query.OrderBy(order => order.Status),
                _ => isDescending
                    ? query.OrderByDescending(order => order.OrderId)
                    : query.OrderBy(order => order.OrderId)
            };
        }

        private static AdminOrderDto ToDto(Order order)
        {
            return new AdminOrderDto
            {
                OrderId = order.OrderId,
                OrderTime = order.OrderTime,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                UserId = order.UserId,
                Username = order.User?.Username ?? $"User #{order.UserId}",
                AddressId = order.AddressId,
                PaymentMethod = order.PaymentMethod.ToString(),
                IsPaid = order.IsPaid,
                Note = order.Note,
                CancelReason = order.CancelReason,
                CancelReasonDetail = order.CancelReasonDetail,
                CancelledBy = order.CancelledBy,
                CancelledAt = order.CancelledAt,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderDetail.Select(ToDetailDto).ToList()
            };
        }

        private static AdminOrderDetailDto ToDetailDto(OrderDetail detail)
        {
            var isFood = detail.FoodItemID.HasValue;

            return new AdminOrderDetailDto
            {
                OrderDetailId = detail.OrderDetailID,
                Quantity = detail.Quantity,
                Price = detail.Price,
                Subtotal = detail.Price * detail.Quantity,
                FoodItemId = detail.FoodItemID,
                ComboId = detail.ComboID,
                ProductName = isFood
                    ? detail.FoodItem?.Name ?? "Food item"
                    : detail.Combo?.Name ?? "Combo",
                ImagePath = isFood
                    ? detail.FoodItem?.ImagePath
                    : detail.Combo?.ImagePath,
                ItemType = isFood ? "Food" : "Combo"
            };
        }
    }
}
