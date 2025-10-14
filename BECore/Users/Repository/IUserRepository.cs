using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BECore.Users.Models;

namespace BECore.Users.Repository
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();
        public Task<User?> GetUserByIdAsync(Guid id);
        Task<User> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(Guid id, User user);
        Task<User?> DeleteUserAysnc(Guid id);
    }
}