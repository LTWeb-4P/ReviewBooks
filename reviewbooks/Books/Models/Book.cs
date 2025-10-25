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
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }

        public string? Content { get; set; }

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        public ICollection<User>? FavoritedByUsers { get; set; } = new List<User>();

    }
}