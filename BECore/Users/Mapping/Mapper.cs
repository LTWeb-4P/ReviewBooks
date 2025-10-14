using AutoMapper;
using BECore.Users.Dto;
using BECore.Users.Models;

namespace BECore.Users.Automapping
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserDto, AddUserRequestDto>().ReverseMap();
            CreateMap<UserDto, UpdateUserRequestDto>().ReverseMap();
        }
    }

}