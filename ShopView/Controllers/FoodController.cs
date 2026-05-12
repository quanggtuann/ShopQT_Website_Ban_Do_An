using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ShopView.ViewModels;

namespace ShopView.Controllers
{
    public class FoodController : Controller
    {
        private readonly HttpClient _httpClient;

        public FoodController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] FoodFilterViewModel filter)
        {
            try
            {
                var query = new List<string>
                {
                    $"page={filter.page}",
                    $"pageSize={filter.pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(filter.Keyword)) query.Add($"keyword={Uri.EscapeDataString(filter.Keyword)}");
                if (filter.categoryID.HasValue) query.Add($"categoryID={filter.categoryID.Value}");
                if (filter.PriceFrom.HasValue) query.Add($"priceFrom={filter.PriceFrom.Value}");
                if (filter.PriceTo.HasValue) query.Add($"priceTo={filter.PriceTo.Value}");
                if (!string.IsNullOrWhiteSpace(filter.SortBy)) query.Add($"sortBy={Uri.EscapeDataString(filter.SortBy)}");
                if (!string.IsNullOrWhiteSpace(filter.SortOrder)) query.Add($"sortOrder={Uri.EscapeDataString(filter.SortOrder)}");

                var response = await _httpClient.GetAsync($"api/customer/foods?{string.Join("&", query)}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Cannot load food menu.";
                    return View("~/Views/Food/Index.cshtml", new FoodMenuViewModel { Filter = filter });
                }

                var result = await response.Content.ReadFromJsonAsync<PagedResult<FoodItemDto>>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                }) ?? new PagedResult<FoodItemDto>();

                var categoryResponse = await _httpClient.GetAsync("api/categoryes?activeOnly=true");
                var categories = new List<CategoryDto>();
                if (categoryResponse.IsSuccessStatusCode)
                {
                    categories = await categoryResponse.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
                }

                result.Data = result.Data.Where(x => x.IsAvailable).ToList();

                var vm = new FoodMenuViewModel
                {
                    Filter = filter,
                    PagedResult = result,
                    Categories = categories,
                    ImageBaseUrl = "https://localhost:7130/"
                };

                return View("~/Views/Food/Index.cshtml", vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("~/Views/Food/Index.cshtml", new FoodMenuViewModel { Filter = filter });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/customer/foods/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                var item = await response.Content.ReadFromJsonAsync<FoodItemDto>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                });

                if (item == null || !item.IsAvailable)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ImageBaseUrl = "https://localhost:7130/";
                return View(item);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }

}
