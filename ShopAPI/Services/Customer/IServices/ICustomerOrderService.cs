using ShopAPI.DTOs;

namespace ShopAPI.Services.Customer.IServices
{
    public interface ICustomerOrderService
    {
        CreateOrderResponse CreateOrderFromCart(int userId, CreateOrderRequest request);
        PagedResult<CustomerOrderDto> GetOrders(int userId, int page, int pageSize);
        CustomerOrderDto? GetOrderDetail(int userId, int orderId);
        void CancelOrder(int userId, int orderId, CancelOrderRequest request);
    }
}
