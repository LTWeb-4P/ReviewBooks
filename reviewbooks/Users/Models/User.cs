using System.ComponentModel.DataAnnotations;
using ReviewBooks.Reviews.Models;
using ReviewBooks.Books.Models;

namespace ReviewBooks.Users.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "User"; // User or Admin

        // Profile fields
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

        // Email verification
        public bool EmailConfirmed { get; set; } = false;
        public string? EmailVerifyToken { get; set; }
        public DateTime? EmailVerifyTokenExpires { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Book>? FavoriteBooks { get; set; }
    }
}
