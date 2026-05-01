# ShopQT - E-Commerce Food Ordering System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API%20%7C%20MVC-5C2D91)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-6D4C41)](https://docs.microsoft.com/ef/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927)](https://www.microsoft.com/sql-server)

A full-stack e-commerce application for food ordering built with **.NET 8**, implementing **3-layer architecture** with **RESTful API** and **Repository Pattern**.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER                      │
│  ┌─────────────────────────────────────────────────────────┐│
│  │  ShopView (ASP.NET Core MVC)                            ││
│  │  - Razor Views + Bootstrap UI                           ││
│  │  - HttpClient integration with API                      ││
│  │  - Session-based authentication                       ││
│  │  - Areas: /Admin & /Customer                            ││
│  └─────────────────────────────────────────────────────────┘│
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTP/REST
┌──────────────────────────▼──────────────────────────────────┐
│                      API LAYER                               │
│  ┌─────────────────────────────────────────────────────────┐│
│  │  ShopAPI (ASP.NET Core Web API)                         ││
│  │  - RESTful Controllers                                  ││
│  │  - Swagger/OpenAPI Documentation                      ││
│  │  - CORS Policy for cross-origin requests                ││
│  │  - Static file serving (food images)                  ││
│  └─────────────────────────────────────────────────────────┘│
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│                    DATA ACCESS LAYER                         │
│  ┌─────────────────────────────────────────────────────────┐│
│  │  ShopDAL (Class Library)                                ││
│  │  - Repository Pattern (IRepository → Repository)        ││
│  │  - Entity Framework Core (Code-First)                 ││
│  │  - SQL Server with Migrations                         ││
│  │  - Domain Models & DTOs                               ││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
```

## Key Features

### Admin Portal (`/Admin`)
- **Food Management**: CRUD operations with image upload, category assignment
- **Combo Management**: Create food combos with pricing
- **Order Management**: View and manage customer orders
- **User Management**: Account CRUD, activate/deactivate, role assignment, filtering & sorting

### Customer Portal
- **Menu Browsing**: View foods and combos with pagination, search, category filter, price range filter, sorting
- **Shopping Cart**: Add/remove items, update quantities
- **Order Placement**: Create orders from cart, order history, order cancellation
- **Authentication**: Registration, login, profile management

### Planned Features
- **Google OAuth 2.0** integration for social login
- **VNPay Sandbox** integration for payment processing

## Technology Stack

| Category | Technologies |
|----------|--------------|
| **Backend** | C# 12, .NET 8, ASP.NET Core Web API, RESTful API, Repository Pattern, Dependency Injection |
| **Frontend** | ASP.NET Core MVC, Razor Views, HTML5, CSS3, Bootstrap 5 |
| **Database** | SQL Server, Entity Framework Core 8 (Code-First), LINQ |
| **Integration** | HttpClient, CORS, Swagger/OpenAPI |
| **Tools** | Visual Studio 2022, Git, Postman |

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (Express or higher)

## Getting Started

### 1. Database Configuration

Update the connection string in `ShopAPI/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=ShopQT;Trusted_Connection=True;TrustServerCertificate=true;"
}
```

### 2. Database Migration

Run from solution root directory:

```powershell
# Install EF Core tools (first time only)
dotnet tool restore

# Create database with migrations
dotnet ef database update --project .\ShopDAL\ShopDAL.csproj --startup-project .\ShopAPI\ShopAPI.csproj
```

> **Note**: Migrations are stored in `ShopDAL/Migrations`. The `--startup-project` parameter ensures the connection string is read from `ShopAPI/appsettings.json`.

### 3. Run the Application

**Step 1: Start the API Server**
```powershell
dotnet run --project .\ShopAPI\ShopAPI.csproj
```
- API: `https://localhost:7130`
- Swagger UI: `https://localhost:7130/swagger`

**Step 2: Start the Web UI (new terminal)**
```powershell
dotnet run --project .\ShopView\ShopView.csproj
```
- Web App: `https://localhost:7106`
- Admin Portal: `https://localhost:7106/Admin`

## API Documentation

Complete API documentation is available at:
- **Swagger UI**: `https://localhost:7130/swagger` (when API is running)
- **API Docs**: See `docs/API.md` for endpoint details

## Project Structure

```
ShopQT/
├── ShopAPI/                 # ASP.NET Core Web API
│   ├── Controllers/         # RESTful API Controllers
│   ├── wwwroot/img/foods/   # Food images storage
│   └── Program.cs           # DI Configuration, CORS
├── ShopDAL/                 # Data Access Layer
│   ├── Areas/
│   │   └── Repository/      # Repository Pattern implementation
│   │       └── Irepository/ # Repository interfaces
│   ├── Context/             # DbContext
│   ├── Models/              # Domain entities
│   └── Migrations/          # EF Core migrations
├── ShopView/                # ASP.NET Core MVC Frontend
│   ├── Areas/
│   │   └── Admin/           # Admin portal
│   └── Controllers/         # MVC Controllers
└── docs/
    └── API.md               # API documentation
```

## Key Design Patterns & Practices

- **Repository Pattern**: Abstraction layer between Controllers and DbContext
- **Dependency Injection**: Services registered in `Program.cs`, injected via constructors
- **DTO Pattern**: Data Transfer Objects for API responses
- **Areas**: Logical separation of Admin and Customer functionality
- **Code-First Migration**: Database schema managed through EF Core migrations

## Troubleshooting

| Issue | Solution |
|-------|----------|
| CORS Error | Update `WithOrigins()` in `ShopAPI/Program.cs` with your ShopView URL |
| Database connection | Verify SQL Server is running and connection string is correct |
| NuGet restore errors | Check NuGet.Config or run `dotnet restore --no-cache` |

---

## Author

**[Your Name]** - Fullstack .NET Developer Intern

- 📧 Email: your.email@example.com
- 💼 LinkedIn: [linkedin.com/in/yourprofile](https://linkedin.com/in/yourprofile)
- 🐱 GitHub: [github.com/yourusername](https://github.com/yourusername)

This project was built as a learning exercise to demonstrate proficiency in ASP.NET Core, Entity Framework Core, and modern web application architecture patterns.

## License

This project is open source and available under the [MIT License](LICENSE).
