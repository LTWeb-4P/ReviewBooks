using BECore.Data;
using BECore.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace BECore.Users.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteUserAysnc(Guid id)
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (existingUser == null) return null;
            dbContext.Remove(existingUser);
            await dbContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<User?> UpdateUserAsync(Guid id, User user)
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (existingUser == null) return null;

            existingUser.Username = user.Username;
            existingUser.AvatarUrl = user.AvatarUrl;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;

            await dbContext.SaveChangesAsync();
            return existingUser;
        }

    }
}