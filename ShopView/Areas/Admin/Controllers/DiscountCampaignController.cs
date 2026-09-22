using Microsoft.AspNetCore.Mvc;
using ShopView.Areas.Admin.Models;
using ShopView.Areas.Admin.ViewModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShopView.Areas.Admin.Controllers
{
    public class DiscountCampaignController : AdminControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DiscountCampaignController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        public async Task<IActionResult> Index(
            [FromQuery] string? keyword,
            [FromQuery] bool? isActive,
            [FromQuery] string? discountType,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = new List<string> { $"page={page}", $"pageSize={pageSize}" };
                if (!string.IsNullOrWhiteSpace(keyword)) query.Add($"keyword={Uri.EscapeDataString(keyword)}");
                if (isActive.HasValue) query.Add($"isActive={isActive.Value}");
                if (!string.IsNullOrWhiteSpace(discountType)) query.Add($"discountType={Uri.EscapeDataString(discountType)}");
                if (!string.IsNullOrWhiteSpace(sortBy)) query.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
                if (!string.IsNullOrWhiteSpace(sortOrder)) query.Add($"sortOrder={Uri.EscapeDataString(sortOrder)}");

                var response = await _httpClient.GetAsync($"api/DiscountCampaigns?{string.Join("&", query)}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to load discount campaigns";
                    return View(new DiscountCampaignIndexViewModel());
                }

                var result = await response.Content.ReadFromJsonAsync<PagedResponse<DiscountCampaignDto>>(_jsonOptions);
                return View(new DiscountCampaignIndexViewModel
                {
                    Filter = new DiscountCampaignFilterViewModel
                    {
                        Keyword = keyword,
                        IsActive = isActive,
                        SortBy = sortBy,
                        SortOrder = sortOrder,
                        Page = page,
                        PageSize = pageSize
                    },
                    Campaigns = result?.Data ?? new(),
                    TotalPages = result?.TotalPages ?? 1,
                    CurrentPage = result?.CurrentPage ?? page
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new DiscountCampaignIndexViewModel());
            }
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new DiscountCampaignFormViewModel();
            await FillOptionsAsync(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiscountCampaignFormViewModel viewModel)
        {
            await FillOptionsAsync(viewModel);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/DiscountCampaigns", ToApiRequest(viewModel));
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Discount campaign created successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/DiscountCampaigns/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Discount campaign not found";
                    return RedirectToAction(nameof(Index));
                }

                var campaign = await response.Content.ReadFromJsonAsync<DiscountCampaignDto>(_jsonOptions);
                if (campaign == null)
                {
                    TempData["ErrorMessage"] = "Discount campaign not found";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new DiscountCampaignFormViewModel
                {
                    DiscountCampaignId = campaign.DiscountCampaignId,
                    CampaignName = campaign.CampaignName,
                    Description = campaign.Description,
                    DiscountType = campaign.DiscountType,
                    DiscountValue = campaign.DiscountValue,
                    StartDate = campaign.StartDate,
                    EndDate = campaign.EndDate,
                    IsActive = campaign.IsActive,
                    FoodItemIds = campaign.Products.Select(p => p.FoodItemId).ToList(),
                    ComboIds = campaign.Combos.Select(c => c.ComboId).ToList()
                };

                await FillOptionsAsync(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DiscountCampaignFormViewModel viewModel)
        {
            await FillOptionsAsync(viewModel);

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/DiscountCampaigns/{id}", ToApiRequest(viewModel));
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Discount campaign updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = await ReadApiErrorAsync(response);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            await ToggleStatusAsync(id, true);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)
        {
            await ToggleStatusAsync(id, false);
            return RedirectToAction(nameof(Index));
        }

        private async Task ToggleStatusAsync(int id, bool isActive)
        {
            try
            {
                var endpoint = isActive ? "activate" : "deactivate";
                var response = await _httpClient.PatchAsync($"api/DiscountCampaigns/{id}/{endpoint}", null);
                TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                    response.IsSuccessStatusCode
                        ? $"Discount campaign {(isActive ? "activated" : "deactivated")} successfully"
                        : await ReadApiErrorAsync(response);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }

        private async Task FillOptionsAsync(DiscountCampaignFormViewModel viewModel)
        {
            viewModel.AvailableFoods = await GetFoodsAsync();
            viewModel.AvailableCombos = await GetCombosAsync();
        }

        private async Task<List<DiscountFoodOptionDto>> GetFoodsAsync()
        {
            var response = await _httpClient.GetAsync("api/Foods/all");
            if (!response.IsSuccessStatusCode)
            {
                return new List<DiscountFoodOptionDto>();
            }

            var foods = await response.Content.ReadFromJsonAsync<List<DiscountFoodOptionDto>>(_jsonOptions);
            return foods ?? new List<DiscountFoodOptionDto>();
        }

        private async Task<List<DiscountComboOptionDto>> GetCombosAsync()
        {
            var response = await _httpClient.GetAsync("api/Combos?page=1&pageSize=1000");
            if (!response.IsSuccessStatusCode)
            {
                return new List<DiscountComboOptionDto>();
            }

            var result = await response.Content.ReadFromJsonAsync<PagedResponse<DiscountComboOptionDto>>(_jsonOptions);
            return result?.Data ?? new List<DiscountComboOptionDto>();
        }

        private static object ToApiRequest(DiscountCampaignFormViewModel viewModel)
        {
            return new
            {
                viewModel.CampaignName,
                viewModel.Description,
                viewModel.DiscountType,
                viewModel.DiscountValue,
                viewModel.StartDate,
                viewModel.EndDate,
                viewModel.IsActive,
                viewModel.FoodItemIds,
                viewModel.ComboIds
            };
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
            }
            catch
            {
                return raw;
            }

            return raw;
        }
    }
}
