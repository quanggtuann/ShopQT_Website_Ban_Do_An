using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ShopView.Controllers
{
    public class LanguageController : Controller
    {
        private static readonly HashSet<string> SupportedCultures = new()
        {
            "vi",
            "en"
        };

        public IActionResult Set(string culture, string? returnUrl = null)
        {
            if (!SupportedCultures.Contains(culture))
            {
                culture = "en";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
