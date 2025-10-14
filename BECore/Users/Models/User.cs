using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BECore.Reviews.Models;

namespace BECore.Users.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } 
        public string Email { get; set; } 
        public string Password { get; set; } 
        public Role role { get; set; } = Role.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? AvatarUrl { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }

    public enum Role
    {
        Admin, User
    }
    // public class RefeshToken
    // {
    //     public Guid Id { get; set; } = Guid.NewGuid();
    //     public Guid UserId { get; set; }
    //     public string Token { get; set; } 
    //     public DateTime ExpiresAt { get; set; }
    //     public bool IsRevoked { get; set; } = false;
    // }
}