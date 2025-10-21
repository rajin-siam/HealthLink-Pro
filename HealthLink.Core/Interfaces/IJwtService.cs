using HealthLink.Core.Entities;
using System.Security.Claims;

namespace HealthLink.Core.Interfaces
{
    public interface IJwtService
    {

        public string GenerateAccessToken(User user, IList<string> roles);

        public string GenerateRefreshToken();
        public ClaimsPrincipal ValidateToken(string token);

        public Guid? GetUserIdFromToken(string token);

        public DateTime? GetTokenExpirationDate(string token);
    }
}
