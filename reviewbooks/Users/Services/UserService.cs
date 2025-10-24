using ReviewBooks.Users.Dto;
using ReviewBooks.Users.Models;
using ReviewBooks.Users.Repository;
using Shared;

namespace ReviewBooks.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PageResult<UserDto>> GetAllUsersAsync(Query query, string currentUserRole)
        {
            // Only Admin can view all users
            if (currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("Only admins can view all users");
            }

            var users = await _userRepository.GetUsersAsync(query);
            return new PageResult<UserDto>
            {
                Items = users.Items.Select(u => MapToDto(u)).ToList(),
                TotalCount = users.TotalCount,
                PageSize = users.PageSize,
                CurrentPage = users.CurrentPage
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id, Guid currentUserId, string currentUserRole)
        {
            // User can only view their own profile, Admin can view any
            if (currentUserRole != "Admin" && id != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only view your own profile");
            }

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            return MapToDto(user);
        }

        public async Task<UserDto?> UpdateUserProfileAsync(Guid id, UpdateUserProfileDto dto, Guid currentUserId, string currentUserRole)
        {
            // User can only update their own profile, Admin can update any
            if (currentUserRole != "Admin" && id != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own profile");
            }

            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null) return null;

            // Update allowed fields
            if (dto.Username != null) existingUser.Username = dto.Username;
            if (dto.FullName != null) existingUser.FullName = dto.FullName;
            if (dto.PhoneNumber != null) existingUser.PhoneNumber = dto.PhoneNumber;
            if (dto.AvatarUrl != null) existingUser.AvatarUrl = dto.AvatarUrl;
            if (dto.Bio != null) existingUser.Bio = dto.Bio;
            if (dto.BirthDate.HasValue) existingUser.BirthDate = dto.BirthDate;
            if (dto.Gender != null) existingUser.Gender = dto.Gender;

            var updatedUser = await _userRepository.UpdateUserAsync(id, existingUser);
            if (updatedUser == null) return null;

            return MapToDto(updatedUser);
        }

        public async Task<UserDto?> AdminUpdateUserAsync(Guid id, AdminUpdateUserDto dto, string currentUserRole)
        {
            // Only Admin can use this method
            if (currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("Only admins can perform this action");
            }

            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null) return null;

            // Update all fields including sensitive ones
            if (dto.Username != null) existingUser.Username = dto.Username;
            if (dto.Email != null) existingUser.Email = dto.Email;
            if (dto.Role != null) existingUser.Role = dto.Role;
            if (dto.FullName != null) existingUser.FullName = dto.FullName;
            if (dto.PhoneNumber != null) existingUser.PhoneNumber = dto.PhoneNumber;
            if (dto.AvatarUrl != null) existingUser.AvatarUrl = dto.AvatarUrl;
            if (dto.Bio != null) existingUser.Bio = dto.Bio;
            if (dto.BirthDate.HasValue) existingUser.BirthDate = dto.BirthDate;
            if (dto.Gender != null) existingUser.Gender = dto.Gender;

            var updatedUser = await _userRepository.UpdateUserAsync(id, existingUser);
            if (updatedUser == null) return null;

            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id, Guid currentUserId, string currentUserRole)
        {
            // User can delete their own account, Admin can delete any
            if (currentUserRole != "Admin" && id != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own account");
            }

            return await _userRepository.DeleteUserAsync(id);
        }

        private static UserDto MapToDto(User user) => new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            CreatedAt = user.CreatedAt
        };
    }
}
