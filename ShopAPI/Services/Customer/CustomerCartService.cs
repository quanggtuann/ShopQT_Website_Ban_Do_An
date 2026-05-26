using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;
using ShopAPI.DTOs;

namespace ShopAPI.Services.Customer
{
    public class CustomerCartService : ICustomerCartService
    {
        private readonly ICartRepo _cartRepo;
        public CustomerCartService(ICartRepo cartRepo)
        {
            _cartRepo = cartRepo;
        }
        public Cart GetByUserId(int id)
        {
            return _cartRepo.GetByUserID(id);
        }
        public void AddToCart(AddToCartRequest addToCartRequest)
        {
            var userCart = _cartRepo.GetByUserID(addToCartRequest.UserId);
            if (userCart == null)
            {
                userCart = new Cart
                {
                    UserID = addToCartRequest.UserId,
                    CartItems = new List<CartItem>()
                };
                _cartRepo.AddCart(userCart);
            }

            userCart.CartItems ??= new List<CartItem>();
            var FoodItem = _cartRepo.GetFoodItem(addToCartRequest.FoodItemId);
            if (FoodItem == null)
            {
                throw new KeyNotFoundException("Food item not found");
            }
            if (addToCartRequest.Quantity <= 0)
            {
                throw new ArgumentException("quantity must > 0");
            }
            var cartItem = userCart.CartItems.FirstOrDefault(
                x => x.FoodItemID == addToCartRequest.FoodItemId
                );
            if (cartItem != null)
            {
                cartItem.Quantity += addToCartRequest.Quantity;
                cartItem.Price = FoodItem.Price;
            }
            else
            {
                userCart.CartItems.Add(new CartItem
                {
                    FoodItemID = addToCartRequest.FoodItemId,
                    Quantity = addToCartRequest.Quantity,
                    Price = FoodItem.Price,
                });
            }
            _cartRepo.Save();
        }
        public void AddComboToCart(AddComboToCartRequest addComboToCartRequest)
        {
            var userCart = _cartRepo.GetByUserID(addComboToCartRequest.UserId);
            if (userCart == null)
            {
                userCart = new Cart
                {
                    UserID = addComboToCartRequest.UserId,
                    CartItems = new List<CartItem>()
                };
                _cartRepo.AddCart(userCart);
            }

            userCart.CartItems ??= new List<CartItem>();
            var combo = _cartRepo.GetCombo(addComboToCartRequest.ComboId);
            if (combo == null)
            {
                throw new KeyNotFoundException("Combo not found");
            }
            if (addComboToCartRequest.Quantity <= 0)
            {
                throw new ArgumentException("quantity must >0");
            }
            var cartItem = userCart.CartItems.FirstOrDefault(
                x => x.ComboID == addComboToCartRequest.ComboId
                );
            if (cartItem != null)
            {
                cartItem.Quantity += addComboToCartRequest.Quantity;
                cartItem.Price = combo.Price;
            }
            else
            {
                userCart.CartItems.Add(new CartItem
                {
                    ComboID = addComboToCartRequest.ComboId,
                    Quantity = addComboToCartRequest.Quantity,
                    Price = combo.Price,
                });
            }
            _cartRepo.Save();
        }
        public void RemoveCartItem(int cartItemId)
        {
            var cartItem = _cartRepo.GetCartItem(cartItemId);
            if (cartItem == null)
            {
                throw new KeyNotFoundException("Cart Item Not Found");
            }
            _cartRepo.RemoveCartItem(cartItem);
            _cartRepo.Save();
        }
        public void Update(UpdateCartItem updateCartItem)
        {
            var cartItem = _cartRepo.GetCartItem(updateCartItem.CartItemId);
            if (cartItem == null)
            {
                throw new KeyNotFoundException("Cart item not found");
            }

            if (updateCartItem.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must > 0");
            }
            cartItem.Quantity = updateCartItem.Quantity;
            if (cartItem.FoodItemID != null)
            {
                cartItem.Price = cartItem.FoodItem.Price;
            }
            if (cartItem.ComboID != null)
            {
                cartItem.Price=cartItem.Combo.Price;
            }
            _cartRepo.Save();
        }
    }
}
