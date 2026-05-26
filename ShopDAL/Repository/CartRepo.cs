using Microsoft.EntityFrameworkCore;
using ShopDAL.Context;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopDAL.Repository
{
    public class CartRepo : ICartRepo
    {
        private readonly ApplicationDbContext _dbContext;

        public CartRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Cart GetByUserID(int id)
        {
            return _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.FoodItem)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Combo)
                .FirstOrDefault(c => c.UserID == id);
        }

        public void AddCart(Cart cart)
        {
            _dbContext.Carts.Add(cart);
        }
        public FoodItem GetFoodItem(int id)
        {
            return _dbContext.FoodItems.Find(id);
        }
        public Combo GetCombo(int id)
        {
            return _dbContext.Combos.Find(id);
        }
        public CartItem GetCartItem(int id)
        {
            return _dbContext.CartItems
                .Include(ci => ci.FoodItem)
                .Include(ci => ci.Combo)
                .FirstOrDefault(ci => ci.CartItemId == id);
        }
        public void RemoveCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Remove(cartItem);
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }

    }
}
