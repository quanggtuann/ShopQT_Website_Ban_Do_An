using Microsoft.EntityFrameworkCore;
using ShopDAL.Areas.Repository;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS cho phép ShopView gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowShopView", policy =>
    {
        policy.WithOrigins("https://localhost:5000", "https://localhost:5001", "https://localhost:7000", "https://localhost:7001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//admin
builder.Services.AddScoped<IAdminFoodRepo, AdminFoodRepo>();
builder.Services.AddScoped<IAdminCategoryRepo, AdminCategoryRepo>();
builder.Services.AddScoped<IAdminAccountRepo, AdminAccountRepo>();
builder.Services.AddScoped<IAdminComboRepo, AdminComboRepo>();
builder.Services.AddScoped<IAdminOrderRepo, AdminOrderRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Phục vụ ảnh từ wwwroot
app.UseCors("AllowShopView");
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern : "{areas : exists}/{controller=Home}/{action=index}/{id?}"
    );
app.MapControllerRoute(
    name : "default",
    pattern : "{controller= Food}/{action= Index}/{id?}"
    );
app.MapControllers();

app.Run();
