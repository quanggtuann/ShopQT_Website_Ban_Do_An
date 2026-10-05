using ShopDAL.Models;

namespace ShopDAL.Areas.Repository.Irepository
{
    public interface IAdminOrderRepo
    {
        List<Order> GetAll();
        Order? GetById(int id);
        void CancelOrder(int orderId, string cancelReason, string cancelledBy);
        void UpdateNextStatus(int orderId);
    }
}
