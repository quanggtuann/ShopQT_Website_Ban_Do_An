using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopView.ViewModels;
using System.Text.Json.Serialization;

namespace ShopView.Controllers
{
    public class ComboController : Controller
    {
        private readonly HttpClient _httpClient;

        public ComboController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ComboFilterViewModels filter)
        {
            try
            {
                var query = new List<string>
                {
                    $"page={filter.page}",
                    $"pageSize={filter.pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                    query.Add($"KeyWord={Uri.EscapeDataString(filter.Keyword)}");
                if (filter.PriceFrom.HasValue)
                    query.Add($"FromPrice={filter.PriceFrom.Value}");
                if (filter.PriceTo.HasValue)
                    query.Add($"ToPrice={filter.PriceTo.Value}");
                if (!string.IsNullOrWhiteSpace(filter.SortBy))
                    query.Add($"ShortBy={Uri.EscapeDataString(filter.SortBy)}");
                if (!string.IsNullOrWhiteSpace(filter.SortOrder))
                    query.Add($"ShortOrder={Uri.EscapeDataString(filter.SortOrder)}");

                var response = await _httpClient.GetAsync($"api/customer/combos?{string.Join("&", query)}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Cannot load combo menu. ({(int)response.StatusCode}) {errorBody}";
                    return View(new ComboViewModels { Filter = filter });
                }

                var result = await response.Content.ReadFromJsonAsync<PagedResult<ComboDto>>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                }) ?? new PagedResult<ComboDto>();

                result.Data ??= new List<ComboDto>();

                var vm = new ComboViewModels
                {
                    Filter = filter,
                    PagedResult = result,
                    ImageBaseUrl = "https://localhost:7130/"
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new ComboViewModels { Filter = filter });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/customer/combos/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                var item = await response.Content.ReadFromJsonAsync<ComboDto>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                });

                if (item == null || !item.IsVaiLabel)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ImageBaseUrl = "https://localhost:7130/";
                return View("Detail", item);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
