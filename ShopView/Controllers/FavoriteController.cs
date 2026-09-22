using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ShopView.Controllers
{
    [Authorize(Roles = "customer")]
    public class FavoriteController : Controller
    {
        private readonly HttpClient _httpClient;

        public FavoriteController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/customer/favorites");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return await ExpireSessionAsync(Url.Action("Index", "Favorite"));
                }

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Cannot load favorites. ({(int)response.StatusCode})";
                    ViewBag.ImageBaseUrl = "https://localhost:7130/";
                    return View(new List<FavoriteDto>());
                }

                var favorites = await response.Content.ReadFromJsonAsync<List<FavoriteDto>>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                }) ?? new List<FavoriteDto>();

                ViewBag.ImageBaseUrl = "https://localhost:7130/";
                return View(favorites);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.ImageBaseUrl = "https://localhost:7130/";
                return View(new List<FavoriteDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ToggleFood(int id, string? returnUrl = null)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/customer/favorites/food/{id}/toggle", null);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return await ExpireSessionAsync(returnUrl ?? Url.Action("Index", "Food"));
                }

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Update favorite failed. ({(int)response.StatusCode})";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToLocal(returnUrl) ?? RedirectToAction("Index", "Favorite");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFood(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/customer/favorites/food/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return await ExpireSessionAsync(Url.Action("Index", "Favorite"));
                }

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Remove favorite failed. ({(int)response.StatusCode})";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private IActionResult? RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return null;
        }

        private async Task<IActionResult> ExpireSessionAsync(string? returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["ErrorMessage"] = "Your session has expired. Please login again.";
            return RedirectToAction("Login", "Account", new { returnUrl });
        }
    }
}
