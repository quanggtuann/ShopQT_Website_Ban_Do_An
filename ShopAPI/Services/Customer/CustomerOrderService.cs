using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;
using ShopDAL.Repository.IRepository;

namespace ShopAPI.Services.Customer
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly ICartRepo _cartRepo;
        private readonly IAddressRepo _addressRepo;
        private readonly ICustomerDiscountPriceService _discountPriceService;

        public CustomerOrderService(
            IOrderRepo orderRepo,
            ICartRepo cartRepo,
            IAddressRepo addressRepo,
            ICustomerDiscountPriceService discountPriceService)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _addressRepo = addressRepo;
            _discountPriceService = discountPriceService;
        }

        public CreateOrderResponse CreateOrderFromCart(int userId, CreateOrderRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var cart = _cartRepo.GetByUserID(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Your cart is empty.");
            }

            var address = _addressRepo.GetById(request.AddressId);
            if (address == null || address.UserId != userId)
            {
                throw new InvalidOperationException("Delivery address is invalid.");
            }

            var paymentMethod = ParsePaymentMethod(request.PaymentMethod);
            var order = new Order
            {
                UserId = userId,
                AddressId = address.AddressId,
                OrderTime = DateTime.Now,
                Status = OrderStatus.Pending,
                PaymentMethod = paymentMethod,
                IsPaid = false,
                Note = request.Note,
                ReceiverName = address.ReceiverName,
                ReceiverPhone = address.PhoneNumber,
                ShippingAddress = BuildShippingAddress(address)
            };

            foreach (var cartItem in cart.CartItems)
            {
                var quantity = cartItem.Quantity;
                if (quantity <= 0)
                {
                    continue;
                }

                var price = GetServerPrice(cartItem);
                order.OrderDetail.Add(new OrderDetail
                {
                    FoodItemID = cartItem.FoodItemID,
                    ComboID = cartItem.ComboID,
                    Quantity = quantity,
                    Price = price
                });
            }

            if (!order.OrderDetail.Any())
            {
                throw new InvalidOperationException("Your cart has no valid items.");
            }

            order.TotalAmount = order.OrderDetail.Sum(item => item.Price * item.Quantity);

            _orderRepo.Add(order);
            _cartRepo.ClearCartItems(cart.CartID);
            _orderRepo.Save();

            return new CreateOrderResponse
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                Message = "Order created successfully."
            };
        }

        public PagedResult<CustomerOrderDto> GetOrders(int userId, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 6 : pageSize;

            var query = _orderRepo.GetByUserId(userId);
            var totalItems = query.Count();
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
            var currentPage = totalPages > 0 ? Math.Min(page, totalPages) : 1;

            var data = query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return new PagedResult<CustomerOrderDto>
            {
                TotalItems = totalItems,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                Data = data
            };
        }

        public CustomerOrderDto? GetOrderDetail(int userId, int orderId)
        {
            var order = _orderRepo.GetByIdForUser(orderId, userId);
            return order == null ? null : ToDto(order);
        }

        public void CancelOrder(int userId, int orderId, CancelOrderRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CancelReason))
            {
                throw new InvalidOperationException("Please choose a cancellation reason.");
            }

            var cancelReason = request.CancelReason.Trim();
            var cancelReasonDetail = string.IsNullOrWhiteSpace(request.CancelReasonDetail)
                ? null
                : request.CancelReasonDetail.Trim();

            if (cancelReason.Equals("Other", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(cancelReasonDetail))
            {
                throw new InvalidOperationException("Please enter your cancellation reason.");
            }

            var order = _orderRepo.GetTrackedByIdForUser(orderId, userId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be cancelled.");
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelReason = cancelReason;
            order.CancelReasonDetail = cancelReasonDetail;
            order.CancelledBy = "Customer";
            order.CancelledAt = DateTime.Now;
            _orderRepo.Save();
        }

        private decimal GetServerPrice(CartItem cartItem)
        {
            if (cartItem.FoodItemID.HasValue && cartItem.FoodItem != null)
            {
                return _discountPriceService
                    .GetFoodDiscount(cartItem.FoodItemID.Value, cartItem.FoodItem.Price)
                    .FinalPrice;
            }

            if (cartItem.ComboID.HasValue && cartItem.Combo != null)
            {
                return _discountPriceService
                    .GetComboDiscount(cartItem.ComboID.Value, cartItem.Combo.Price)
                    .FinalPrice;
            }

            return cartItem.Price ?? 0;
        }

        private static PaymentMethod ParsePaymentMethod(string? paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                return PaymentMethod.Cash;
            }

            if (paymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase)
                || paymentMethod.Equals("Cash", StringComparison.OrdinalIgnoreCase))
            {
                return PaymentMethod.Cash;
            }

            if (Enum.TryParse<PaymentMethod>(paymentMethod, true, out var parsed))
            {
                return parsed;
            }

            throw new InvalidOperationException("Payment method is invalid.");
        }

        private static string BuildShippingAddress(Address address)
        {
            var parts = new[]
            {
                address.DetailAddress,
                address.Ward,
                address.District,
                address.Province
            };

            return string.Join(", ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
        }

        private static CustomerOrderDto ToDto(Order order)
        {
            return new CustomerOrderDto
            {
                OrderId = order.OrderId,
                OrderTime = order.OrderTime,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                AddressId = order.AddressId,
                PaymentMethod = order.PaymentMethod.ToString(),
                IsPaid = order.IsPaid,
                Note = order.Note,
                CancelReason = order.CancelReason,
                CancelReasonDetail = order.CancelReasonDetail,
                CancelledBy = order.CancelledBy,
                CancelledAt = order.CancelledAt,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderDetail.Select(ToDetailDto).ToList()
            };
        }

        private static CustomerOrderDetailDto ToDetailDto(OrderDetail detail)
        {
            var isFood = detail.FoodItemID.HasValue;
            return new CustomerOrderDetailDto
            {
                OrderDetailId = detail.OrderDetailID,
                Quantity = detail.Quantity,
                Price = detail.Price,
                Subtotal = detail.Price * detail.Quantity,
                FoodItemId = detail.FoodItemID,
                ComboId = detail.ComboID,
                ProductName = isFood
                    ? detail.FoodItem?.Name ?? "Food item"
                    : detail.Combo?.Name ?? "Combo",
                ImagePath = isFood
                    ? detail.FoodItem?.ImagePath
                    : detail.Combo?.ImagePath,
                ItemType = isFood ? "Food" : "Combo"
            };
        }
    }
}
