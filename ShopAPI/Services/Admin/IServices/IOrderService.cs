using ShopAPI.DTOs;
using ShopDAL.Models;

namespace ShopAPI.Services.IServices
{
    public interface IOrderService
    {
        PagedResult<AdminOrderDto> GetAll(OrderFilterViewModel filter);
        AdminOrderDto GetById(int id);
        void CancelOrder(int orderId, CancelOrderRequest request);
        void UpdateNextStatus(int orderId);
    }
}
