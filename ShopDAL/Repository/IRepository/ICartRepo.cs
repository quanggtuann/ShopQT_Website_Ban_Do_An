using ShopDAL.Models;

namespace ShopDAL.Repository.IRepository
{
    public interface ICartRepo
   {
        Cart GetByUserID(int id);
        void AddCart(Cart cart);
        FoodItem GetFoodItem(int id);
        Combo GetCombo(int id);
        CartItem GetCartItem(int id);
        void RemoveCartItem(CartItem cartItem);
        void Save();
    }
}
