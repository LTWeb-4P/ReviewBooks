using ReviewBooks.Books.Models;

namespace ReviewBooks.Books.Repository
{
    public interface IBookRepository
    {
        Task<Book?> GetBookByIdAsync(string id, CancellationToken ct = default);
        Task<BookCache?> GetBookCacheAsync(string bookId, CancellationToken ct = default);
        Task UpsertBookAsync(Book book, CancellationToken ct = default);
        Task UpsertBookCacheAsync(BookCache cache, CancellationToken ct = default);
        Task UpdateBookRatingsAsync(string bookId, CancellationToken ct = default);
    }
}
