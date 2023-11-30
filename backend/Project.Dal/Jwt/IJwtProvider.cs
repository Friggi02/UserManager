using Microsoft.AspNetCore.Http;
using Project.Dal.Entities;

namespace Project.Dal.Jwt
{
    public interface IJwtProvider
    {
        public Task<string> GenerateAccessToken(User user);
        public string GenerateRefreshToken(User user);
        public DateTime GetExpirationDate(string jwtToken);
        public string GetIdFromRequest(HttpRequest request);
    }
}