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

            var backendUrl = _configuration["App:BackendUrl"] ?? "http://localhost:8080/swagger/index.html";
            var verifyUrl = $"{backendUrl}/api/auth/verify-email?userId={pendingUser.Id}&token={pendingUser.VerifyToken}";

            var emailBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <title>BooksNest Email Verification</title>
  <style>
    body {{
      font-family: 'Segoe UI', Arial, sans-serif;
      background-color: #f5f7fa;
      margin: 0;
      padding: 0;
      color: #333;
    }}

    .container {{
      max-width: 600px;
      margin: 40px auto;
      background-color: #ffffff;
      border-radius: 10px;
      overflow: hidden;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }}

    .header {{
      background: linear-gradient(90deg, #667eea, #764ba2);
      color: white;
      padding: 30px 20px;
      text-align: center;
    }}

    .header h1 {{
      margin: 0;
      font-size: 26px;
      letter-spacing: 0.5px;
    }}

    .content {{
      padding: 30px 40px;
      line-height: 1.6;
      text-align: center;
    }}

    .content h2 {{
      margin-top: 0;
      font-size: 20px;
      color: #333;
    }}

    .button {{
      display: inline-block;
      margin: 20px 0;
      background: #667eea;
      color: white !important;
      padding: 12px 30px;
      border-radius: 6px;
      text-decoration: none;
      font-weight: bold;
      letter-spacing: 0.5px;
      transition: background 0.2s ease;
    }}

    .button:hover {{
      background: #5a67d8;
    }}

    .footer {{
      background-color: #f1f3f7;
      text-align: center;
      padding: 15px;
      font-size: 13px;
      color: #777;
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
      <p>Please verify your email address to complete your registration.</p>
      <a href='{verifyUrl}' class='button'>VERIFY EMAIL</a>
      <p style='font-size: 13px; color: #777;'>This link will expire in 30 minutes.</p>
    </div>
    <div class='footer'>
      © 2025 BooksNest. All rights reserved.<br>
      This email was sent automatically — please do not reply.
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
