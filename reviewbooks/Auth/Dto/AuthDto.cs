using System.ComponentModel.DataAnnotations;

namespace ReviewBooks.Auth.Dto
{
    public class RegisterRequestDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, MinLength(6)]
        public required string Password { get; set; }

        [Required, MinLength(3), MaxLength(50)]
        public required string Username { get; set; }
    }

    public class LoginRequestDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = null!;
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class ResendVerificationDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }
    }
}
