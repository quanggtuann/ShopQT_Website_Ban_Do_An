using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ShopView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryeController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "admin";
        }

        // GET: /Admin/Categorye/Index
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var response = await _httpClient.GetAsync("api/categoryes");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<ShopDAL.Models.Category>>();
                    return View(categories);
                }

                ViewData["ErrorMessage"] = "Failed to load categories";
                return View("Error", "Shared");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error", "Shared");
            }
        }
        // GET: /Admin/Categorye/Create
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            return View();
        }
        // POST: /Admin/Categorye/Create
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    ModelState.AddModelError("", "Category name is required");
                    return View();
                }

                var category = new { Name = name, Description = "" };
                var response = await _httpClient.PostAsJsonAsync("api/categoryes", category);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category added successfully";
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }
        // GET: /Admin/Categorye/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {
                var response = await _httpClient.GetAsync($"api/categoryes/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API Response: {jsonString}");
                
                using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
                var root = doc.RootElement;
                
                var category = new ShopDAL.Models.Category
                {
                    CategoryId = root.GetProperty("categoryId").GetInt32(),
                    Name = root.GetProperty("name").GetString() ?? "",
                    Description = root.TryGetProperty("description", out var desc) ? desc.GetString() : null,
                    IsActive = root.TryGetProperty("isActive", out var active) && active.GetBoolean()
                };
                
                return View(category);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error", "Shared");
            }
        }
        // POST: /Admin/Categorye/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name)
        {
            try
            {
                var category = new { CategoryId = id, Name = name, Description = "" };
                var response = await _httpClient.PutAsJsonAsync($"api/categoryes/{id}", category);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category updated successfully";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Failed to update category";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }
        // POST: /Admin/Categorye/Deaction/5
        [HttpPost]
        public async Task<IActionResult> Deaction(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {
                var response = await _httpClient.PatchAsync($"api/categoryes/{id}/deactivate", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category deactivated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to deactivate category";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        // POST: /Admin/Categorye/Action/5
        [HttpPost]
        public async Task<IActionResult> Action(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {
                var response = await _httpClient.PatchAsync($"api/categoryes/{id}/activate", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category activated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to activate category";
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
