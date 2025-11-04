<div align="center">
  <h1> ReviewBooks Backend API</h1>
  <p>API Backend hiện đại cho nền tảng đọc sách và đánh giá sách xã hội</p>
  
  ![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=flat-square&logo=dotnet)
  ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Latest-336791?style=flat-square&logo=postgresql)
  ![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=flat-square&logo=jsonwebtokens)
  ![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)
</div>

---

##  Tổng Quan

ReviewBooks là một REST API backend toàn diện cung cấp nền tảng đọc sách xã hội, nơi người dùng có thể khám phá sách, viết đánh giá, tham gia thảo luận và quản lý danh sách đọc của mình. Được xây dựng với công nghệ .NET hiện đại và tuân thủ các best practices.

###  Tính Năng Chính

-  **Xác Thực Bảo Mật** - JWT authentication với xác minh email
-  **Quản Lý Sách** - Tích hợp Google Books API & OpenLibrary
-  **Hệ Thống Đánh Giá Kép** - Rating từ bên ngoài + rating cộng đồng
-  **Hệ Thống Review** - Người dùng đánh giá với tính toán rating tự động
-  **Diễn Đàn Thảo Luận** - Cộng đồng thảo luận về sách
-  **Danh Sách Yêu Thích** - Quản lý sách yêu thích cá nhân
-  **Quản Lý Người Dùng** - Phân quyền theo vai trò (User/Admin)
-  **Dịch Vụ Email** - Email xác minh với template HTML

---

##  Công Nghệ Sử Dụng

| Lớp                      | Công Nghệ                     |
| ------------------------ | ----------------------------- |
| **Framework**            | ASP.NET Core 9.0 Web API      |
| **Cơ Sở Dữ Liệu**       | PostgreSQL (Cloud: Aiven)     |
| **ORM**                  | Entity Framework Core 9.0     |
| **Xác Thực**             | JWT Bearer Tokens             |
| **Bảo Mật Mật Khẩu**     | BCrypt.Net-Next               |
| **Email**                | SMTP (Gmail/Ethereal)         |
| **API Bên Ngoài**        | Google Books API, OpenLibrary |
| **Mapping**              | AutoMapper                    |
| **Kiến Trúc**            | Repository-Service-Controller |

---

##  Yêu Cầu Hệ Thống

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) (hoặc cloud instance)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/) hoặc [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

---

##  Bắt Đầu Nhanh

### 1. Clone repository

```bash
git clone https://github.com/LTWeb-4P/ReviewBooks.git
cd ReviewBooks/reviewbooks
```

### 2. Cấu Hình

Cập nhật `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=reviewbooks;Username=postgres;Password=yourpassword;SslMode=Require"
  },
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "https://your-domain.com",
    "Audience": "https://your-domain.com"
  },
  "App": {
    "BackendUrl": "https://api.your-domain.com",
    "FrontendUrl": "https://your-domain.com"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromName": "ReviewBooks Team"
  },
  "GoogleBooks": {
    "BaseUrl": "https://www.googleapis.com/books/v1",
    "ApiKey": "your-google-books-api-key"
  }
}
```

### 3. Khởi Tạo Database

```bash
# Áp dụng migrations
dotnet ef database update

# Hoặc tạo migration mới nếu cần
dotnet ef migrations add InitialMigration
```

### 4. Chạy Ứng Dụng

```bash
dotnet run
```

Swagger UI: `http://localhost:8080/swagger`

---

##  Cấu Trúc Project

```
reviewbooks/
 Auth/                    # Xác thực & Phân quyền
 Users/                   # Quản lý người dùng
 Books/                   # Quản lý sách
 Reviews/                 # Hệ thống đánh giá
 Forum/                   # Diễn đàn thảo luận
 Favorite/                # Sách yêu thích
 Data/                    # Database Context
 Migrations/              # EF Core Migrations
 Program.cs               # Entry Point
```

---

##  Tính Năng Nổi Bật

###  Luồng Xác Thực

**1. Đăng Ký Với Xác Minh Email**

- Người dùng đăng ký  Lưu vào `PendingRegistrations` (chưa lưu vào `Users`)
- Gửi email xác minh với token (hết hạn sau 30 phút)
- Người dùng click link xác minh
- Backend validate  Tạo tài khoản trong `Users`
- Tự động đăng nhập với JWT token

**2. Đăng Nhập**

- Yêu cầu email đã được xác minh
- Xác minh mật khẩu với BCrypt
- Cấp JWT token (hết hạn 24 giờ)

###  Hệ Thống Đánh Giá Kép

**Rating Bên Ngoài** (Google Books)
- `AverageRating` - Rating từ Google
- `RatingsCount` - Số lượng rating

**Rating Hệ Thống** (Người dùng platform)
- `SystemAverageRating` - Tính từ reviews (1-5 sao)
- `SystemRatingsCount` - Số reviews
- **Tự động cập nhật** khi review tạo/sửa/xóa

---

##  Tính Năng Bảo Mật

-  JWT Bearer Authentication
-  Phân quyền theo vai trò (User/Admin)
-  Mã hóa mật khẩu BCrypt (salt rounds: 12)
-  Xác minh email bắt buộc
-  Token expiration (30 phút email, 24 giờ JWT)
-  CORS configuration
-  SQL injection prevention

---

##  Tài Liệu API

Swagger UI: `http://localhost:5072/swagger`

### Tài Liệu Module

- **[Hướng Dẫn Frontend Developer](./FRONTEND_GUIDE.md)** - Hướng dẫn tích hợp đầy đủ
- [Module Auth](./reviewbooks/Auth/README.md)
- [Module Users](./reviewbooks/Users/README.md)
- [Module Books](./reviewbooks/Books/README.md)
- [Module Reviews](./reviewbooks/Reviews/README.md)
- [Module Forum](./reviewbooks/Forum/README.md)
- [Module Favorite](./reviewbooks/Favorite/README.md)

---

##  Database Schema

### Các Bảng Chính

- **Users** - Tài khoản với vai trò
- **PendingRegistrations** - Tạm thời (TTL 30 phút)
- **Books** - Metadata với dual rating
- **BookCaches** - Cache từ Google Books
- **Reviews** - Rating 1-5 sao
- **Forums** - Chủ đề thảo luận
- **Replies** - Phản hồi
- **Favorites** - Quan hệ User-Book

---

##  Cấu Hình Production

### Biến Môi Trường

```bash
# Database
DB_HOST=your-host
DB_PORT=5432
DB_NAME=reviewbooks
DB_USER=username
DB_PASSWORD=password

# JWT
JWT_KEY=your-secret-key-32-chars
JWT_ISSUER=https://api.domain.com
JWT_AUDIENCE=https://domain.com

# URLs
APP_BACKEND_URL=https://api.domain.com
APP_FRONTEND_URL=https://domain.com

# Email
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USERNAME=email@gmail.com
EMAIL_PASSWORD=app-password

# API Keys
GOOGLE_BOOKS_API_KEY=your-key
```

---

##  Deployment

### Azure App Service

```bash
# Publish
dotnet publish -c Release -o ./publish

# Deploy qua Azure CLI
az webapp up --name your-app --resource-group your-rg
```

### Docker

```bash
docker build -t reviewbooks-api .
docker run -d -p 5072:80 reviewbooks-api
```

### Linux Server với Nginx

```nginx
server {
    listen 80;
    server_name api.domain.com;
    location / {
        proxy_pass http://localhost:5072;
        proxy_set_header Host $host;
    }
}
```

---

##  Đóng Góp

1. Fork repository
2. Tạo branch (`git checkout -b feature/TinhNang`)
3. Commit (`git commit -m 'Thêm tính năng'`)
4. Push (`git push origin feature/TinhNang`)
5. Mở Pull Request

---

##  License

MIT License - Xem file [LICENSE](LICENSE)

---

##  Team

**LTWeb-4P** - Backend Development Team

Repository: [github.com/LTWeb-4P/ReviewBooks](https://github.com/LTWeb-4P/ReviewBooks)

---

<div align="center">
  <p>Được xây dựng với  bởi LTWeb-4P Team</p>
  <p> Star trên GitHub nếu bạn thấy hữu ích!</p>
</div>
