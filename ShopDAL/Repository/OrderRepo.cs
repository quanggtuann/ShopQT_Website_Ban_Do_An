using Microsoft.EntityFrameworkCore;
using ShopDAL.Context;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopDAL.Repository
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Order order)
        {
            _dbContext.Orders.Add(order);
        }

        public IQueryable<Order> GetByUserId(int userId)
        {
            return _dbContext.Orders
                .AsNoTracking()
                .Include(order => order.Address)
                .Include(order => order.OrderDetail)
                    .ThenInclude(detail => detail.FoodItem)
                .Include(order => order.OrderDetail)
                    .ThenInclude(detail => detail.Combo)
                .Where(order => order.UserId == userId)
                .OrderByDescending(order => order.OrderId);
        }

        public Order? GetByIdForUser(int orderId, int userId)
        {
            return _dbContext.Orders
                .AsNoTracking()
                .Include(order => order.Address)
                .Include(order => order.OrderDetail)
                    .ThenInclude(detail => detail.FoodItem)
                .Include(order => order.OrderDetail)
                    .ThenInclude(detail => detail.Combo)
                .FirstOrDefault(order => order.OrderId == orderId && order.UserId == userId);
        }

        public Order? GetTrackedByIdForUser(int orderId, int userId)
        {
            return _dbContext.Orders
                .FirstOrDefault(order => order.OrderId == orderId && order.UserId == userId);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
