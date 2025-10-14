using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BECore.Users.Dto;


namespace BECore.Users.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUserAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto> CreateUserAsync(AddUserRequestDto dto);
        Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserRequestDto dto);
        Task<bool?> DeleteUserAsync(Guid id);
    }

}