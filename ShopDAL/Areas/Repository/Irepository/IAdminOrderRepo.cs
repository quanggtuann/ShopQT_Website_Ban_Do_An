using ShopDAL.Models;

namespace ShopDAL.Areas.Repository.Irepository
{
    public interface IAdminOrderRepo
    {
        List<Order> GetAll();
        Order GetById(int id);
       void CancelOrder(int orderId);
        void UpdateNextStatus(int orderId);
    }
}
