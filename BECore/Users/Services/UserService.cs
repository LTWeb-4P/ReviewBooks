using BECore.Users.Dto;
using BECore.Users.Models;
using BECore.Users.Repository;
namespace BECore.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUserAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                AvatarUrl = u.AvatarUrl
            });
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task<UserDto> CreateUserAsync(AddUserRequestDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                AvatarUrl = dto.AvatarUrl,
                Password = dto.Password
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            return new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                AvatarUrl = createdUser.AvatarUrl,

            };
        }

        public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserRequestDto dto)
        {
            var userToUpdate = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                AvatarUrl = dto.AvatarUrl,
                Password = dto.Password

            };

            var updatedUser = await _userRepository.UpdateUserAsync(id, userToUpdate);
            if (updatedUser == null) return null;

            return new UserDto
            {
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                AvatarUrl = updatedUser.AvatarUrl,

            };
        }

        public async Task<bool?> DeleteUserAsync(Guid id)
        {
            var deletedUser = await _userRepository.DeleteUserAysnc(id);
            return deletedUser != null;
        }
    }
}
