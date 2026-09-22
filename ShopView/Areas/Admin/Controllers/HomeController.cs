using Microsoft.AspNetCore.Mvc;
using ShopView.Areas.Admin.ViewModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShopView.Areas.Admin.Controllers
{
    public class HomeController : AdminControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var model = new AdminDashboardViewModel
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            try
            {
                var query = new List<string>();
                if (fromDate.HasValue) query.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
                if (toDate.HasValue) query.Add($"toDate={toDate.Value:yyyy-MM-dd}");

                var endpoint = query.Any()
                    ? $"api/admin/statistics?{string.Join("&", query)}"
                    : "api/admin/statistics";

                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                    return View(model);
                }

                model.Statistics = await response.Content.ReadFromJsonAsync<AdminStatisticsViewModel>(_jsonOptions)
                    ?? new AdminStatisticsViewModel();
                model.FromDate = model.Statistics.FromDate;
                model.ToDate = model.Statistics.ToDate;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return View(model);
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
                if (document.RootElement.TryGetProperty("errorMessage", out var errorMessage))
                {
                    return errorMessage.GetString() ?? raw;
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
