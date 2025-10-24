using ReviewBooks.Data;
using ReviewBooks.Books.Models;
using ReviewBooks.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace ReviewBooks.Favorite.Repository
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetUserFavoriteBooksAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.FavoriteBooks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.FavoriteBooks ?? new List<Book>();
        }

        public async Task<bool> AddFavoriteAsync(Guid userId, string bookId)
        {
            var user = await _context.Users
                .Include(u => u.FavoriteBooks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            // Check if already favorited
            if (user.FavoriteBooks?.Any(b => b.Id == bookId) == true)
            {
                return false; // Already in favorites
            }

            // Get or create book
            var book = await GetOrCreateBookAsync(bookId);
            if (book == null) return false;

            // Add to favorites
            user.FavoriteBooks ??= new List<Book>();
            user.FavoriteBooks.Add(book);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(Guid userId, string bookId)
        {
            var user = await _context.Users
                .Include(u => u.FavoriteBooks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            var book = user.FavoriteBooks?.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return false;

            user.FavoriteBooks!.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavoriteAsync(Guid userId, string bookId)
        {
            var user = await _context.Users
                .Include(u => u.FavoriteBooks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.FavoriteBooks?.Any(b => b.Id == bookId) ?? false;
        }

        public async Task<Book?> GetBookByIdAsync(string bookId)
        {
            return await _context.Books.FindAsync(bookId);
        }

        private async Task<Book?> GetOrCreateBookAsync(string bookId)
        {
            // Try to find existing book
            var book = await _context.Books.FindAsync(bookId);
            if (book != null) return book;

            return null;
        }
    }
}
