using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Project.Dal.Entities;
using Project.Dal.Repositories.Interfaces;

namespace Project.Dal.Jwt
{
    public sealed class JwtProvider : IJwtProvider
    {
        private readonly IPermissionRepository _permissionService;
        private readonly JwtOptions _options;
        public JwtProvider(IOptions<JwtOptions> options, IPermissionRepository permissionService)
        {
            _options = options.Value;
            _permissionService = permissionService;
        }

        public async Task<string> GenerateAccessToken(User user)
        {

            // building claims
            var claims = new List<Claim>{
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };

            HashSet<string> permissions = await _permissionService.GetPermissionsAsync(user.Id);
            foreach (string permission in permissions) claims.Add(new Claim(CustomClaims.Permissions, permission));

            // building signingCredentials
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256
            );

            // building the token
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                expires: DateTime.UtcNow.Add(_options.ExpirationAccessToken),
                claims: claims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken(User user)
        {

            // building claims
            var claims = new List<Claim>{
                 new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // building signingCredentials
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256
            );

            // building the token
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                expires: DateTime.UtcNow.Add(_options.ExpirationRefreshToken),
                claims: claims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public DateTime GetExpirationDate(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.ReadToken(jwtToken) is not JwtSecurityToken jsonToken)
            {
                throw new ArgumentException("Token not valid");
            }

            var expireDate = jsonToken.Claims
                .FirstOrDefault(claim => claim.Type == "exp")
                ?.Value;

            if (expireDate == null || !DateTime.TryParse(expireDate, out DateTime result))
            {
                throw new ArgumentException("Retriving expire date from token failed");
            }

            return result;
        }
        public string GetIdFromRequest(HttpRequest request)
        {
            string tokenEncoded = request.Headers[HeaderNames.Authorization].SingleOrDefault()!.Replace("Bearer ", "");
            JwtSecurityToken tokenDecoded = new JwtSecurityTokenHandler().ReadJwtToken(tokenEncoded);

            return tokenDecoded.Claims.First(c => c.Type == "sub").Value;
        }
    }
}