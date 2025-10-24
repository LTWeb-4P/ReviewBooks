using ReviewBooks.Data;
using ReviewBooks.Users.Models;
using Microsoft.EntityFrameworkCore;
using Shared;
using ReviewBooks.Books.Models;

namespace ReviewBooks.Users.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PageResult<User>> GetUsersAsync(Query query)

        {
            var q = dbContext.Users.AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(query.search))
            {
                q = q.Where(u => u.Username.Contains(query.search) || u.Email.Contains(query.search));
            }

            // Sorting
            if (!string.IsNullOrEmpty(query.sortBy))
            {
                q = query.sortBy.ToLower() switch
                {
                    "username" => query.isDescending ? q.OrderByDescending(x => x.Username) : q.OrderBy(x => x.Username),
                    "email" => query.isDescending ? q.OrderByDescending(x => x.Email) : q.OrderBy(x => x.Email),
                    _ => q.OrderBy(x => x.CreatedAt),
                };
            }

            // Pagination
            var totalItems = await q.CountAsync();
            var users = await q.Skip((query.pageNumber - 1) * query.pageSize).Take(query.pageSize).ToListAsync();

            return new PageResult<User>
            {
                Items = users,
                TotalCount = totalItems,
                PageSize = query.pageSize,
                CurrentPage = query.pageNumber
            };
        }

        // public async Task<User?> FavoriteBook(string email, string bookId)
        // {
        //     var user = await dbContext.Users.Include(u => u.FavoriteBooks).FirstOrDefaultAsync(u => u.Email == email);
        //     if (user == null) return null;

        //     var book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId);
        //     if (book == null) return null;

        //     if (!user.FavoriteBooks.Contains(book))
        //     {
        //         user.FavoriteBooks.Add(book);
        //         await dbContext.SaveChangesAsync();
        //     }

        //     return user;
        // }



        public async Task<User> CreateUserAsync(User user)
        {
            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (existingUser == null) return false;
            dbContext.Remove(existingUser);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> UpdateUserAsync(Guid id, User user)
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (existingUser == null) return null;

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FullName = user.FullName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.AvatarUrl = user.AvatarUrl;
            existingUser.Bio = user.Bio;
            existingUser.BirthDate = user.BirthDate;
            existingUser.Gender = user.Gender;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return existingUser;
        }
    }
}