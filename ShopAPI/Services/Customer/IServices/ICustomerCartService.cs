using ShopAPI.DTOs;
using ShopDAL.Models;

namespace ShopAPI.Services.Customer.IServices
{
    public interface ICustomerCartService
    {
        Cart GetByUserId(int id);
        void AddToCart(AddToCartRequest addToCartRequest);
        void AddComboToCart(AddComboToCartRequest addComboToCartRequest);
        void RemoveCartItem(int cartItemId);
        void Update(UpdateCartItem updateCartItem);
    }
}
