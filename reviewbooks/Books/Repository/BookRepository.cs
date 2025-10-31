using ReviewBooks.Books.Models;
using ReviewBooks.Data;
using Microsoft.EntityFrameworkCore;

namespace ReviewBooks.Books.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _db;
        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Book?> GetBookByIdAsync(string id, CancellationToken ct = default)
        {
            return await _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);
        }

        public async Task<BookCache?> GetBookCacheAsync(string bookId, CancellationToken ct = default)
        {
            return await _db.BookCaches.AsNoTracking().FirstOrDefaultAsync(c => c.BookId == bookId, ct);
        }

        public async Task UpsertBookAsync(Book book, CancellationToken ct = default)
        {
            var tracked = await _db.Books.FirstOrDefaultAsync(b => b.Id == book.Id, ct);
            if (tracked is null)
            {
                _db.Books.Add(book);
            }
            else
            {
                // update fields
                tracked.Title = book.Title;
                tracked.Authors = book.Authors;
                tracked.Publisher = book.Publisher;
                tracked.Description = book.Description;
                tracked.Thumbnail = book.Thumbnail;
                tracked.AverageRating = book.AverageRating;
                tracked.RatingsCount = book.RatingsCount;
                tracked.CachedAt = book.CachedAt;
            }
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpsertBookCacheAsync(BookCache cache, CancellationToken ct = default)
        {
            var tracked = await _db.BookCaches.FirstOrDefaultAsync(c => c.BookId == cache.BookId, ct);
            if (tracked is null)
            {
                _db.BookCaches.Add(cache);
            }
            else
            {
                tracked.JsonData = cache.JsonData;
                tracked.CachedAt = cache.CachedAt;
            }
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateBookRatingsAsync(string bookId, CancellationToken ct = default)
        {
            var book = await _db.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == bookId, ct);

            if (book == null) return;

            // Calculate system ratings from user reviews
            if (book.Reviews != null && book.Reviews.Any())
            {
                book.SystemAverageRating = Math.Round(book.Reviews.Average(r => r.Rating), 1);
                book.SystemRatingsCount = book.Reviews.Count;
            }
            else
            {
                book.SystemAverageRating = null;
                book.SystemRatingsCount = 0;
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}
