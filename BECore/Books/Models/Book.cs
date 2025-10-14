using BECore.Reivews.Models;

namespace BECore.Books.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Authors { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Review>? Reviews { get; set; }
    }
}