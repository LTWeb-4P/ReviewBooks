using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BECore.Data;
using BECore.Dto;
using BECore.Interface;
using BECore.Models;
using Microsoft.EntityFrameworkCore;

namespace BECore.Repository
{
    public class UserRepository : IUser
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
            existingUser.Id = user.Id;
            existingUser.Email = user.Email;

            await dbContext.SaveChangesAsync();
            return existingUser;
        }

    }
}