using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using System.Net.Http.Json;
using System.Security.Claims;
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
                if (filter.page <= 0) filter.page = 1;
                if (filter.pageSize <= 0) filter.pageSize = 12;

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
                var comboResponse = await _httpClient.GetAsync("api/customer/combos?page=1&pageSize=20");
                var combos = new List<ComboDto>();
                if (comboResponse.IsSuccessStatusCode)
                {
                    var comboResult = await comboResponse.Content.ReadFromJsonAsync<PagedResult<ComboDto>>(new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    });
                    combos = comboResult?.Data?.Where(x => x.IsVaiLabel && x.HasDiscount).ToList() ?? new List<ComboDto>();
                }

                ViewBag.FavoriteFoodIds = await GetFavoriteFoodIdsAsync();

                var vm = new FoodMenuViewModel
                {
                    Filter = filter,
                    PagedResult = result,
                    Categories = categories,
                    FeaturedCombos = combos,
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

        private async Task<HashSet<int>> GetFavoriteFoodIdsAsync()
        {
            if (User.Identity?.IsAuthenticated != true ||
                !string.Equals(User.FindFirst(ClaimTypes.Role)?.Value, "customer", StringComparison.OrdinalIgnoreCase))
            {
                return new HashSet<int>();
            }

            try
            {
                var response = await _httpClient.GetAsync("api/customer/favorites");
                if (!response.IsSuccessStatusCode)
                {
                    return new HashSet<int>();
                }

                var favorites = await response.Content.ReadFromJsonAsync<List<FavoriteDto>>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                }) ?? new List<FavoriteDto>();

                return favorites.Select(f => f.FoodId).ToHashSet();
            }
            catch
            {
                return new HashSet<int>();
            }
        }
    }

}
