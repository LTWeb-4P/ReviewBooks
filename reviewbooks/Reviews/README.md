# Module Review

Tài liệu này giải thích thiết kế module Review trong ReviewBooks, bao gồm phân trang, tìm kiếm, sắp xếp, lọc theo tác giả, và phân quyền theo vai trò (Admin/User).

## Mục tiêu
- Quản lý đánh giá (reviews) của người dùng cho sách.
- Hỗ trợ phân trang, tìm kiếm, sắp xếp theo nhiều tiêu chí.
- Lọc theo tác giả (userId) để xem reviews của một người dùng cụ thể.
- Phân quyền: Admin có thể sửa/xóa mọi review, User chỉ sửa/xóa của mình.
- Sử dụng 3 lớp: Repository, Service, Controller.

## Kiến trúc tổng quan

```
Client  →  Controller  →  Service  →  Repository  →  DB (EF Core)
                         │
                         └── Role-based logic (Admin/User)
```

## Thư mục và lớp chính

- **Controller**: `Reviews/Controller/ReviewController.cs`
  - Endpoints:
    - `GET /api/reviews` → Danh sách reviews (có pagination, search, filter, sort)
    - `GET /api/reviews/book/{bookId}` → Reviews của một cuốn sách
    - `GET /api/reviews/user/{userId}` → Reviews của một user
    - `GET /api/reviews/{id}` → Chi tiết một review
    - `POST /api/reviews` → Tạo review mới
    - `PUT /api/reviews/{id}` → Cập nhật review
    - `DELETE /api/reviews/{id}` → Xóa review

- **Service**: `Reviews/Services/ReviewService.cs`
  - Logic nghiệp vụ: filter, sort, pagination, role-based authorization.
  
- **Repository**:
  - `Reviews/Repository/IReviewRepository.cs`
  - `Reviews/Repository/ReviewRepository.cs`
  - Giao tiếp DB: CRUD, trả về `IQueryable` để service xử lý filter/sort hiệu quả.

- **Models**:
  - `Reviews/Models/Review.cs`
    - `Id` (Guid), `BookId` (string), `UserId` (Guid), `Rating` (1-5), `Comment`, `CreatedAt`, `UpdatedAt`
    - Navigation: `Book`, `User`

- **DTO**:
  - `Reviews/Dto/ReviewDto.cs`
    - `ReviewListDto`: Dùng cho danh sách (ít field hơn).
    - `ReviewDetailDto`: Dùng cho chi tiết (đầy đủ field).
    - `AddReviewRequestDto`: Tạo review mới.
    - `UpdateReviewRequestDto`: Cập nhật review (Rating, Comment).

## Hợp đồng Repository

`IReviewRepository`:
- `Task<IQueryable<Review>> GetReviewsQueryAsync()` → Trả IQueryable để service filter/sort.
- `Task<List<Review>> GetReviewsAsync()` → Lấy toàn bộ (ít dùng, thay bằng query).
- `Task<IEnumerable<Review>> GetReviewByBookIdAsync(string bookId)`
- `Task<IEnumerable<Review>> GetReviewByUserIdAsync(Guid userId)`
- `Task<Review?> GetReviewByIdAsync(Guid id)`
- `Task<Review> CreateReviewAsync(Review review)`
- `Task<Review?> UpdateReviewAsync(Guid id, Review review)`
- `Task<bool> DeleteReviewAsync(Guid id)`

## Luồng xử lý Service

### 1) GetReviewsAsync(Query)
- Normalize query (pageNumber, pageSize).
- Lấy `IQueryable<Review>` từ repository (Include User, Book).
- **Search**: Tìm theo `BookId`, `Username`, `BookTitle` (case-insensitive).
- **Filter by author**: Nếu `query.filterBy` là Guid (userId), lọc `UserId == filterBy`.
- **Sort**:
  - `sortBy=rating`: Sắp xếp theo Rating (asc/desc).
  - `sortBy=createdat`: Sắp xếp theo CreatedAt (asc/desc).
  - Mặc định: CreatedAt descending (mới nhất trước).
- **Pagination**: Skip, Take theo pageNumber và pageSize.
- Trả `PageResult<ReviewListDto>` (Items, TotalCount, PageSize, CurrentPage, TotalPages).

### 2) GetReviewsByBookIdAsync(bookId, Query)
- Lấy reviews của một cuốn sách cụ thể.
- Hỗ trợ sort và pagination.
- Trả `PageResult<ReviewDetailDto>`.

### 3) GetReviewsByUserIdAsync(userId, Query)
- Lấy reviews của một user cụ thể.
- Hỗ trợ sort và pagination.
- Trả `PageResult<ReviewDetailDto>`.

### 4) CreateReviewAsync(dto, currentUserId)
- Tạo review mới, gán `UserId = currentUserId` (từ user đang login).
- Gán `CreatedAt = UtcNow`.
- Lưu DB, trả `ReviewDetailDto`.

### 5) UpdateReviewAsync(id, dto, currentUserId, userRole)
- **Role-based logic**:
  - Admin: Có thể sửa mọi review.
  - User: Chỉ sửa review của mình (`review.UserId == currentUserId`).
- Nếu vi phạm → throw `UnauthorizedAccessException`.
- Cập nhật Rating, Comment (nếu có), gán `UpdatedAt = UtcNow`.
- Trả `ReviewDetailDto`.

### 6) DeleteReviewAsync(id, currentUserId, userRole)
- **Role-based logic**:
  - Admin: Có thể xóa mọi review.
  - User: Chỉ xóa review của mình.
- Nếu vi phạm → throw `UnauthorizedAccessException`.
- Xóa review, trả `bool`.

## API Endpoints

### GET /api/reviews
Danh sách reviews với pagination, search, filter, sort.

**Query params**:
- `search` (string, optional): Tìm kiếm theo BookId, Username, BookTitle.
- `filterBy` (Guid, optional): Lọc theo userId (tác giả review).
- `sortBy` (string, optional): `rating`, `createdat` (mặc định: `createdat`).
- `isDescending` (bool): `true` = giảm dần, `false` = tăng dần.
- `pageNumber` (int): Trang hiện tại (mặc định: 1).
- `pageSize` (int): Số item mỗi trang (mặc định: 10).

**Response**: `PageResult<ReviewListDto>`

**Ví dụ**:
```
GET /api/reviews?search=harry&filterBy=<userId>&sortBy=rating&isDescending=true&pageNumber=1&pageSize=20
```

### GET /api/reviews/book/{bookId}
Reviews của một cuốn sách, có sort và pagination.

**Response**: `PageResult<ReviewDetailDto>`

### GET /api/reviews/user/{userId}
Reviews của một user, có sort và pagination.

**Response**: `PageResult<ReviewDetailDto>`

### GET /api/reviews/{id}
Chi tiết một review.

**Response**: `ReviewDetailDto` hoặc `404 Not Found`

### POST /api/reviews
Tạo review mới.

**Headers** (tạm thời, sau này dùng JWT):
- `X-User-Id` (Guid): Id của user đang login.

**Body**: `AddReviewRequestDto`
```json
{
  "bookId": "abc123",
  "rating": 5,
  "comment": "Excellent book!"
}
```

**Response**: `ReviewDetailDto` (201 Created)

### PUT /api/reviews/{id}
Cập nhật review.

**Headers**:
- `X-User-Id` (Guid): Id của user đang login.
- `X-User-Role` (string): `Admin` hoặc `User`.

**Body**: `UpdateReviewRequestDto`
```json
{
  "rating": 4,
  "comment": "Updated comment"
}
```

**Response**: `ReviewDetailDto` hoặc `403 Forbidden` (nếu không có quyền)

### DELETE /api/reviews/{id}
Xóa review.

**Headers**:
- `X-User-Id` (Guid)
- `X-User-Role` (string)

**Response**: `204 No Content` hoặc `403 Forbidden`

## Phân quyền (Role-based)

Hiện tại chưa bật `[Authorize]` attribute. Controller nhận `currentUserId` và `userRole` từ:
- Header: `X-User-Id`, `X-User-Role`
- Query param: `currentUserId`, `userRole` (fallback)

Logic phân quyền trong Service:
- **Admin**: Có thể CREATE, UPDATE, DELETE mọi review.
- **User**: 
  - CREATE: review của mình (gán `UserId = currentUserId`).
  - UPDATE/DELETE: Chỉ review của mình (`review.UserId == currentUserId`).
  - Nếu vi phạm → `UnauthorizedAccessException` → Controller trả `403 Forbidden`.

Sau này khi thêm JWT Authentication:
- Thay thế header/query bằng `ClaimsPrincipal` từ `HttpContext.User`.
- Bật `[Authorize]` attribute trên controller hoặc action.

## EF Core và DB

- **Review** entity có navigation properties: `Book`, `User`.
- Relationship đã cấu hình trong `ApplicationDbContext`:
  - `Review.BookId (string)` → `Book.Id (string)` (Cascade delete)
  - `Review.UserId (Guid)` → `User.Id (Guid)` (Cascade delete)
- Repository dùng `Include(r => r.User).Include(r => r.Book)` để eager load.

## AutoMapper

- Đã đăng ký `AutoMapper` trong `Program.cs`.
- Profile: `ReviewBooks.Reviews.Automapping.ReviewProfile`
  - `Review` → `ReviewDetailDto`
  - `AddReviewRequestDto` → `Review`
  - `UpdateReviewRequestDto` → `Review`

## Đăng ký DI

Trong `Program.cs`:
```csharp
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
```

## Lỗi thường gặp & gợi ý

- **403 Forbidden**: User cố sửa/xóa review của người khác. Kiểm tra `X-User-Id` và `X-User-Role`.
- **400 Bad Request**: Thiếu `currentUserId` khi POST/PUT/DELETE.
- **404 Not Found**: Review không tồn tại.
- **AutoMapper missing**: Đảm bảo đã đăng ký `AddAutoMapper(typeof(Program))`.

## Hướng phát triển

- Thêm JWT Authentication: Lấy userId và role từ Claims thay vì header/query.
- Bật `[Authorize]` attribute với policy-based authorization (Admin, User).
- Thêm validation: User không thể review cùng một sách nhiều lần (unique constraint `UserId + BookId`).
- Thêm endpoint thống kê: Số lượng reviews, rating trung bình theo sách.
- Cache: Lưu rating trung bình vào `Book.AverageRating` khi có review mới/cập nhật/xóa.
