using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopView.ViewModels;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ShopView.Controllers
{
    [Authorize(Roles = "customer")]
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/customer/cart");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Cannot load cart. ({(int)response.StatusCode}) {body}";
                    return View(CreateEmptyCartModel());
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return View(CreateEmptyCartModel());
                }

                if (response.Content?.Headers?.ContentLength == 0)
                {
                    return View(CreateEmptyCartModel());
                }

                var cart = await response.Content.ReadFromJsonAsync<CartViewModel>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                }) ?? CreateEmptyCartModel();

                cart.CartItems ??= new List<CartItemViewModel>();
                return View(cart);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(CreateEmptyCartModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddFood(int id, int quantity = 1, string? returnUrl = null)
        {
            try
            {
                if (quantity <= 0) quantity = 1;

                var response = await _httpClient.PostAsJsonAsync("api/customer/cart/add-food", new AddFoodToCartApiRequest
                {
                    FoodItemId = id,
                    Quantity = quantity
                });
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? Url.Action("Index", "Cart") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Add to cart failed. ({(int)response.StatusCode}) {body}";
                    return RedirectToLocal(returnUrl) ?? RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Added to cart.";
                return RedirectToLocal(returnUrl) ?? RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToLocal(returnUrl) ?? RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddCombo(int id, int quantity = 1, string? returnUrl = null)
        {
            try
            {
                if (quantity <= 0) quantity = 1;

                var response = await _httpClient.PostAsJsonAsync("api/customer/cart/add-combo", new AddComboToCartApiRequest
                {
                    ComboId = id,
                    Quantity = quantity
                });
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? Url.Action("Index", "Cart") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Add to cart failed. ({(int)response.StatusCode}) {body}";
                    return RedirectToLocal(returnUrl) ?? RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Added to cart.";
                return RedirectToLocal(returnUrl) ?? RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToLocal(returnUrl) ?? RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int cartItemId, int quantity)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/customer/cart/update", new UpdateCartItemApiRequest
                {
                    CartItemId = cartItemId,
                    Quantity = quantity
                });
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Update failed. ({(int)response.StatusCode}) {body}";
                }
                else
                {
                    TempData["SuccessMessage"] = "Updated cart.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/customer/cart/{cartItemId}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Remove failed. ({(int)response.StatusCode}) {body}";
                }
                else
                {
                    TempData["SuccessMessage"] = "Removed item.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private IActionResult? RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return null;
        }

        private static CartViewModel CreateEmptyCartModel()
        {
            return new CartViewModel
            {
                CartItems = new List<CartItemViewModel>()
            };
        }
    }
}
