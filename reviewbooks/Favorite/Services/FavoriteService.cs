using ReviewBooks.Favorite.Dto;
using ReviewBooks.Favorite.Repository;
using ReviewBooks.Books.Services;

namespace ReviewBooks.Favorite.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _repository;
        private readonly IBookService _bookService;

        public FavoriteService(IFavoriteRepository repository, IBookService bookService)
        {
            _repository = repository;
            _bookService = bookService;
        }

        public async Task<IEnumerable<FavoriteBookDto>> GetUserFavoritesAsync(Guid userId)
        {
            var books = await _repository.GetUserFavoriteBooksAsync(userId);

            return books.Select(b => new FavoriteBookDto
            {
                BookId = b.Id,
                Title = b.Title,
                Authors = b.Authors,
                Publisher = b.Publisher,
                Description = b.Description,
                Thumbnail = b.Thumbnail,
                AverageRating = b.AverageRating,
                RatingsCount = b.RatingsCount,
                AddedAt = b.CachedAt
            });
        }

        public async Task<bool> AddFavoriteAsync(Guid userId, string bookId)
        {
            // Check if book exists in DB, if not fetch from Google Books API
            var existingBook = await _repository.GetBookByIdAsync(bookId);
            if (existingBook == null)
            {
                // Fetch from Google Books API and save to DB
                var bookDto = await _bookService.GetByIdAsync(bookId);
                if (bookDto == null)
                {
                    throw new InvalidOperationException($"Book with id {bookId} not found");
                }
            }

            return await _repository.AddFavoriteAsync(userId, bookId);
        }

        public async Task<bool> RemoveFavoriteAsync(Guid userId, string bookId)
        {
            return await _repository.RemoveFavoriteAsync(userId, bookId);
        }

        public async Task<bool> IsFavoriteAsync(Guid userId, string bookId)
        {
            return await _repository.IsFavoriteAsync(userId, bookId);
        }
    }
}
