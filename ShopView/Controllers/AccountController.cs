using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Security.Claims;
using ShopView.Infrastructure;
using ShopView.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShopView.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userData = new
                {
                    Username = model.Username,
                    Password = model.Password,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DateorBirth = model.DateofBirth
                };

                var response = await _httpClient.PostAsJsonAsync("api/customer/account/register", userData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please login.";
                    return RedirectToAction("Login");
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Registration failed: {error}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("admin"))
                    return Redirect("/Admin/Home/Index");
                return RedirectToAction("Index", "Food");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var response = await _httpClient.PostAsJsonAsync("api/customer/account/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result == null || string.IsNullOrEmpty(result.token))
                    {
                        ViewBag.Error = "Invalid login response from server";
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, result.userId.ToString()),
                        new(ClaimTypes.Name, result.username),
                        new(ClaimTypes.Role, result.role),
                        new(AuthClaimTypes.AccessToken, result.token)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = false,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                        });

                    if (result.role == "admin")
                        return Redirect("/Admin/Home/Index");

                    return RedirectToAction("Index", "Food");
                }

                ViewBag.Error = "Invalid username or password";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return RedirectToAction("Login");
                }

                var response = await _httpClient.GetAsync($"api/customer/account/{userId}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", new { returnUrl = Url.Action("Profile") });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Cannot load profile. ({(int)response.StatusCode}) {body}";
                    return View(new CustomerProfileViewModel { Username = User.Identity?.Name ?? string.Empty });
                }

                var profile = await response.Content.ReadFromJsonAsync<CustomerProfileViewModel>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new CustomerProfileViewModel();

                return View(profile);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new CustomerProfileViewModel { Username = User.Identity?.Name ?? string.Empty });
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return RedirectToAction("Login");
                }
                var response = await _httpClient.GetAsync($"api/customer/account/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                TempData["ErrorMessage"] = "Cannot load profile";
                return RedirectToAction("Profile");
            }
                var profile= await response.Content.ReadFromJsonAsync<UpdateProfile>();
            return View(profile);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfile model)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out var userId))
                    return RedirectToAction("Login");

                var response = await _httpClient.PutAsJsonAsync(
                    $"api/customer/account/{userId}",
                    model);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();

                    TempData["ErrorMessage"] = body;

                    return View(model);
                }

                TempData["SuccessMessage"] = "Profile updated successfully";

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return View(model);
            }
        }
        [Authorize]
        public IActionResult MyAddresses()
        {
            return RedirectToAction("Index", "Address");
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!string.Equals(model.NewPassword, model.ConfirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, "New password and confirm password do not match.");
                return View(model);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"api/customer/account/{userId}/change-password",
                    new
                    {
                        CurrentPassword = model.CurrentPassword,
                        NewPassword = model.NewPassword
                    });

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", new { returnUrl = Url.Action("ChangePassword") });
                }

                if (response.IsSuccessStatusCode)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["SuccessMessage"] = "Password changed successfully. Please login again.";
                    return RedirectToAction("Login");
                }

                var body = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Change password failed. ({(int)response.StatusCode}) {body}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory(int page = 1, int pageSize = 6)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/customer/orders?page={page}&pageSize={pageSize}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                    return RedirectToAction("Login", new { returnUrl = Url.Action(nameof(OrderHistory), "Account", new { page, pageSize }) });
                }

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Cannot load orders. ({(int)response.StatusCode}) {body}";
                    return View(new PagedResponse<CustomerOrderViewModel>());
                }

                var orders = await response.Content.ReadFromJsonAsync<PagedResponse<CustomerOrderViewModel>>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    }) ?? new PagedResponse<CustomerOrderViewModel>();

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new PagedResponse<CustomerOrderViewModel>());
            }
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
