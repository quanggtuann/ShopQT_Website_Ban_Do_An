# API (ShopAPI)

Base URL (local theo mặc định): `https://localhost:7130`

## Foods

### GET `/api/foods`

Query (tùy chọn):

- `page` (mặc định 1)
- `pageSize` (mặc định 5 theo ShopView, API đọc từ model filter)
- `keyword`
- `categoryId`
- `priceFrom`
- `priceTo`
- `sortBy`: `Name` | `Price`
- `sortOrder`: `asc` | `desc`

Response (rút gọn):

- `totalItems`
- `currentPage`
- `totalPages`
- `data`: danh sách `FoodItemDto`

### GET `/api/foods/{id}`

Lấy chi tiết 1 món ăn theo id.

### POST `/api/foods`

`multipart/form-data`:

- Fields theo model `FoodItem` (ví dụ `Name`, `Description`, `Price`, `CategoryId`, `IsAvailable`, ...)
- File ảnh: `imageFile` (tùy chọn)

### PUT `/api/foods/{id}`

`multipart/form-data`:

- Fields theo model `FoodItem`
- File ảnh: `formFile` (tùy chọn)

### PATCH `/api/foods/{id}/deactivate`

Tắt món ăn (soft deactivate).

### PATCH `/api/foods/{id}/activate`

Bật lại món ăn.

## Categories

### GET `/api/categoryes`

Query (tùy chọn):

- `activeOnly=true` để chỉ lấy category đang active

### GET `/api/categoryes/{id}`

### POST `/api/categoryes`

Body JSON:

- `name` (bắt buộc)
- `description` (tùy chọn)

### PUT `/api/categoryes/{id}`

Body JSON:

- `categoryId` (có thể bỏ qua; server sẽ set theo `{id}`)
- `name`
- `description`

### PATCH `/api/categoryes/{id}/deactivate`

### PATCH `/api/categoryes/{id}/activate`

