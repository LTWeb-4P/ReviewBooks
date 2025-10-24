# Module Book (Google Books + Cache)

Tài liệu này giải thích thiết kế module Book trong ReviewBooks, cách tách lớp, luồng cache, cấu hình và cách sử dụng API.

## Mục tiêu
- Giảm số lần gọi Google Books API bằng cách cache dữ liệu thô (JSON) và bản chuẩn hoá (entity Book) vào DB.
- Dễ bảo trì theo mô hình 3 lớp: Repository, Service, Controller.

## Kiến trúc tổng quan
- Book dùng để hiển thị/trao đổi dữ liệu trong hệ thống (đã chuẩn hoá các trường cần thiết).
- BookCache dùng để lưu toàn bộ JSON thô trả về từ Google Books theo `BookId` (volumeId) + thời điểm cache `CachedAt`.
- TTL cache mặc định 24 giờ.

```
Client  →  Controller  →  Service  →  Repository  →  DB
                         │             ↑
                         └── Google Books API (khi cache miss/hết hạn)
```

## Thư mục và lớp chính
- Controller: `Books/Controller/BookController.cs`
  - Endpoint: `GET /api/books/{id}` → trả `BookDto`
- Service: `Books/Services/BookService.cs`
  - Nhiệm vụ: Áp dụng logic cache, gọi Google Books API, parse JSON, upsert Book + BookCache.
- Repository:
  - `Books/Repository/IBookRepository.cs`
  - `Books/Repository/BookRepository.cs`
  - Nhiệm vụ: Giao tiếp DB (EF Core) cho Book và BookCache (get/upsert).
- Models:
  - `Books/Models/Book.cs` (Id: string – volumeId)
  - `Books/Models/BookCache.cs` (BookId: string, JsonData, CachedAt)
- DTO:
  - `Books/Dto/BookDto.cs`

## Hợp đồng Repository
`IBookRepository`
- `Task<Book?> GetBookByIdAsync(string id)`
- `Task<BookCache?> GetBookCacheAsync(string bookId)`
- `Task UpsertBookAsync(Book book)`
- `Task UpsertBookCacheAsync(BookCache cache)`

Mục đích: gom toàn bộ truy cập DB; Service không thao tác DbContext trực tiếp.

## Luồng xử lý Service (GetById)
1) Kiểm tra `BookCache` theo `BookId`:
   - Nếu cache còn hạn TTL (24h):
     - Ưu tiên trả `Book` từ DB nếu có (nhanh và đã chuẩn hoá).
     - Nếu chưa có `Book` trong DB, parse từ `JsonData` của cache (không ghi DB).
2) Nếu không có cache hoặc hết hạn:
   - Gọi Google Books API `GET /volumes/{id}` (kèm API key nếu cấu hình).
   - Parse JSON ra `BookDto` và upsert `Book` + `BookCache`.
3) Trả về `BookDto` cho client.

## API
- `GET /api/books/{id}`
  - `{id}` là `volumeId` của Google Books (ví dụ: `zyTCAlFPjgYC`).
  - Response mẫu (rút gọn):

```json
{
  "id": "zyTCAlFPjgYC",
  "title": "Example Title",
  "authors": "Author A, Author B",
  "publisher": "Example Publisher",
  "description": "...",
  "thumbnail": "https://...",
  "averageRating": 4.5,
  "ratingsCount": 123
}
```

## Cấu hình
- `appsettings.json` / `appsettings.Development.json`

```json
{
  "GoogleBooks": {
    "BaseUrl": "https://www.googleapis.com/books/v1",
    "ApiKey": "<API_KEY>"
  }
}
```

- Ở môi trường Development, khoá `ApiKey` đã được đặt sẵn theo yêu cầu.
- Có thể mở rộng thêm `CacheTtlHours` để cấu hình TTL động.

## EF Core và DB
- `Book.Id` là string (volumeId), KHÔNG phải Guid.
- Đã có `DbSet<Book>` và `DbSet<BookCache>` trong `ApplicationDbContext`.
- Quan hệ: `Review.BookId (string)` → `Book.Id (string)`.
- Sau khi đổi kiểu Id và đổi entity cache, cần migration + update DB:
  - Đã chuẩn bị: `UpdateBookToStringIdAndCache`
  - Cập nhật DB:
    - `dotnet ef database update --project ReviewBooks/ReviewBooks.csproj`

## Đăng ký DI
Trong `Program.cs`:
- `AddHttpClient()`
- `AddScoped<IBookRepository, BookRepository>()`
- `AddScoped<IBookService, BookService>()`

## Lỗi thường gặp & gợi ý xử lý
- 404 NotFound: Sai `volumeId` hoặc API trả về lỗi.
- 401/403 từ Google: Chưa cấu hình `ApiKey` hoặc key hết hạn/quyền không đủ.
- Lỗi DB/Migration: Chưa update DB sau khi đổi kiểu khoá. Chạy migration + update DB.

## Hướng phát triển
- Thêm endpoint search `/api/books/search?q=...` (không lưu DB, có thể cache ngắn hạn in-memory).
- Cho phép cấu hình TTL qua `GoogleBooks:CacheTtlHours`.
- Thêm chỉ mục (index) cho `BookCache.BookId` để tăng tốc lookup.
