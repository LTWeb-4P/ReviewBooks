using ReviewBooks.Reviews.Models;
using ReviewBooks.Users.Models;
namespace ReviewBooks.Books.Models
{
    public class Book
    {
        // Google Books volumeId (string) will be used as primary key
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Authors { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? ISBN { get; set; }
        public string? Categories { get; set; }
        public double? Price { get; set; }
        public string? Url_Buy { get; set; }

        // Google Books ratings (external)
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }

        // System ratings (from user reviews)
        public double? SystemAverageRating { get; set; }
        public int SystemRatingsCount { get; set; } = 0;

        public string? Content { get; set; }

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        public ICollection<User>? FavoritedByUsers { get; set; } = new List<User>();

    }
}