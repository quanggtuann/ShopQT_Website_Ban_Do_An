using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopDAL.Models;
using ShopDAL.Models.Dto;
using ShopView.Models;
using System.Net.Http.Json;

namespace ShopView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ComboController : Controller
    {
        private readonly HttpClient _httpClient;
        public ComboController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }
        private bool IsAdmin()
        {
            var UserRole = HttpContext.Session.GetString("UserRole");
            return UserRole == "admin";
        }
        public async Task<IActionResult> Index(
            [FromQuery] string? keyWord,
            [FromQuery] decimal? fromPrice,
            [FromQuery] decimal? toPrice,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try 
            {
                var query = new List<string> { $"page={page}", $"pageSize={pageSize}" };
                if (!string.IsNullOrEmpty(keyWord)) query.Add($"keyWord={Uri.EscapeDataString(keyWord)}");
                if (fromPrice.HasValue) query.Add($"fromPrice={fromPrice}");
                if (toPrice.HasValue) query.Add($"toPrice={toPrice}");
                if (!string.IsNullOrEmpty(sortBy)) query.Add($"sortBy={sortBy}");
                if (!string.IsNullOrEmpty(sortOrder)) query.Add($"sortOrder={sortOrder}");
                var response = await _httpClient.GetAsync($"api/Combos?{string.Join("&", query)}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to load combo";
                    return View(new ComboIndexViewModel());
                }
                var result = await response.Content.ReadFromJsonAsync<ComboListResponse>();
                var viewModel = new ComboIndexViewModel
                {
                    Filter = new ShopDAL.Models.ComboFilterViewmodel
                    {
                        KeyWord = keyWord,
                        FromPrice = fromPrice,
                        ToPrice = toPrice,
                        ShortBy = sortBy,
                        ShortOrder = sortOrder,
                        page = page,
                        pageSize = pageSize
                    },
                    Combos = result?.Data ?? new List<ComboDto>(),
                    TotalPage = result?.ToTalPage ?? 1,
                    CurrentPage = result?.CurrentPage ?? 1,
                    ImageBaseUrl = "https://localhost:7130/"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new ComboIndexViewModel());
            }
        }
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {
                var response = await _httpClient.GetAsync("api/foods/all");
                List<FoodItemDto> foods = new();
                if (response.IsSuccessStatusCode)
                {
                    foods = await response.Content.ReadFromJsonAsync<List<FoodItemDto>>() ?? new();
                }
                var viewModel = new ComboCreateViewModel
                {
                    Combo = new CreateComboRequest(),
                    AvailableFoods = foods,
                    ImageBaseUrl = "https://localhost:7130/"
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new ComboCreateViewModel());
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection form, IFormFile? imageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(form["Name"]!), "Name");
                content.Add(new StringContent(form["Description"]!), "Description");
                content.Add(new StringContent(form["Price"]!), "Price");
                content.Add(new StringContent(form["IsAvailable"].ToString() == "true" ? "true" : "false"), "IsAvailable");
                int formIndex = 0;
                int sendIndex = 0;
                while (form.ContainsKey($"FoodItems[{formIndex}].Quantity"))
                {
                    var foodId = form[$"FoodItems[{formIndex}].FoodItemId"];
                    var quantity = form[$"FoodItems[{formIndex}].Quantity"];
                    if (!string.IsNullOrEmpty(foodId) && int.TryParse(quantity, out var qty) && qty > 0)
                    {
                        content.Add(new StringContent(foodId!), $"FoodItems[{sendIndex}].FoodItemId");
                        content.Add(new StringContent(qty.ToString()), $"FoodItems[{sendIndex}].Quantity");
                        sendIndex++;
                    }
                    formIndex++;
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    var streamContent = new StreamContent(imageFile.OpenReadStream());
                    content.Add(streamContent, "ImageFile", imageFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/Combos", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Combo created successfully";
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"{error}";

                var foodsResponse = await _httpClient.GetAsync("api/foods/all");
                var foods = await foodsResponse.Content.ReadFromJsonAsync<List<FoodItemDto>>() ?? new();

                var viewModel = new ComboCreateViewModel
                {
                    Combo = new CreateComboRequest
                    {
                        Name = form["Name"],
                        Description = form["Description"]
                    },
                    AvailableFoods = foods,
                    ImageBaseUrl = "https://localhost:7130/"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        // GET: /Admin/Combo/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var comboResponse = await _httpClient.GetAsync($"api/Combos/{id}");
                if (!comboResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Combo not found";
                    return RedirectToAction(nameof(Index));
                }
                var combo = await comboResponse.Content.ReadFromJsonAsync<ComboDto>();
                var foodsResponse = await _httpClient.GetAsync("api/foods/all");
                List<FoodItemDto> foods = new();
                if (foodsResponse.IsSuccessStatusCode)
                {
                    foods = await foodsResponse.Content.ReadFromJsonAsync<List<FoodItemDto>>() ?? new();
                }
                var viewModel = new ComboCreateViewModel
                {
                    Combo = new CreateComboRequest
                    {
                        Name = combo?.Name ?? "",
                        Description = combo?.Description ?? "",
                        Price = combo?.Price ?? 0,
                    },
                    AvailableFoods = foods,
                    ImageBaseUrl = "https://localhost:7130/"
                };
                ViewBag.IsEdit = true;
                ViewBag.ComboId = id;
                ViewBag.CurrentImage = combo?.ImagePath;

                ViewBag.SelectedFoods = combo?.FoodItems ?? new List<ShopDAL.Models.Dto.ComboFoodItemDto>();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Admin/Combo/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, IFormCollection form, IFormFile? imageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(form["Name"]!), "Name");
                content.Add(new StringContent(form["Description"]!), "Description");
                content.Add(new StringContent(form["Price"]!), "Price");
                content.Add(new StringContent(form["IsAvailable"].ToString() == "true" ? "true" : "false"), "IsAvailable");

                int formIndex = 0;
                int sendIndex = 0;
                while (form.ContainsKey($"FoodItems[{formIndex}].Quantity"))
                {
                    var foodId = form[$"FoodItems[{formIndex}].FoodItemId"];
                    var quantity = form[$"FoodItems[{formIndex}].Quantity"];

                    if (!string.IsNullOrEmpty(foodId) && int.TryParse(quantity, out var qty) && qty > 0)
                    {
                        content.Add(new StringContent(foodId!), $"FoodItems[{sendIndex}].FoodItemId");
                        content.Add(new StringContent(qty.ToString()), $"FoodItems[{sendIndex}].Quantity");
                        sendIndex++;
                    }
                    formIndex++;
                }
                if (form.ContainsKey("RemoveImage") && form["RemoveImage"] == "true")
                {
                    content.Add(new StringContent("true"), "RemoveImage");
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    var streamContent = new StreamContent(imageFile.OpenReadStream());
                    content.Add(streamContent, "ImageFile", imageFile.FileName);
                }

                var response = await _httpClient.PutAsync($"api/Combos/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Combo updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"{error}";
                return RedirectToAction(nameof(Edit), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Edit), new { id });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { Areas = "" });
            try
            {
                var response = await _httpClient.PatchAsync($"api/combos/{id}/deactivate", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Combo Deactivate successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to deactivate combo";
                }
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { Areas = "" });
            try
            {
                var response = await _httpClient.PatchAsync($"api/combos/{id}/activate", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Combo activate successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to activate combo";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");

        }
    }
}
