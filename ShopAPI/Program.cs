using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopAPI.Configuration;
using ShopAPI.Services;
using ShopAPI.Services.IServices;
using ShopDAL.Areas.Repository;
using ShopDAL.Areas.Repository.Irepository;
using ShopDAL.Context;
using ShopDAL.Repository;
using ShopDAL.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration is missing.");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowShopView", policy =>
    {
        policy.WithOrigins("https://localhost:7106")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAdminFoodRepo, AdminFoodRepo>();
builder.Services.AddScoped<IAdminCategoryRepo, AdminCategoryRepo>();
builder.Services.AddScoped<IAdminAccountRepo, AdminAccountRepo>();
builder.Services.AddScoped<IAdminComboRepo, AdminComboRepo>();
builder.Services.AddScoped<IAdminOrderRepo, AdminOrderRepo>();
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IFoodRepo, FoodRepo>();

builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<ICategoryesService, CategoryesService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICustomerFoodService, CustomerFoodService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowShopView");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
