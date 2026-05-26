using ShopView.ViewModels;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ShopView.Infrastructure
{
    public class CartCountProvider : ICartCountProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartCountProvider(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetCartItemCountAsync(CancellationToken cancellationToken = default)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return 0;
            }

            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "customer", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ShopAPI");
                var response = await httpClient.GetAsync("api/customer/cart", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return 0;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return 0;
                }

                if (response.Content?.Headers?.ContentLength == 0)
                {
                    return 0;
                }

                var cart = await response.Content.ReadFromJsonAsync<CartViewModel>(
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    },
                    cancellationToken);

                if (cart?.CartItems == null || !cart.CartItems.Any())
                {
                    return 0;
                }

                return cart.CartItems.Sum(item => item.Quantity);
            }
            catch
            {
                return 0;
            }
        }
    }
}
