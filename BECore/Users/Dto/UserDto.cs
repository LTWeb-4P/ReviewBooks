namespace BECore.Users.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }


    }
    // public class RefeshToken
    // {
    //     public Guid UserId { get; set; }
    //     public string Token { get; set; }
    //     public DateTime ExpiresAt { get; set; }
    //     public bool IsRevoked { get; set; } = false;
    // }

    public class AddUserRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string Password { get; set; } 

    }
    public class UpdateUserRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public string? AvatarUrl { get; set; }
    }

}