# ShopQT

Giải pháp ASP.NET Core (.NET 8) gồm 3 project:

- `ShopAPI`: Web API + Swagger, cung cấp dữ liệu (Food/Category) và phục vụ ảnh tĩnh từ `wwwroot`.
- `ShopDAL`: Data Access Layer (Entity Framework Core) + Models + Migrations.
- `ShopView`: Web MVC (UI) gọi API thông qua `HttpClient` (named client `ShopAPI`) và có khu vực quản trị (`/Admin`).

## Yêu cầu

- .NET SDK 8.x (hoặc mới hơn, build được `net8.0`)
- SQL Server (khuyến nghị SQL Server Express)

## Cấu hình database

`ShopAPI` đang đọc connection string từ `D:\ShopDAL\ShopAPI\appsettings.json` (key: `ConnectionStrings:DefaultConnection`).

Ví dụ (bạn cần đổi lại cho đúng máy của bạn):

```json
"DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=ShopQT;Trusted_Connection=True;TrustServerCertificate=true;"
```

## Chạy migration (tạo DB)

Chạy ở thư mục gốc solution `D:\ShopDAL`:

```powershell
dotnet tool restore
dotnet ef database update --project .\ShopDAL\ShopDAL.csproj --startup-project .\ShopAPI\ShopAPI.csproj
```

Ghi chú:
- Migrations nằm trong `ShopDAL\Migrations`.
- `--startup-project ShopAPI` để lấy config connection string từ `ShopAPI\appsettings.json`.

## Chạy hệ thống (local)

1) Chạy API:

```powershell
dotnet run --project .\ShopAPI\ShopAPI.csproj
```

- Mặc định (theo `launchSettings.json`): `https://localhost:7130` (Swagger: `/swagger`)

2) Chạy Web UI:

```powershell
dotnet run --project .\ShopView\ShopView.csproj
```

- Mặc định (theo `launchSettings.json`): `https://localhost:7106`

## Liên kết ShopView ↔ ShopAPI

- `ShopView` cấu hình base address tại `D:\ShopDAL\ShopView\Program.cs` (named client `ShopAPI`).
  - Hiện đang là `https://localhost:7130/` (khớp với ShopAPI).
- `ShopAPI` bật CORS policy `"AllowShopView"` tại `D:\ShopDAL\ShopAPI\Program.cs`.
  - Nếu bạn đổi port của `ShopView`, hãy thêm origin tương ứng (ví dụ `https://localhost:7106`) vào danh sách `WithOrigins(...)`.

## API chính

Xem chi tiết tại `D:\ShopDAL\docs\API.md`.

## Thư mục ảnh món ăn

- Khi tạo/cập nhật món ăn, `ShopAPI` lưu ảnh vào `ShopAPI\wwwroot\img\foods`.
- Ảnh được phục vụ qua URL: `https://localhost:7130/img/foods/<ten_file>`

## Troubleshooting nhanh

- Nếu `ShopView` gọi API bị lỗi CORS: kiểm tra danh sách `WithOrigins(...)` trong `D:\ShopDAL\ShopAPI\Program.cs` có chứa đúng URL của `ShopView`.
- Nếu `dotnet restore/build` bị lỗi nguồn NuGet: kiểm tra cấu hình NuGet trên máy/CI (nuget.org, proxy, quyền truy cập file `NuGet.Config`).

