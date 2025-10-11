using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BECore.Models;

namespace BECore.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }
        public Role role { get; set; } = Role.User;


    }
    public class RefeshToken
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
    }

    public class addUserRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }
    }
    public class updateUserRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }
    }

}