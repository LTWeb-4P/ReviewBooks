# Favorite Books Module

## Overview
Favorite module cho phép users quản lý danh sách sách yêu thích của họ. Module này chỉ dành cho User, Admin không cần quản lý favorites của users.

## Features
- ✅ View list of favorite books
- ✅ Add book to favorites (auto-fetch from Google Books API nếu chưa có trong DB)
- ✅ Remove book from favorites
- ✅ Check if book is in favorites
- ✅ User can only manage their own favorites
- ✅ JWT authentication required for all endpoints

## Architecture
Sử dụng many-to-many relationship có sẵn giữa `User` và `Book` thông qua join table `UserFavoriteBooks` (không cần bảng Favorite riêng).

## API Endpoints

### GET /api/favorites
Get current user's favorite books.
- **Auth**: Required (JWT Bearer token)
- **Authorization**: User can only view own favorites
- **Response**: IEnumerable<FavoriteBookDto>
```json
[
    {
        "bookId": "abc123",
        "title": "The Great Gatsby",
        "authors": "F. Scott Fitzgerald",
        "publisher": "Scribner",
        "description": "A classic novel...",
        "thumbnail": "https://...",
        "averageRating": 4.5,
        "ratingsCount": 1234,
        "addedAt": "2024-01-15T10:30:00Z"
    }
]
```

### POST /api/favorites
Add book to favorites.
- **Auth**: Required (JWT Bearer token)
- **Body**: AddFavoriteDto
```json
{
    "bookId": "abc123"
}
```
- **Behavior**:
  - If book exists in DB: Add directly to favorites
  - If book not in DB: Fetch from Google Books API, save to DB, then add to favorites
- **Response**: 200 OK
```json
{
    "message": "Book added to favorites successfully"
}
```
- **Errors**:
  - 400 Bad Request: Book already in favorites
  - 404 Not Found: Book not found in Google Books API
  - 401 Unauthorized: Invalid or missing token

### DELETE /api/favorites/{bookId}
Remove book from favorites.
- **Auth**: Required (JWT Bearer token)
- **Path Parameter**: `bookId` (string) - Google Books volume ID
- **Response**: 200 OK
```json
{
    "message": "Book removed from favorites successfully"
}
```
- **Errors**:
  - 404 Not Found: Book not in favorites
  - 401 Unauthorized: Invalid or missing token

### GET /api/favorites/check/{bookId}
Check if book is in user's favorites.
- **Auth**: Required (JWT Bearer token)
- **Path Parameter**: `bookId` (string) - Google Books volume ID
- **Response**: 200 OK
```json
{
    "bookId": "abc123",
    "isFavorite": true
}
```

## DTOs

### FavoriteBookDto
```csharp
{
    BookId: string           // Google Books volume ID
    Title: string?
    Authors: string?         // Comma-separated authors
    Publisher: string?
    Description: string?
    Thumbnail: string?       // Cover image URL
    AverageRating: double?
    RatingsCount: int?
    AddedAt: DateTime        // When added to favorites
}
```

### AddFavoriteDto
```csharp
{
    BookId: string (required)  // Google Books volume ID
}
```

## Authorization Rules
- **All endpoints**: Require JWT authentication
- **User scope**: Users can ONLY manage their own favorites
- **Admin**: No special privileges (Admin không cần quản lý favorites của users)
- **UserId**: Automatically extracted from JWT token (ClaimTypes.NameIdentifier)

## Database Schema

### UserFavoriteBooks Table (Join Table)
- Composite Primary Key: (UserId, BookId)
- Foreign Keys:
  - UserId -> Users(Id)
  - BookId -> Books(Id)
- Additional Column:
  - CreatedAt (datetime2) - DEFAULT GETUTCDATE()

**No separate Favorites table** - Sử dụng many-to-many relationship có sẵn.

## Integration with Other Modules

### Book Module
- **Auto-fetch**: Khi add favorite, nếu book chưa có trong DB sẽ tự động gọi `IBookService.GetByIdAsync()` để fetch từ Google Books API
- **Cache benefit**: Sách đã favorite sẽ được cache trong DB, giúp tăng tốc độ truy vấn

### User Module
- **Navigation property**: `User.FavoriteBooks` (ICollection<Book>)
- **Cascade**: Khi delete user, tất cả favorite entries cũng bị xóa

## Business Rules

1. **Duplicate prevention**: Không thể add cùng 1 book vào favorites 2 lần
2. **Auto-fetch**: Book tự động được fetch từ Google Books API nếu chưa có trong DB
3. **User isolation**: User chỉ có thể view/modify favorites của chính họ
4. **JWT required**: Tất cả endpoints đều require authentication

## Example Usage

### Get My Favorites
```http
GET /api/favorites
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Add to Favorites
```http
POST /api/favorites
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
    "bookId": "NgvxAAAAMAAJ"
}
```

### Remove from Favorites
```http
DELETE /api/favorites/NgvxAAAAMAAJ
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Check if Favorited
```http
GET /api/favorites/check/NgvxAAAAMAAJ
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Error Handling

All endpoints return standard error responses:
- **400 Bad Request**: Book already in favorites
- **401 Unauthorized**: Missing/invalid JWT token
- **404 Not Found**: Book not found (in favorites or Google Books API)
- **500 Internal Server Error**: Server-side error

Error response format:
```json
{
    "message": "Error description"
}
```

## Implementation Notes

1. **No Favorite Model**: Module không sử dụng `Favorite.cs` model vì đã có many-to-many relationship sẵn
2. **Repository pattern**: Sử dụng EF Core Include() để load `User.FavoriteBooks` navigation property
3. **Service integration**: Inject `IBookService` để fetch books từ Google Books API khi cần
4. **Performance**: Favorites được cache trong DB giống như Book module (24h TTL)
