using ReviewBooks.Favorite.Dto;

namespace ReviewBooks.Favorite.Services
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteBookDto>> GetUserFavoritesAsync(Guid userId);
        Task<bool> AddFavoriteAsync(Guid userId, string bookId);
        Task<bool> RemoveFavoriteAsync(Guid userId, string bookId);
        Task<bool> IsFavoriteAsync(Guid userId, string bookId);
    }
}
