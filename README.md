# ReviewBooks - Backend API

## Overview
RESTful API backend cho ứng dụng review sách ReviewBooks. Xây dựng với ASP.NET Core 9.0, Entity Framework Core, SQL Server và JWT authentication.

## 🏗️ Architecture
- **Framework**: ASP.NET Core 9.0 Web API
- **Database**: SQL Server (ReviewBooksDB)
- **ORM**: Entity Framework Core 9.0
- **Authentication**: JWT Bearer Token
- **Password Hashing**: BCrypt.Net
- **API Integration**: Google Books API
- **Pattern**: 3-layer architecture (Controller → Service → Repository)

## 📦 Modules

### 1. Auth Module
JWT-based authentication với role-based access control.

**Features:**
- Register user với email, password, username
- Login trả về JWT token (24h expiration)
- Role: User, Admin
- Password hashing với BCrypt

**Endpoints:**
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login và get JWT token

**Documentation:** [Auth/README.md](./ReviewBooks/Auth/README.md)

### 2. Users Module
Quản lý user profiles với role-based permissions.

**Features:**
- View all users (Admin only)
- View user profile (own or Admin)
- Update profile (own or Admin)
- Admin can update email/role
- Delete account (own or Admin)

**Endpoints:**
- `GET /api/users` - Get all users (Admin only)
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/me` - Get current user
- `PUT /api/users/{id}` - Update user profile
- `PUT /api/users/admin/{id}` - Admin update (Admin only)
- `DELETE /api/users/{id}` - Delete user

**Documentation:** [Users/README.md](./ReviewBooks/Users/README.md)

### 3. Books Module
Google Books API integration với database caching.

**Features:**
- Search books từ Google Books API
- Get book details by ID
- 24-hour cache TTL
- Auto-save results to database
- Pagination và filtering

**Endpoints:**
- `GET /api/books/{id}` - Get book by Google volumeId
- `GET /api/books/search` - Search books với query, pagination, sort

**Documentation:** [Books/README.md](./ReviewBooks/Books/README.md)

### 4. Reviews Module
Book reviews với role-based authorization.

**Features:**
- Create/Read/Update/Delete reviews
- Rating (1-5) và comment
- User can manage own reviews
- Admin can manage all reviews
- Pagination, search, filter by author

**Endpoints:**
- `GET /api/reviews` - Get all reviews với pagination
- `GET /api/reviews/book/{bookId}` - Reviews by book
- `GET /api/reviews/user/{userId}` - Reviews by user
- `GET /api/reviews/{id}` - Get review by ID
- `POST /api/reviews` - Create review (Auth required)
- `PUT /api/reviews/{id}` - Update review (Auth required)
- `DELETE /api/reviews/{id}` - Delete review (Auth required)

**Documentation:** [Reviews/README.md](./ReviewBooks/Reviews/README.md)

### 5. Forum Module
Discussion forum với posts và comments.

**Features:**
- Create/Read/Update/Delete posts
- Create/Read/Update/Delete comments
- Pin posts (Admin only)
- Lock posts (Admin only)
- View count tracking
- Pagination và search

**Endpoints:**
- `GET /api/forum/posts` - Get all posts
- `POST /api/forum/posts` - Create post (Auth required)
- `PUT /api/forum/posts/{id}` - Update post (Auth required)
- `DELETE /api/forum/posts/{id}` - Delete post (Auth required)
- `PATCH /api/forum/posts/{id}/pin` - Pin/Unpin (Admin only)
- `PATCH /api/forum/posts/{id}/lock` - Lock/Unlock (Admin only)
- `GET /api/forum/posts/{postId}/comments` - Get comments
- `POST /api/forum/posts/{postId}/comments` - Create comment (Auth required)
- `PUT /api/forum/comments/{id}` - Update comment (Auth required)
- `DELETE /api/forum/comments/{id}` - Delete comment (Auth required)

**Documentation:** [Forum/README.md](./ReviewBooks/Forum/README.md)

### 6. Favorite Module
User favorite books management.

**Features:**
- Add book to favorites
- Remove book from favorites
- View favorite list
- Check if book is favorited
- User-only (không dành cho Admin)
- Auto-fetch từ Google Books API

**Endpoints:**
- `GET /api/favorites` - Get my favorites (Auth required)
- `POST /api/favorites` - Add to favorites (Auth required)
- `DELETE /api/favorites/{bookId}` - Remove from favorites (Auth required)
- `GET /api/favorites/check/{bookId}` - Check if favorited (Auth required)

**Documentation:** [Favorite/README.md](./ReviewBooks/Favorite/README.md)

## 🔐 Authentication & Authorization

### JWT Configuration
```json
{
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "ReviewBooksAPI",
    "Audience": "ReviewBooksClient",
    "ExpiryInHours": 24
  }
}
```

### Roles
- **User**: Standard user với quyền quản lý own content
- **Admin**: Full access, quản lý all content

### Authorization Matrix

| Module | Endpoint | Public | User | Admin |
|--------|----------|--------|------|-------|
| Auth | Register/Login | ✅ | ✅ | ✅ |
| Users | GET all | ❌ | ❌ | ✅ |
| Users | GET own profile | ❌ | ✅ | ✅ |
| Users | UPDATE own | ❌ | ✅ | ✅ |
| Users | DELETE own | ❌ | ✅ | ✅ |
| Users | Admin operations | ❌ | ❌ | ✅ |
| Books | Search/Get | ✅ | ✅ | ✅ |
| Reviews | GET | ✅ | ✅ | ✅ |
| Reviews | POST | ❌ | ✅ | ✅ |
| Reviews | PUT own | ❌ | ✅ | ✅ |
| Reviews | PUT any | ❌ | ❌ | ✅ |
| Reviews | DELETE own | ❌ | ✅ | ✅ |
| Reviews | DELETE any | ❌ | ❌ | ✅ |
| Forum | GET posts/comments | ✅ | ✅ | ✅ |
| Forum | POST | ❌ | ✅ | ✅ |
| Forum | PUT own | ❌ | ✅ | ✅ |
| Forum | PUT any | ❌ | ❌ | ✅ |
| Forum | DELETE own | ❌ | ✅ | ✅ |
| Forum | DELETE any | ❌ | ❌ | ✅ |
| Forum | Pin/Lock | ❌ | ❌ | ✅ |
| Favorite | All | ❌ | ✅ | ❌ |

## 🗄️ Database Schema

### Tables
1. **Users** - User accounts và profiles
2. **Books** - Cached books from Google Books API
3. **BookCaches** - JSON cache với TTL
4. **Reviews** - Book reviews
5. **ForumPosts** - Forum discussion posts
6. **ForumComments** - Comments on forum posts
7. **UserFavoriteBooks** - Many-to-many join table

### Relationships
```
User (1) ----< (N) Review
User (1) ----< (N) ForumPost
User (1) ----< (N) ForumComment
User (M) ----< (N) Book (via UserFavoriteBooks)
Book (1) ----< (N) Review
ForumPost (1) ----< (N) ForumComment
```

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server 2019+
- Google Books API Key

### Configuration

1. **appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ReviewBooksDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "ReviewBooksAPI",
    "Audience": "ReviewBooksClient",
    "ExpiryInHours": 24
  },
  "GoogleBooks": {
    "BaseUrl": "https://www.googleapis.com/books/v1",
    "ApiKey": "YOUR_GOOGLE_BOOKS_API_KEY"
  }
}
```

2. **Database Migration**
```bash
cd ReviewBooks
dotnet ef database update
```

3. **Run**
```bash
dotnet run
```

API sẽ chạy tại: `https://localhost:7xxx`

### First User (Admin)
Sau khi database được tạo, register user đầu tiên:
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Admin@123",
  "username": "admin"
}
```

Sau đó update role thành Admin trực tiếp trong database:
```sql
UPDATE Users SET Role = 'Admin' WHERE Email = 'admin@example.com'
```

## 📡 API Usage Examples

### 1. Register & Login
```bash
# Register
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "User@123",
  "username": "johndoe"
}

# Login
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "User@123"
}

# Response
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-10-24T10:30:00Z",
  "userInfo": {
    "userId": "guid",
    "email": "user@example.com",
    "username": "johndoe",
    "role": "User"
  }
}
```

### 2. Search Books
```bash
GET /api/books/search?query=gatsby&pageNumber=1&pageSize=10
```

### 3. Create Review (với JWT)
```bash
POST /api/reviews
Authorization: Bearer <your-jwt-token>
Content-Type: application/json

{
  "bookId": "NgvxAAAAMAAJ",
  "rating": 5,
  "comment": "Excellent book!"
}
```

### 4. Add to Favorites
```bash
POST /api/favorites
Authorization: Bearer <your-jwt-token>
Content-Type: application/json

{
  "bookId": "NgvxAAAAMAAJ"
}
```

### 5. Create Forum Post
```bash
POST /api/forum/posts
Authorization: Bearer <your-jwt-token>
Content-Type: application/json

{
  "title": "Discussion about The Great Gatsby",
  "content": "What are your thoughts on..."
}
```

## 🛠️ Development

### Project Structure
```
ReviewBooks/
├── Auth/
│   ├── Controller/
│   ├── Dto/
│   ├── Models/
│   ├── Repository/
│   └── Services/
├── Users/
│   ├── Controller/
│   ├── Dto/
│   ├── Models/
│   ├── Repository/
│   └── Services/
├── Books/
│   ├── Controller/
│   ├── Dto/
│   ├── Models/
│   ├── Repository/
│   └── Services/
├── Reviews/
│   ├── Controller/
│   ├── Dto/
│   ├── Models/
│   ├── Repository/
│   └── Services/
├── Forum/
│   ├── Controller/
│   ├── Dto/
│   ├── Model/
│   ├── Repository/
│   └── Service/
├── Favorite/
│   ├── Controller/
│   ├── Dto/
│   ├── Models/
│   ├── Repository/
│   └── Services/
├── Data/
│   └── ApplicationDbContext.cs
├── Migrations/
├── Program.cs
└── Shared.cs
```

### Key Dependencies
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.9" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.9" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.10" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
```

### Build & Test
```bash
# Build
dotnet build

# Run
dotnet run

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Drop database
dotnet ef database drop --force
```

## 📝 API Documentation

- **Swagger UI**: Available at `/swagger` when running in Development mode
- **Module READMEs**: Each module has detailed documentation
  - [AUTHORIZATION.md](./ReviewBooks/AUTHORIZATION.md) - Comprehensive RBAC documentation
  - [Auth/README.md](./ReviewBooks/Auth/README.md)
  - [Books/README.md](./ReviewBooks/Books/README.md)
  - [Reviews/README.md](./ReviewBooks/Reviews/README.md)
  - [Forum/README.md](./ReviewBooks/Forum/README.md)
  - [Favorite/README.md](./ReviewBooks/Favorite/README.md)

## 🔒 Security Features

1. **JWT Authentication**: Bearer token với 24h expiration
2. **Password Hashing**: BCrypt với salt
3. **Role-Based Access Control**: User/Admin roles
4. **Authorization Checks**: Service-layer validation
5. **HTTPS**: Enforced trong production
6. **SQL Injection Prevention**: EF Core parameterized queries

## 🌐 External APIs

### Google Books API
- **Purpose**: Search và retrieve book information
- **Base URL**: `https://www.googleapis.com/books/v1`
- **Endpoints Used**:
  - `GET /volumes/{volumeId}` - Get book details
  - `GET /volumes?q={query}` - Search books
- **Caching**: 24-hour TTL in database

## ⚙️ Configuration Options

### JWT Settings
- `Key`: Secret key (minimum 32 characters)
- `Issuer`: Token issuer
- `Audience`: Token audience
- `ExpiryInHours`: Token expiration (default: 24)

### Google Books
- `BaseUrl`: API base URL
- `ApiKey`: Google Books API key

### Database
- `DefaultConnection`: SQL Server connection string

## 🐛 Error Handling

All endpoints return standard HTTP status codes:
- `200 OK`: Success
- `201 Created`: Resource created
- `204 No Content`: Success with no return data
- `400 Bad Request`: Invalid input
- `401 Unauthorized`: Missing or invalid JWT token
- `403 Forbidden`: Valid token but insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

Error response format:
```json
{
  "message": "Error description"
}
```

## 👥 Team
- LTWeb-4P

## 📄 License
[Your License Here]

## 🔄 Version
Current Version: 1.0.0
