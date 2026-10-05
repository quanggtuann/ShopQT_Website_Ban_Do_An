using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopView.Infrastructure;
using ShopView.ViewModels;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace ShopView.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly HttpClient _httpClient;

        public AddressController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var response = await _httpClient.GetAsync("api/customer/address");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account",
                        new { returnUrl = Url.Action(nameof(Index), "Address") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();

                    TempData["ErrorMessage"] =
                        $"Cannot load addresses. ({(int)response.StatusCode}) {body}";

                    return View(new AddressPageViewModel());
                }

                var addresses =
                    await response.Content.ReadFromJsonAsync<List<AddressViewModel>>
                    (
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    ) ?? new List<AddressViewModel>();

                addresses = addresses
                    .OrderByDescending(a => a.IsDefault)
                    .ThenByDescending(a => a.AddressId)
                    .ToList();

                const int pageSize = 6;

                var totalItems = addresses.Count;

                var pagedAddresses = addresses
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var model = new AddressPageViewModel
                {
                    Addresses = pagedAddresses,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(
                        totalItems / (double)pageSize)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return View(new AddressPageViewModel());
            }
        }

        public async Task<IActionResult> Create()
        {
            await LoadProvincesAsync();
            return View(new AddressFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadProvincesAsync();
                return View(model);
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/customer/address", new
                {
                    model.ReceiverName,
                    model.PhoneNumber,
                    model.Province,
                    District = string.Empty,
                    model.Ward,
                    model.DetailAddress
                });

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Create), "Address") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, body);
                    await LoadProvincesAsync();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Address created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadProvincesAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/customer/address/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Edit), "Address", new { id }) });
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Address not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Cannot load address. ({(int)response.StatusCode}) {body}";
                    return RedirectToAction(nameof(Index));
                }

                var address = await response.Content.ReadFromJsonAsync<AddressFormViewModel>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new AddressFormViewModel();

                await LoadProvincesAsync();
                return View(address);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddressFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadProvincesAsync();
                return View(model);
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/customer/address/{id}", new
                {
                    AddressId = model.AddressId,
                    model.ReceiverName,
                    model.PhoneNumber,
                    model.Province,
                    District = string.Empty,
                    model.Ward,
                    model.DetailAddress
                });

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Edit), "Address", new { id }) });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, body);
                    await LoadProvincesAsync();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Address updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadProvincesAsync();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/customer/address/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Address") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Delete failed. ({(int)response.StatusCode}) {body}";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Address deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/customer/address/{id}/default", null);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Address") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Update default failed. ({(int)response.StatusCode}) {body}";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Default address updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            return userId;
        }
        private async Task LoadProvincesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/location/provinces");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Provinces = new List<ProvinceDto>();
                    return;
                }

                var provinces = await response.Content.ReadFromJsonAsync<List<ProvinceDto>>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                ViewBag.Provinces = provinces ?? new List<ProvinceDto>();
            }
            catch
            {
                ViewBag.Provinces = new List<ProvinceDto>();
            }
        }
    }
}
