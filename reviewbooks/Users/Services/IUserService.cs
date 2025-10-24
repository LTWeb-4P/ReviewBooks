using ReviewBooks.Users.Dto;
using Shared;

namespace ReviewBooks.Users.Services
{
    public interface IUserService
    {
        Task<PageResult<UserDto>> GetAllUsersAsync(Query query, string currentUserRole);
        Task<UserDto?> GetUserByIdAsync(Guid id, Guid currentUserId, string currentUserRole);
        Task<UserDto?> UpdateUserProfileAsync(Guid id, UpdateUserProfileDto dto, Guid currentUserId, string currentUserRole);
        Task<UserDto?> AdminUpdateUserAsync(Guid id, AdminUpdateUserDto dto, string currentUserRole);
        Task<bool> DeleteUserAsync(Guid id, Guid currentUserId, string currentUserRole);
    }
}
