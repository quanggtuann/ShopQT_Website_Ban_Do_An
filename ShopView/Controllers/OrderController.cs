using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShopView.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShopView.Controllers
{
    [Authorize(Roles = "customer")]
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public OrderController(IHttpClientFactory httpClientFactory, IStringLocalizer<SharedResource> localizer)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int? addressId)
        {
            var model = await BuildCheckoutModelAsync(addressId > 0
                ? new CheckoutViewModel { AddressId = addressId.Value }
                : null);
            if (model == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            if (!model.Cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            if (!model.Addresses.Any())
            {
                return RedirectToAction("Create", "Address", new { returnToCheckout = true });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (model.AddressId <= 0)
            {
                ModelState.AddModelError(nameof(model.AddressId), "Please choose a delivery address.");
            }

            if (!ModelState.IsValid)
            {
                var reloadModel = await BuildCheckoutModelAsync(model);
                return View(reloadModel ?? model);
            }

            try
            {
                if (string.Equals(model.PaymentMethod, "VNPay", StringComparison.OrdinalIgnoreCase))
                {
                    var availability = await _httpClient.GetAsync("api/payments/vnpay/availability");
                    if (availability.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        return await ExpireSessionAsync(Url.Action(nameof(Checkout), "Order"));
                    if (!availability.IsSuccessStatusCode
                        || !(await availability.Content.ReadFromJsonAsync<VnpayAvailabilityResponse>(JsonOptions))?.Available == true)
                    {
                        ModelState.AddModelError(string.Empty, _localizer["VnpayNotConfigured"].Value);
                        var reloadModel = await BuildCheckoutModelAsync(model);
                        return View(reloadModel ?? model);
                    }
                }

                var response = await _httpClient.PostAsJsonAsync("api/customer/orders", new CreateOrderApiRequest
                {
                    AddressId = model.AddressId,
                    PaymentMethod = model.PaymentMethod,
                    Note = model.Note
                });

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return await ExpireSessionAsync(Url.Action(nameof(Checkout), "Order"));
                }

                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, $"Create order failed. ({(int)response.StatusCode}) {body}");
                    var reloadModel = await BuildCheckoutModelAsync(model);
                    return View(reloadModel ?? model);
                }

                var result = JsonSerializer.Deserialize<ApiResult<CreateOrderApiResponse>>(body, JsonOptions);
                if (result?.Data == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid order response from server.");
                    var reloadModel = await BuildCheckoutModelAsync(model);
                    return View(reloadModel ?? model);
                }

                if (string.Equals(model.PaymentMethod, "VNPay", StringComparison.OrdinalIgnoreCase))
                {
                    return await StartVnpayAsync(result.Data.OrderId);
                }

                TempData["SuccessMessage"] = result.Data.Message;
                return RedirectToAction(nameof(Details), new { id = result.Data.OrderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var reloadModel = await BuildCheckoutModelAsync(model);
                return View(reloadModel ?? model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayWithVnpay(int id) => await StartVnpayAsync(id);

        [HttpGet]
        public async Task<IActionResult> PaymentResult(int? orderId, string? result)
        {
            if (orderId is not > 0)
            {
                TempData["ErrorMessage"] = _localizer["VnpayInvalidResult"].Value;
                return RedirectToAction("OrderHistory", "Account");
            }

            var orderResponse = await _httpClient.GetAsync($"api/customer/orders/{orderId}");
            if (orderResponse.IsSuccessStatusCode)
            {
                var order = await orderResponse.Content.ReadFromJsonAsync<CustomerOrderViewModel>(JsonOptions);
                if (order?.IsPaid == true)
                {
                    TempData[order.Status == "Cancelled" ? "ErrorMessage" : "SuccessMessage"] =
                        order.Status == "Cancelled"
                            ? _localizer["VnpayCancelledPaid"].Value
                            : _localizer["VnpayConfirmed"].Value;
                    return RedirectToAction(nameof(Details), new { id = orderId });
                }
            }

            TempData[result == "pending" ? "InfoMessage" : "ErrorMessage"] = result switch
            {
                "failed" => _localizer["VnpayFailed"].Value,
                "pending" => _localizer["VnpayPending"].Value,
                _ => _localizer["VnpayInvalidResult"].Value
            };
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        private async Task<IActionResult> StartVnpayAsync(int orderId)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _httpClient.PostAsJsonAsync($"api/payments/vnpay/orders/{orderId}", new { ipAddress });
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return await ExpireSessionAsync(Url.Action(nameof(Details), "Order", new { id = orderId }));

                if (response.IsSuccessStatusCode)
                {
                    var payment = await response.Content.ReadFromJsonAsync<VnpayStartResponse>(JsonOptions);
                    if (payment != null && Uri.TryCreate(payment.PaymentUrl, UriKind.Absolute, out var uri)
                        && uri.Scheme == Uri.UriSchemeHttps
                        && uri.Host.Equals("sandbox.vnpayment.vn", StringComparison.OrdinalIgnoreCase))
                        return Redirect(uri.ToString());
                }
            }
            catch (Exception)
            {
                // The order already exists; let the customer retry payment from its details.
            }

            TempData["ErrorMessage"] = _localizer["VnpayStartFailed"].Value;
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/customer/orders/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return await ExpireSessionAsync(Url.Action(nameof(Details), "Order", new { id }));
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Order not found.";
                    return RedirectToAction("OrderHistory", "Account");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Cannot load order. ({(int)response.StatusCode}) {body}";
                    return RedirectToAction("OrderHistory", "Account");
                }

                var order = await response.Content.ReadFromJsonAsync<CustomerOrderViewModel>(JsonOptions);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Cannot load order detail.";
                    return RedirectToAction("OrderHistory", "Account");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("OrderHistory", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(
            int id,
            string cancelReason,
            string? cancelReasonDetail = null,
            string? returnUrl = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cancelReason))
                {
                    TempData["ErrorMessage"] = "Please choose a cancellation reason.";
                    return RedirectAfterCancel(id, returnUrl);
                }

                if (cancelReason.Equals("Other", StringComparison.OrdinalIgnoreCase)
                    && string.IsNullOrWhiteSpace(cancelReasonDetail))
                {
                    TempData["ErrorMessage"] = "Please enter your cancellation reason.";
                    return RedirectAfterCancel(id, returnUrl);
                }

                var response = await _httpClient.PatchAsync(
                    $"api/customer/orders/{id}/cancel",
                    JsonContent.Create(new CancelOrderApiRequest
                    {
                        CancelReason = cancelReason,
                        CancelReasonDetail = cancelReasonDetail
                    }));

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return await ExpireSessionAsync(returnUrl ?? Url.Action(nameof(Details), "Order", new { id }));
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    string? message = null;
                    try
                    {
                        message = JsonSerializer.Deserialize<ApiResult<object>>(body, JsonOptions)?.Message;
                    }
                    catch (JsonException)
                    {
                        // Show a concise error if the API did not return its usual JSON shape.
                    }
                    TempData["ErrorMessage"] = string.IsNullOrWhiteSpace(message)
                        ? $"Unable to cancel this order ({(int)response.StatusCode})."
                        : message;
                }
                else
                {
                    TempData["SuccessMessage"] = _localizer["OrderCancelled"].Value;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectAfterCancel(id, returnUrl);
        }

        private IActionResult RedirectAfterCancel(int id, string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<CheckoutViewModel?> BuildCheckoutModelAsync(CheckoutViewModel? current = null)
        {
            var cartResponse = await _httpClient.GetAsync("api/customer/cart");
            if (cartResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                return null;
            }

            var cart = new CartViewModel();
            if (cartResponse.IsSuccessStatusCode && cartResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                cart = await cartResponse.Content.ReadFromJsonAsync<CartViewModel>(JsonOptions) ?? new CartViewModel();
            }

            cart.CartItems ??= new List<CartItemViewModel>();

            var addressesResponse = await _httpClient.GetAsync("api/customer/address");
            if (addressesResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                return null;
            }

            var addresses = new List<AddressViewModel>();
            if (addressesResponse.IsSuccessStatusCode)
            {
                addresses = await addressesResponse.Content.ReadFromJsonAsync<List<AddressViewModel>>(JsonOptions)
                    ?? new List<AddressViewModel>();
            }

            addresses = addresses
                .OrderByDescending(address => address.IsDefault)
                .ThenByDescending(address => address.AddressId)
                .ToList();

            var vnpayAvailable = false;
            try
            {
                var availability = await _httpClient.GetAsync("api/payments/vnpay/availability");
                if (availability.IsSuccessStatusCode)
                    vnpayAvailable = (await availability.Content
                        .ReadFromJsonAsync<VnpayAvailabilityResponse>(JsonOptions))?.Available == true;
            }
            catch (HttpRequestException)
            {
                // Cash on delivery remains available when the payment service is unreachable.
            }

            return new CheckoutViewModel
            {
                Cart = cart,
                Addresses = addresses,
                VnpayAvailable = vnpayAvailable,
                AddressId = current?.AddressId > 0 && addresses.Any(address => address.AddressId == current.AddressId)
                    ? current.AddressId
                    : addresses.FirstOrDefault(address => address.IsDefault)?.AddressId
                        ?? addresses.FirstOrDefault()?.AddressId
                        ?? 0,
                PaymentMethod = string.IsNullOrWhiteSpace(current?.PaymentMethod) ? "Cash" : current.PaymentMethod,
                Note = current?.Note
            };
        }

        private async Task<IActionResult> ExpireSessionAsync(string? returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["ErrorMessage"] = "Your session has expired. Please login again.";
            return RedirectToAction("Login", "Account", new { returnUrl });
        }
    }
}
