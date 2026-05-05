using Microsoft.AspNetCore.Mvc;
using ShopView.Models;
using System.Net.Http.Json;

namespace ShopView.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
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

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var response = await _httpClient.PostAsJsonAsync("api/customer/account/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    
                    // Store in session
                    HttpContext.Session.SetInt32("UserID", result.userId);
                    HttpContext.Session.SetString("Username", result.username);
                    HttpContext.Session.SetString("UserRole", result.role);

                    if (result.role == "admin")
                        return RedirectToAction("Index", "Account", new { area = "Admin" });
                    else
                        return RedirectToAction("Index", "Home");
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

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateofBirth { get; set; }
    }

    public class LoginResponse
    {
        public int userId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string role { get; set; }
    }
}

