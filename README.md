# ShopQT - E-Commerce Food Ordering System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API%20%7C%20MVC-5C2D91)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-6D4C41)](https://docs.microsoft.com/ef/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927)](https://www.microsoft.com/sql-server)

> **Full-stack e-commerce application** xây dựng với **.NET 8**, áp dụng **3-layer architecture**, **RESTful API**, và **Repository Pattern**.

## 🚀 Quick Summary

| | |
|:---|:---|
| **Chức năng** | Đặt món ăn online với giỏ hàng, quản lý đơn hàng, phân quyền Admin/Customer |
| **Kiến trúc** | 3-Layer: Presentation (MVC) → API → Data Access |
| **Công nghệ** | .NET 8, ASP.NET Core Web API + MVC, EF Core, SQL Server |
| **Patterns** | Repository, Dependency Injection, DTO, SOLID |

---

## 📐 System Architecture

```
┌─────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER (ShopView - ASP.NET Core MVC)       │
│  • Razor Views + Bootstrap UI                           │
│  • HttpClient gọi API                                     │
│  • Session Authentication                                 │
│  • Areas: /Admin & /Customer                              │
└─────────────────────────┬───────────────────────────────┘
                          │ HTTP/REST + JSON
┌─────────────────────────▼───────────────────────────────┐
│  API LAYER (ShopAPI - ASP.NET Core Web API)             │
│  • RESTful Controllers                                    │
│  • Business Services (mới thêm)                         │
│  • Swagger Documentation                                  │
│  • CORS, Static Files (hình ảnh)                        │
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

## ✨ Key Features

### 👨‍💼 Admin Portal (`/Admin`)
| Feature | Mô tả kỹ thuật |
|---------|---------------|
| **Food Management** | CRUD, upload ảnh, phân loại, filter/sort/pagination |
| **Combo Management** | Tạo combo từ nhiều món, tính giá tự động |
| **Order Management** | Xem đơn hàng, cập nhật trạng thái |
| **User Management** | CRUD user, phân quyền, activate/deactivate |

### 🛒 Customer Portal
| Feature | Mô tả kỹ thuật |
|---------|---------------|
| **Menu Browsing** | Tìm kiếm, filter theo giá/danh mục, sort, pagination |
| **Shopping Cart** | Thêm/xóa/sửa số lượng, tính tổng tiền real-time |
| **Order Placement** | Tạo đơn từ giỏ hàng, lịch sử đơn hàng |
| **Authentication** | Đăng ký/đăng nhập, session-based auth |

---

## 🛠️ Technology Stack

### Backend
- **.NET 8** + **C# 12**
- **ASP.NET Core Web API** - RESTful endpoints
- **ASP.NET Core MVC** - Server-side rendering
- **Entity Framework Core 8** - ORM, Code-First
- **SQL Server** - Relational database

### Frontend
- **Razor Views** + **Bootstrap 5**
- **jQuery** + **AJAX** cho interactive components
- **Font Awesome** icons

### Patterns & Practices
- ✅ **Repository Pattern** - Tách Data Access
- ✅ **Dependency Injection** - Loose coupling
- ✅ **DTO Pattern** - Data transformation
- ✅ **3-Layer Architecture** - Separation of concerns
- ✅ **SOLID Principles**

---

## 📁 Project Structure

```
ShopQT/
├── ShopAPI/                    # API Layer (.NET Web API)
│   ├── Controllers/            # API endpoints
│   ├── Services/               # Business logic (mới)
│   ├── wwwroot/img/foods/      # Image storage
│   └── Program.cs              # DI, CORS, Swagger
│
├── ShopDAL/                    # Data Access Layer
│   ├── Areas/Repository/       # Repository implementation
│   ├── Context/                # DbContext
│   ├── Models/                 # Domain entities + DTOs
│   └── Migrations/             # EF Core migrations
│
└── ShopView/                   # Presentation Layer (MVC)
    ├── Areas/
    │   ├── Admin/              # Admin controllers/views
    │   └── Customer/           # Customer controllers/views
    ├── Controllers/            # Main controllers
    └── Views/                  # Razor views
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (Express or higher)

### Installation

```powershell
# 1. Clone repo
git clone <repo-url>
cd ShopQT

# 2. Update connection string trong ShopAPI/appsettings.json

# 3. Tạo database
dotnet ef database update --project .\ShopDAL\ShopDAL.csproj --startup-project .\ShopAPI\ShopAPI.csproj

# 4. Chạy API
dotnet run --project .\ShopAPI\ShopAPI.csproj
# API: https://localhost:7130
# Swagger: https://localhost:7130/swagger

# 5. Chạy Web UI (terminal mới)
dotnet run --project .\ShopView\ShopView.csproj
# Web: https://localhost:7106
```

---

## 🎯 What I Learned

| Kỹ năng | Mô tả |
|---------|-------|
| **Architecture Design** | Thiết kế hệ thống 3-layer, tách biệt concerns |
| **API Development** | Xây dựng RESTful API với proper HTTP status codes |
| **Database Design** | Code-First EF Core, migrations, relationships |
| **Frontend-Backend Integration** | HttpClient, CORS, DTOs |
| **File Handling** | Upload/download ảnh, static file serving |
| **Authentication** | Session-based auth, role-based authorization |

---

## 🔧 Refactoring Journey

### Trước
- ❌ Controller chứa business logic, filter, mapping
- ❌ Code duplication giữa các controller
- ❌ Khó unit test

### Sau
- ✅ Tách Service layer chứa business logic
- ✅ Controller chỉ còn routing + HTTP handling
- ✅ Dễ unit test, maintain, mở rộng

---

## 📸 Screenshots

<!-- Thêm ảnh chụp màn hình ở đây -->
> *Screenshots sẽ được cập nhật sau*

---

## 👤 Author

**Nguyễn Quang Tuấn** - .NET Backend Developer Intern

- 📧 Email: tuannqph51813@gmail.com
- 💼 LinkedIn: [linkedin.com/in/tuannq](https://linkedin.com/in/tuannq)
- 🐱 GitHub: [github.com/quanggtuann](https://github.com/quanggtuann)

> Project được xây dựng trong quá trình học tập tại trường FPT Polytechnic.

---

## 📝 License

MIT License - xem [LICENSE](LICENSE) để biết thêm chi tiết.
