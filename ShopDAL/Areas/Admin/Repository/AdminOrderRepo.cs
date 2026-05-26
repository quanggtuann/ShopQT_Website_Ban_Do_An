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
            return _context.Orders.OrderByDescending(o => o.OrderId).ToList();
        }
        public Order GetById(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.OrderId == id);
        }
        public void CancelOrder(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("order not found"); 
            }
            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("only pending order can be cancelled");
            }
            order.Status = OrderStatus.Pending;
            _context.SaveChanges();
        }
        public void UpdateNextStatus(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("order not found");
            }
            if (order.Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException("Compeleted order can not be processed anymore");
            }
            if ( order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("cancelled order can not be processed anymore");
            }
            if(order.Status == OrderStatus.Pending)
            {
                order.Status=OrderStatus.Confirmed;
            }
            if (order.Status == OrderStatus.Confirmed)
            {
                order.Status=OrderStatus.Shipping;
            }
            if (order.Status == OrderStatus.Completed)
            {
                order.Status=OrderStatus.Completed;
            }
            _context.SaveChanges();
        }
    }
}
