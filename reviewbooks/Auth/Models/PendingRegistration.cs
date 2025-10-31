using System.ComponentModel.DataAnnotations;

namespace ReviewBooks.Auth.Models
{
    /// <summary>
    /// Lưu thông tin đăng ký tạm thời - chưa được verify
    /// Sẽ bị xóa sau khi verify thành công hoặc hết hạn
    /// </summary>
    public class PendingRegistration
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string VerifyToken { get; set; } = string.Empty;

        public DateTime TokenExpires { get; set; } = DateTime.UtcNow.AddMinutes(30);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
