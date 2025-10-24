using System.ComponentModel.DataAnnotations;

namespace ReviewBooks.Users.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserProfileDto
    {
        [MinLength(3), MaxLength(50)]
        public string? Username { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Url]
        public string? AvatarUrl { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public DateTime? BirthDate { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }
    }

    // Only Admin can use this
    public class AdminUpdateUserDto : UpdateUserProfileDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Role { get; set; } // User or Admin
    }
}
