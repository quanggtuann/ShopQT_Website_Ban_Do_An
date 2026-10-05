using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using ShopView.Infrastructure;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToAccessDenied = async context =>
        {
            if (context.Request.Path.StartsWithSegments("/Cart")
                || context.Request.Path.StartsWithSegments("/Favorite"))
            {
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var returnUrl = Uri.EscapeDataString(context.Request.PathBase + context.Request.Path + context.Request.QueryString);
                context.Response.Redirect($"/Account/Login?returnUrl={returnUrl}");
                return;
            }

            context.Response.Redirect(context.RedirectUri);
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddTransient<JwtBearerHandler>();
builder.Services.AddScoped<ICartCountProvider, CartCountProvider>();

builder.Services.AddHttpClient("ShopAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7130/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<JwtBearerHandler>();

var app = builder.Build();
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("vi")
};

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Food}/{action=Index}/{id?}");

app.Run();
