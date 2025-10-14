

namespace BECore.Book.Models
{
    public class BookCaches
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BookId { get; set; } = string.Empty;
        public string JsonData { get; set; } = string.Empty; // raw JSON Google Books
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}