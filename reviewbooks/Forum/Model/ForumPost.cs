using System.ComponentModel.DataAnnotations;
using ReviewBooks.Users.Models;

namespace ReviewBooks.Forum.Model
{
    public class ForumPost
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int ViewCount { get; set; } = 0;
        public bool IsPinned { get; set; } = false;
        public bool IsLocked { get; set; } = false;

        // Navigation properties
        public User? User { get; set; }
        public ICollection<ForumComment>? Comments { get; set; }
    }

    public class ForumComment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PostId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ForumPost? Post { get; set; }
        public User? User { get; set; }
    }
}
