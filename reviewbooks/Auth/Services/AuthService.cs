using ReviewBooks.Auth.Dto;
using ReviewBooks.Auth.Repository;
using ReviewBooks.Users.Models;
using ReviewBooks.Users.Repository;

namespace ReviewBooks.Auth.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        public AuthService(IUserRepository userRepository, ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                Username = dto.Username,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            var token = _tokenRepository.GenerateJwtToken(
                createdUser.Id,
                createdUser.Email,
                createdUser.Username,
                createdUser.Role
            );

            var expiresAt = DateTime.UtcNow.AddHours(24);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserInfoDto
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Username = createdUser.Username,
                    Role = createdUser.Role
                }
            };
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return null;
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }

            var token = _tokenRepository.GenerateJwtToken(
                user.Id,
                user.Email,
                user.Username,
                user.Role
            );

            var expiresAt = DateTime.UtcNow.AddHours(24);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
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
