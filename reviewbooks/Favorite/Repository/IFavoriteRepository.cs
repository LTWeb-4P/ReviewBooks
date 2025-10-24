using ReviewBooks.Books.Models;

namespace ReviewBooks.Favorite.Repository
{
    public interface IFavoriteRepository
    {
        Task<IEnumerable<Book>> GetUserFavoriteBooksAsync(Guid userId);
        Task<bool> AddFavoriteAsync(Guid userId, string bookId);
        Task<bool> RemoveFavoriteAsync(Guid userId, string bookId);
        Task<bool> IsFavoriteAsync(Guid userId, string bookId);
        Task<Book?> GetBookByIdAsync(string bookId);
    }
}
