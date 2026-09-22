using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Services.Customer
{
    public class CustomerCartService : ICustomerCartService
    {
        private readonly ICartRepo _cartRepo;
        private readonly ICustomerDiscountPriceService _discountPriceService;

        public CustomerCartService(ICartRepo cartRepo, ICustomerDiscountPriceService discountPriceService)
        {
            _cartRepo = cartRepo;
            _discountPriceService = discountPriceService;
        }

        public Cart GetByUserId(int id)
        {
            var cart = _cartRepo.GetByUserID(id);
            if (cart == null)
            {
                return cart;
            }

            var hasPriceChanged = false;
            foreach (var item in cart.CartItems ?? new List<CartItem>())
            {
                if (item.FoodItemID != null && item.FoodItem != null)
                {
                    var price = _discountPriceService.GetFoodDiscount(item.FoodItemID.Value, item.FoodItem.Price).FinalPrice;
                    if (item.Price != price)
                    {
                        item.Price = price;
                        hasPriceChanged = true;
                    }
                }

                if (item.ComboID != null && item.Combo != null)
                {
                    var price = _discountPriceService.GetComboDiscount(item.ComboID.Value, item.Combo.Price).FinalPrice;
                    if (item.Price != price)
                    {
                        item.Price = price;
                        hasPriceChanged = true;
                    }
                }
            }

            if (hasPriceChanged)
            {
                _cartRepo.Save();
            }

            return cart;
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
            var foodItem = _cartRepo.GetFoodItem(addToCartRequest.FoodItemId);
            if (foodItem == null)
            {
                throw new KeyNotFoundException("Food item not found");
            }

            if (addToCartRequest.Quantity <= 0)
            {
                throw new ArgumentException("quantity must > 0");
            }

            var discountedPrice = _discountPriceService.GetFoodDiscount(foodItem.FoodItemId, foodItem.Price).FinalPrice;
            var cartItem = userCart.CartItems.FirstOrDefault(x => x.FoodItemID == addToCartRequest.FoodItemId);
            if (cartItem != null)
            {
                cartItem.Quantity += addToCartRequest.Quantity;
                cartItem.Price = discountedPrice;
            }
            else
            {
                userCart.CartItems.Add(new CartItem
                {
                    FoodItemID = addToCartRequest.FoodItemId,
                    Quantity = addToCartRequest.Quantity,
                    Price = discountedPrice,
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

            var discountedPrice = _discountPriceService.GetComboDiscount(combo.ComboId, combo.Price).FinalPrice;
            var cartItem = userCart.CartItems.FirstOrDefault(x => x.ComboID == addComboToCartRequest.ComboId);
            if (cartItem != null)
            {
                cartItem.Quantity += addComboToCartRequest.Quantity;
                cartItem.Price = discountedPrice;
            }
            else
            {
                userCart.CartItems.Add(new CartItem
                {
                    ComboID = addComboToCartRequest.ComboId,
                    Quantity = addComboToCartRequest.Quantity,
                    Price = discountedPrice,
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
                cartItem.Price = _discountPriceService.GetFoodDiscount(cartItem.FoodItemID.Value, cartItem.FoodItem.Price).FinalPrice;
            }

            if (cartItem.ComboID != null)
            {
                cartItem.Price = _discountPriceService.GetComboDiscount(cartItem.ComboID.Value, cartItem.Combo.Price).FinalPrice;
            }

            _cartRepo.Save();
        }
    }
}
