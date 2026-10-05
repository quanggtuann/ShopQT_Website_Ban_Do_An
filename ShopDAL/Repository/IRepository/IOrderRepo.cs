using ShopDAL.Models;

namespace ShopDAL.Repository.IRepository
{
    public interface IOrderRepo
    {
        void Add(Order order);
        IQueryable<Order> GetByUserId(int userId);
        Order? GetByIdForUser(int orderId, int userId);
        Order? GetTrackedByIdForUser(int orderId, int userId);
        void Save();
    }
}
