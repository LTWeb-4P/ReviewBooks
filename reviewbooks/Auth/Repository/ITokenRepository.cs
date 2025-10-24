using System.Security.Claims;

namespace ReviewBooks.Auth.Repository
{
    public interface ITokenRepository
    {
        string GenerateJwtToken(Guid userId, string email, string username, string role);
    }
}
