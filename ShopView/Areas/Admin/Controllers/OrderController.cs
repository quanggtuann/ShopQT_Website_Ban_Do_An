using Microsoft.AspNetCore.Mvc;
using ShopView.Areas.Admin.ViewModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShopView.Areas.Admin.Controllers
{
    public class OrderController : AdminControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        public async Task<IActionResult> Index(
            [FromQuery] int? orderId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? status,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new AdminOrderFilterViewModel
                {
                    OrderId = orderId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Status = status,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    Page = page,
                    PageSize = pageSize
                };

                var response = await _httpClient.GetAsync($"api/admin/orders?{BuildQuery(filter)}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                    return View(new AdminOrderIndexViewModel { Filter = filter });
                }

                var result = await response.Content.ReadFromJsonAsync<PagedResponse<AdminOrderDto>>(_jsonOptions)
                    ?? new PagedResponse<AdminOrderDto>();

                return View(new AdminOrderIndexViewModel
                {
                    Filter = filter,
                    PagedResult = result
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new AdminOrderIndexViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/admin/orders/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                    return RedirectToAction(nameof(Index));
                }

                var order = await response.Content.ReadFromJsonAsync<AdminOrderDto>(_jsonOptions);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(new AdminOrderDetailViewModel
                {
                    Order = order
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NextStatus(int id, string? returnUrl = null)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/admin/orders/{id}/next-status", null);
                TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                    response.IsSuccessStatusCode
                        ? "Order status updated successfully"
                        : await ReadApiErrorAsync(response);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectBack(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string cancelReason, string? returnUrl = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cancelReason))
                {
                    TempData["ErrorMessage"] = "Cancellation reason is required";
                    return RedirectBack(returnUrl);
                }

                var response = await _httpClient.PatchAsync(
                    $"api/admin/orders/{id}/cancel",
                    JsonContent.Create(new { CancelReason = cancelReason }));

                TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                    response.IsSuccessStatusCode
                        ? "Order cancelled successfully"
                        : await ReadApiErrorAsync(response);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectBack(returnUrl);
        }

        private IActionResult RedirectBack(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        private static string BuildQuery(AdminOrderFilterViewModel filter)
        {
            var query = new List<string>
            {
                $"page={filter.Page}",
                $"pageSize={filter.PageSize}"
            };

            if (filter.OrderId.HasValue) query.Add($"orderId={filter.OrderId.Value}");
            if (filter.MinPrice.HasValue) query.Add($"minPrice={filter.MinPrice.Value}");
            if (filter.MaxPrice.HasValue) query.Add($"maxPrice={filter.MaxPrice.Value}");
            if (filter.FromDate.HasValue) query.Add($"fromDate={filter.FromDate.Value:yyyy-MM-dd}");
            if (filter.ToDate.HasValue) query.Add($"toDate={filter.ToDate.Value:yyyy-MM-dd}");
            if (!string.IsNullOrWhiteSpace(filter.Status)) query.Add($"status={Uri.EscapeDataString(filter.Status)}");
            if (!string.IsNullOrWhiteSpace(filter.SortBy)) query.Add($"sortBy={Uri.EscapeDataString(filter.SortBy)}");
            if (!string.IsNullOrWhiteSpace(filter.SortOrder)) query.Add($"sortOrder={Uri.EscapeDataString(filter.SortOrder)}");

            return string.Join("&", query);
        }

        private async Task<string> ReadApiErrorAsync(HttpResponseMessage response)
        {
            var raw = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return $"Request failed ({(int)response.StatusCode})";
            }

            try
            {
                using var document = JsonDocument.Parse(raw);
                if (document.RootElement.TryGetProperty("errorMessage", out var lowerMessage))
                {
                    return lowerMessage.GetString() ?? raw;
                }

                if (document.RootElement.TryGetProperty("ErrorMessage", out var upperMessage))
                {
                    return upperMessage.GetString() ?? raw;
                }

                if (document.RootElement.TryGetProperty("message", out var message))
                {
                    return message.GetString() ?? raw;
                }
            }
            catch
            {
                return raw;
            }

            return raw;
        }
    }
}
