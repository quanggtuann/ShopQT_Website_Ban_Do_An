# ShopQT - E-Commerce Food Ordering System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API%20%7C%20MVC-5C2D91)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-6D4C41)](https://docs.microsoft.com/ef/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927)](https://www.microsoft.com/sql-server)

> Full-stack e-commerce food ordering application built with .NET 8, following 3-Layer Architecture, RESTful API principles, and Repository Pattern.

---

# 🚀 Quick Summary

|                  |                                                                             |
| :--------------- | :-------------------------------------------------------------------------- |
| **Features**     | Online food ordering, shopping cart, order management, Admin/Customer roles |
| **Architecture** | 3-Layer: MVC Presentation → Web API → Data Access                           |
| **Tech Stack**   | .NET 8, ASP.NET Core MVC & Web API, EF Core, SQL Server                     |
| **Patterns**     | Repository, Dependency Injection, DTO, SOLID                                |

---

# 📐 System Architecture

```text
┌─────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER (ShopView - ASP.NET Core MVC)       │
│  • Razor Views + Bootstrap UI                           │
│  • HttpClient communication with API                    │
│  • Session Authentication                               │
│  • Areas: /Admin & /Customer                            │
└─────────────────────────┬───────────────────────────────┘
                          │ HTTP/REST + JSON
┌─────────────────────────▼───────────────────────────────┐
│  API LAYER (ShopAPI - ASP.NET Core Web API)             │
│  • RESTful Controllers                                  │
│  • Business Services                                    │
│  • DTO Mapping                                          │
│  • Swagger Documentation                                │
│  • CORS + Static File Handling                          │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│  DATA ACCESS LAYER (ShopDAL - Class Library)            │
│  • Repository Pattern                                   │
│  • Entity Framework Core (Code-First)                   │
│  • SQL Server + Migrations                              │
└─────────────────────────────────────────────────────────┘
```

---

# ✨ Key Features

## 👨‍💼 Admin Portal (`/Admin`)

| Feature              | Technical Description                                         |
| -------------------- | ------------------------------------------------------------- |
| **Food Management**  | CRUD operations, image upload, filtering, sorting, pagination |
| **Combo Management** | Create combo meals from multiple food items                   |
| **Order Management** | View and update order statuses                                |
| **User Management**  | CRUD users, role management, activate/deactivate accounts     |

---

## 🛒 Customer Portal

| Feature             | Technical Description                                   |
| ------------------- | ------------------------------------------------------- |
| **Menu Browsing**   | Search, filter by category/price, sorting, pagination   |
| **Shopping Cart**   | Add/remove/update quantity, automatic total calculation |
| **Order Placement** | Create orders from cart, order history                  |
| **Authentication**  | Register/login with session-based authentication        |

---

# 🔌 API Design

* RESTful API endpoints
* Proper HTTP status codes
* DTO request/response separation
* Pagination support
* Filtering & sorting
* Service Layer for business logic
* Repository Pattern for data access abstraction

---

# 🛠️ Technology Stack

## Backend

* .NET 8 + C# 12
* ASP.NET Core Web API
* ASP.NET Core MVC
* Entity Framework Core 8
* SQL Server

---

## Frontend

* Razor Views
* Bootstrap 5
* jQuery + AJAX
* Font Awesome

---

## Patterns & Practices

* ✅ Repository Pattern
* ✅ Dependency Injection
* ✅ DTO Pattern
* ✅ 3-Layer Architecture
* ✅ SOLID Principles
* ✅ Separation of Concerns

---

# 🗄️ Main Entities

* FoodItem
* Category
* Combo
* ComboFoodItem
* Order
* OrderDetail
* Account
* Cart

---

# 📁 Project Structure

```text
ShopQT/
├── ShopAPI/                    # API Layer
│   ├── Controllers/            # REST API endpoints
│   ├── Services/               # Business logic layer
│   ├── DTOs/                   # Request/Response DTOs
│   ├── wwwroot/img/            # Static image storage
│   └── Program.cs              # Dependency Injection, CORS, Swagger
│
├── ShopDAL/                    # Data Access Layer
│   ├── Areas/Repository/       # Repository implementations
│   ├── Context/                # DbContext
│   ├── Models/                 # Entities + shared DTOs
│   └── Migrations/             # EF Core migrations
│
└── ShopView/                   # Presentation Layer (MVC)
    ├── Areas/
    │   ├── Admin/
    ├── Controllers/
    ├── Models/
    └── Views/
```

---

# 🚀 Getting Started

## Prerequisites

* .NET SDK 8.0+
* SQL Server 2019+
* Visual Studio 2022

---

## Installation

```powershell
# Clone repository
git clone <repo-url>

cd ShopQT

# Update connection string in appsettings.json

# Apply migrations
dotnet ef database update --project .\ShopDAL\ShopDAL.csproj --startup-project .\ShopAPI\ShopAPI.csproj

# Run API
dotnet run --project .\ShopAPI\ShopAPI.csproj

# Run MVC application (new terminal)
dotnet run --project .\ShopView\ShopView.csproj
```

---

## Default URLs

| Application | URL                            |
| ----------- | ------------------------------ |
| API         | https://localhost:7130         |
| Swagger     | https://localhost:7130/swagger |
| MVC Web     | https://localhost:7106         |

---

# 🎯 What I Learned

| Skill                            | Description                                        |
| -------------------------------- | -------------------------------------------------- |
| **Architecture Design**          | Designing scalable 3-layer systems                 |
| **RESTful API Development**      | Building APIs with proper HTTP conventions         |
| **Database Design**              | EF Core Code-First, migrations, relationships      |
| **Frontend-Backend Integration** | HttpClient communication, DTO mapping              |
| **File Handling**                | Image upload and static file serving               |
| **Authentication**               | Session-based authentication and authorization     |
| **Refactoring**                  | Moving business logic from controllers to services |

---

# 🔧 Refactoring Journey

## Before

* ❌ Controllers contained business logic
* ❌ Filtering and mapping directly inside controllers
* ❌ Code duplication
* ❌ Difficult to maintain and test

---

## After

* ✅ Business logic moved to Service Layer
* ✅ Controllers handle only HTTP requests/responses
* ✅ Cleaner architecture
* ✅ Easier to maintain and extend
* ✅ Better separation of concerns

---

# 📸 Screenshots

> Screenshots will be added later.

---

# 👤 Author

**Nguyễn Quang Tuấn**
.NET Backend Developer Intern

* 📧 Email: [tuannqph51813@gmail.com](mailto:tuannqph51813@gmail.com)
* 🐱 GitHub: https://github.com/quanggtuaann

---

# 📝 License

MIT License
