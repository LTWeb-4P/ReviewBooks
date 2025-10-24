using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewBooks.Reviews.Dto
{
    public class ReviewListDto
    {
        public Guid Id { get; set; }
        public string BookId { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin cơ bản
        public string? Username { get; set; }
        public string? BookTitle { get; set; }
    }
    public class ReviewDetailDto
    {
        public Guid Id { get; set; }
        public string BookId { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Thông tin mở rộng
        public string? Username { get; set; }
        public string? BookTitle { get; set; }
    }
    public class AddReviewRequestDto
    {
        [Required]
        public string BookId { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }

    public class UpdateReviewRequestDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}