using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReviewBooks.Auth.Models
{
    public enum Roles
    {
        Admin, User
    }

    [Table("auth_users")]
    public class AuthUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress]
        [Column("email")]
        public required string Email { get; set; }

        [Required]
        [Column("password")]
        public required string Password { get; set; }

        [Required]
        [Column("username")]
        public required string Username { get; set; }

        [Column("roles")]
        public Roles Roles { get; set; } = Roles.User;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public class RegisterRequestDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string Username { get; set; } = string.Empty;
        public string[] Roles { get; set; } = ["User"];
    }

    public class LoginRequestDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string JwtToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}