using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopDAL.Models;
using ShopDAL.Models.Dto;
using System.Net.Http.Json;
namespace ShopView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FoodController : Controller
    {
        private readonly HttpClient _httpClient;
        public FoodController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "admin";
        }
        private async Task<List<SelectListItem>> GetActiveCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/categoryes?activeOnly=true");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
                    return categories?
                        .Where(c => c.IsActive)
                        .Select(c => new SelectListItem
                        {
                            Value = c.CategoryId.ToString(),
                            Text = c.Name
                        })
                        .ToList() ?? new List<SelectListItem>();
                }
            }
            catch { }
            return new List<SelectListItem>();
        }
        // GET: /Admin/Food/Index

        public async Task<IActionResult> Index(
             [FromQuery] string? keyword,
             [FromQuery] int? categoryId,
             [FromQuery] decimal? priceFrom,
             [FromQuery] decimal? priceTo,
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
                if (!string.IsNullOrEmpty(keyword)) query.Add($"keyword={Uri.EscapeDataString(keyword)}");
                if (categoryId.HasValue) query.Add($"categoryId={categoryId}");
                if (priceFrom.HasValue) query.Add($"priceFrom={priceFrom}");
                if (priceTo.HasValue) query.Add($"priceTo={priceTo}");
                if (!string.IsNullOrEmpty(sortBy)) query.Add($"sortBy={sortBy}");
                if (!string.IsNullOrEmpty(sortOrder)) query.Add($"sortOrder={sortOrder}");
                var response = await _httpClient.GetAsync($"api/foods?{string.Join("&", query)}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to load food items";
                    return View(new FoodIndexViewModel());
                }
                var result = await response.Content.ReadFromJsonAsync<FoodListResponse>();
                var categories = await GetActiveCategoriesAsync();
                var viewModel = new FoodIndexViewModel
                {
                    Filter = new FoodItemFilterViewModel
                    {
                        Keyword = keyword,
                        categoryID = categoryId,
                        PriceFrom = priceFrom,
                        PriceTo = priceTo,
                        SortBy = sortBy,
                        SortOrder = sortOrder,
                        page = page,
                        pageSize = pageSize
                    },
                    FoodItems = result?.Data ?? new List<FoodItemDto>(),
                    Categories = categories,
                    TotalPage = result?.TotalPages ?? 1,
                    CurrentPage = result?.CurrentPage ?? 1,
                    ImageBaseUrl = "https://localhost:7130/"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new FoodIndexViewModel());
            }
        }
        // GET: /Admin/Food/Create
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {
                ViewBag.Categories = await GetActiveCategoriesAsync();
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        // POST: /Admin/Food/Create
        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection form, IFormFile imageFile)
        {
            try
            {
                ViewBag.Categories = await GetActiveCategoriesAsync();
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(form["Name"]!), "Name");
                content.Add(new StringContent(form["Description"]!), "Description");
                content.Add(new StringContent(form["Price"]!), "Price");
                content.Add(new StringContent(form["CategoryId"]!), "CategoryId");
                content.Add(new StringContent(form["IsAvailable"].ToString() == "true" ? "true" : "false"), "IsAvailable");
                if (imageFile != null && imageFile.Length > 0)
                {
                    var streamContent = new StreamContent(imageFile.OpenReadStream());
                    content.Add(streamContent, "imageFile", imageFile.FileName);
                }
                var response = await _httpClient.PostAsync("api/foods", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Food added successfully";
                    return RedirectToAction("Index");
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to create food: {errorContent}";
                ViewBag.Categories = await GetActiveCategoriesAsync();
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }
        // GET: /Admin/Food/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {
                var response = await _httpClient.GetAsync($"api/foods/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewData["ErrorMessage"] = "Food not found";
                    return View("Error", "Shared");
                }
                var food = await response.Content.ReadFromJsonAsync<ShopDAL.Models.FoodItem>();
                ViewBag.Categories = await GetActiveCategoriesAsync();
                ViewBag.ImageBaseUrl = "https://localhost:7130/";
                return View(food);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error", "Shared");
            }
        }
        // POST: /Admin/Food/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, IFormCollection form, IFormFile formFile)
        {
            try
            {
                ViewBag.Categories = await GetActiveCategoriesAsync();
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(form["Name"]!), "Name");
                content.Add(new StringContent(form["Description"]!), "Description");
                content.Add(new StringContent(form["Price"]!), "Price");
                content.Add(new StringContent(form["CategoryId"]!), "CategoryId");
                content.Add(new StringContent(form["IsAvailable"].ToString() == "true" ? "true" : "false"), "IsAvailable");
                if (formFile != null && formFile.Length > 0)
                {
                    var streamContent = new StreamContent(formFile.OpenReadStream());
                    content.Add(streamContent, "formFile", formFile.FileName);
                }
                var response = await _httpClient.PutAsync($"api/foods/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Food updated successfully";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        // POST: /Admin/Food/Deactive/5
        [HttpPost]
        public async Task<IActionResult> Deactive(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {
                var response = await _httpClient.PatchAsync($"api/foods/{id}/deactivate", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Food deactivated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to deactivate food";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        // POST: /Admin/Food/Active/5
        [HttpPost]
        public async Task<IActionResult> Active(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {
                var response = await _httpClient.PatchAsync($"api/foods/{id}/activate", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Food activated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to activate food";
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

