using ReviewBooks.Auth.Dto;
using ReviewBooks.Auth.Repository;
using ReviewBooks.Users.Models;
using ReviewBooks.Users.Repository;
using ReviewBooks.Auth.Models;
using ReviewBooks.Data;
using Microsoft.EntityFrameworkCore;

namespace ReviewBooks.Auth.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto?> VerifyEmailAsync(Guid tempUserId, string token);
        Task<bool> ResendVerificationEmailAsync(string email);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthService(
            IUserRepository userRepository,
            ITokenRepository tokenRepository,
            EmailService emailService,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> RegisterAsync(RegisterRequestDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already registered");
            }

            var existingPending = await _context.PendingRegistrations
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (existingPending != null)
            {
                _context.PendingRegistrations.Remove(existingPending);
                await _context.SaveChangesAsync();
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var verifyToken = Guid.NewGuid().ToString("N");
            var tokenExpires = DateTime.UtcNow.AddMinutes(30);

            var pendingUser = new PendingRegistration
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                Username = dto.Username,
                VerifyToken = verifyToken,
                TokenExpires = tokenExpires,
                CreatedAt = DateTime.UtcNow
            };

            _context.PendingRegistrations.Add(pendingUser);
            await _context.SaveChangesAsync();

            await SendVerificationEmailAsync(pendingUser);

            return "Registration email sent. Please check your email within 30 minutes.";
        }

        private async Task SendVerificationEmailAsync(PendingRegistration pendingUser)
        {
            Console.WriteLine($"[SendVerificationEmailAsync] pendingUser.Email = '{pendingUser.Email}'");

            var backendUrl = _configuration["App:BackendUrl"] ?? "http://localhost:8080";
            var verifyUrl = $"{backendUrl}/api/auth/verify-email?userId={pendingUser.Id}&token={pendingUser.VerifyToken}";

            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(90deg, #667eea, #764ba2);
            color: white;
            padding: 25px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 30px 25px;
            color: #333333;
            line-height: 1.6;
        }}
        .content h2 {{
            color: #444444;
        }}
        .button {{
            display: inline-block;
            background-color: #667eea;
            color: #ffffff !important;
            padding: 12px 30px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            margin-top: 15px;
        }}
        .footer {{
            text-align: center;
            padding: 15px;
            font-size: 13px;
            color: #999999;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to BooksNest!</h1>
        </div>
        <div class='content'>
            <h2>Hi {pendingUser.Username},</h2>
            <p>Thank you for joining BooksNest! Please verify your email address by clicking the button below:</p>
            <p><a href='{verifyUrl}' class='button'>VERIFY EMAIL</a></p>
            <p>This link will expire in <strong>30 minutes</strong>.</p>
        </div>
        <div class='footer'>
            &copy; {DateTime.Now.Year} BooksNest. All rights reserved.
        </div>
    </div>
</body>
</html>";

            await _emailService.SendEmailAsync(pendingUser.Email, "Verify Your Email", emailBody);
        }

        public async Task<LoginResponseDto?> VerifyEmailAsync(Guid tempUserId, string token)
        {
            var pending = await _context.PendingRegistrations.FirstOrDefaultAsync(p => p.Id == tempUserId);
            if (pending == null || pending.VerifyToken != token) return null;

            if (pending.TokenExpires < DateTime.UtcNow)
            {
                _context.PendingRegistrations.Remove(pending);
                await _context.SaveChangesAsync();
                return null;
            }

            // Kiểm tra xem user đã được tạo chưa (tránh verify nhiều lần)
            var existingUser = await _userRepository.GetUserByEmailAsync(pending.Email);
            if (existingUser != null)
            {
                // User đã được verify rồi, xóa pending và trả về null
                _context.PendingRegistrations.Remove(pending);
                await _context.SaveChangesAsync();
                return null;
            }

            var user = new User
            {
                Email = pending.Email,
                PasswordHash = pending.PasswordHash,
                Username = pending.Username,
                Role = "User",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateUserAsync(user);
            _context.PendingRegistrations.Remove(pending);
            await _context.SaveChangesAsync();

            var jwtToken = _tokenRepository.GenerateJwtToken(
                createdUser.Id, createdUser.Email, createdUser.Username, createdUser.Role);

            return new LoginResponseDto
            {
                Token = jwtToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new UserInfoDto
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Username = createdUser.Username,
                    Role = createdUser.Role
                }
            };
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            var pending = await _context.PendingRegistrations.FirstOrDefaultAsync(p => p.Email == email);
            if (pending == null) return false;

            pending.VerifyToken = Guid.NewGuid().ToString("N");
            pending.TokenExpires = DateTime.UtcNow.AddMinutes(30);
            _context.PendingRegistrations.Update(pending);
            await _context.SaveChangesAsync();
            await SendVerificationEmailAsync(pending);
            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            if (!user.EmailConfirmed)
            {
                throw new InvalidOperationException("Please verify your email");
            }

            var token = _tokenRepository.GenerateJwtToken(user.Id, user.Email, user.Username, user.Role);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }
    }
}
