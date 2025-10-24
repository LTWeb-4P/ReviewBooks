using ReviewBooks.Users.Models;
using Shared;

namespace ReviewBooks.Users.Repository
{
    public interface IUserRepository
    {
        Task<PageResult<User>> GetUsersAsync(Query query);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(Guid id, User user);
        Task<bool> DeleteUserAsync(Guid id);
    }
}