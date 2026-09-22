using Microsoft.EntityFrameworkCore;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;
using ShopDAL.Models;

namespace ShopDAL.Areas.Repository
{
    public class AdminStatisticsRepo : IAdminStatisticsRepo
    {
        private readonly ApplicationDbContext _context;

        public AdminStatisticsRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Order> GetOrders()
        {
            return _context.Orders
                .AsNoTracking()
                .Include(order => order.User)
                .Include(order => order.OrderDetail)
                    .ThenInclude(detail => detail.FoodItem)
                .Include(order => order.OrderDetail)
                    .ThenInclude(detail => detail.Combo);
        }

        public IQueryable<User> GetUsers()
        {
            return _context.Users.AsNoTracking();
        }

        public IQueryable<FoodItem> GetFoods()
        {
            return _context.FoodItems.AsNoTracking();
        }

        public IQueryable<Combo> GetCombos()
        {
            return _context.Combos.AsNoTracking();
        }
    }
}
