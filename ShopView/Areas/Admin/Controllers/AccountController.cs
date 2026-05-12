using Microsoft.AspNetCore.Mvc;
using ShopView.Areas.Admin.Models;
using ShopView.Areas.Admin.ViewModel;

namespace ShopView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ShopAPI");
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "admin";
        }

        public async Task<IActionResult> Index(
            string? keyword,
            bool? isActive,
            string? role,
            string? sortBy,
            string? sortOrder,
            int page = 1,
            int pageSize = 10)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(keyword))
                    queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
                if (isActive.HasValue)
                    queryParams.Add($"isActive={isActive.Value}");
                if (!string.IsNullOrWhiteSpace(role))
                    queryParams.Add($"role={Uri.EscapeDataString(role)}");
                if (!string.IsNullOrWhiteSpace(sortBy))
                    queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
                if (!string.IsNullOrWhiteSpace(sortOrder))
                    queryParams.Add($"sortOrder={Uri.EscapeDataString(sortOrder)}");
                queryParams.Add($"page={page}");
                queryParams.Add($"pageSize={pageSize}");

                var url = "api/accounts?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PagedResponse<UserListItem>>();

                    var viewModel = new AccountIndexViewModel
                    {
                        Filter = new AccountFilterViewModel
                        {
                            Keyword = keyword,
                            IsActive = isActive,
                            Role = role,
                            SortBy = sortBy ?? "id",
                            SortOrder = sortOrder ?? "asc",
                            page = page,
                            pageSize = pageSize
                        },
                        PagedResult = result ?? new PagedResponse<UserListItem>()
                    };

                    return View(viewModel);
                }
                return View("Error", "Shared");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error", "Shared");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var response = await _httpClient.PostAsync($"api/accounts/{id}/deactivate", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Account deactivated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to deactivate account.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var response = await _httpClient.PostAsync($"api/accounts/{id}/activate", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Account activated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to activate account.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Account/Create
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            return View();
        }

        // POST: /Admin/Account/Create
        [HttpPost]
        public async Task<IActionResult> Create(UserEditViewModel user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(user);
                }

                user.IsActive = true;
                var response = await _httpClient.PostAsJsonAsync("api/accounts", user);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Account created successfully";
                    return RedirectToAction(nameof(Index));
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to create account: {errorContent}");
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }

        // GET: /Admin/Account/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            try
            {
                var response = await _httpClient.GetAsync($"api/accounts/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var account = await response.Content.ReadFromJsonAsync<UserEditViewModel>();
                return View(account);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Error", "Shared");
            }
        }

        // POST: /Admin/Account/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserEditViewModel user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (id != user.UserID)
            {
                return BadRequest();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(user);
                }

                var response = await _httpClient.PutAsJsonAsync($"api/accounts/{id}", user);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Account updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to update account: {errorContent}");
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }
    }
}



