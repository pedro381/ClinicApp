using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Client.Utils
{
    public static class JwtParser
    {
        public static string? GetRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == "roles")?.Value;
        }
    }
}
