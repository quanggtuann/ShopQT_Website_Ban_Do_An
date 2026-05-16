using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Security.Claims;
using ShopView.Infrastructure;
using ShopView.ViewModels;

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

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
