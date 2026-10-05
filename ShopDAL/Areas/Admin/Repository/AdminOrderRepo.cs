using Microsoft.EntityFrameworkCore;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;
using ShopDAL.Models;

namespace ShopDAL.Areas.Repository
{
    public class AdminOrderRepo : IAdminOrderRepo
    {
        private readonly ApplicationDbContext _context;
        public AdminOrderRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Order> GetAll()
        {
            return _context.Orders.AsNoTracking()
                .Include(o=>o.User)
                .Include(o=>o.Address)
                .Include(o=>o.OrderDetail)
                .OrderByDescending(o => o.OrderId).ToList();
        }
        public Order? GetById(int id)
        {
            return _context.Orders.AsNoTracking()
                .Include(o=>o.OrderDetail)
                .ThenInclude(od=>od.FoodItem)
                .Include(o=>o.OrderDetail)
                .ThenInclude(od=>od.Combo)
                .Include(o=>o.User)
                .Include(o=>o.Address)
                .FirstOrDefault(o => o.OrderId == id);
        }
        public void CancelOrder(int orderId, string cancelReason, string cancelledBy)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("order not found");
            }
            if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed or OrderStatus.Shipping))
            {
                throw new InvalidOperationException("Only pending, confirmed or shipping orders can be cancelled");
            }
            order.Status = OrderStatus.Cancelled;
            order.CancelReason = cancelReason;
            order.CancelReasonDetail = null;
            order.CancelledBy = cancelledBy;
            order.CancelledAt = DateTime.Now;
            _context.SaveChanges();
        }
        public void UpdateNextStatus(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("order not found");
            }
            order.Status = order.Status switch
            {
                OrderStatus.Pending => OrderStatus.Confirmed,
                OrderStatus.Confirmed => OrderStatus.Shipping,
                OrderStatus.Shipping => OrderStatus.Completed,
                OrderStatus.Completed => throw new InvalidOperationException("Completed order cannot be processed anymore"),
                OrderStatus.Cancelled => throw new InvalidOperationException("Cancelled order cannot be processed anymore"),
                _=> throw new InvalidOperationException("Invalid order status")
            };
            if (order.Status == OrderStatus.Completed)
            {
                order.CompletedAt ??= DateTime.Now;
            }
            _context.SaveChanges();
        }
    }
}
