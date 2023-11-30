namespace Project.Dal.Jwt
{
    public class JwtOptions
    {
        public string Issuer { get; init; } = "http://localhost:5000";
        public string Audience { get; init; } = "http://localhost:4200";
        public string SecretKey { get; init; } = "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr";
        public TimeSpan ExpirationAccessToken { get; init; } = TimeSpan.FromHours(1);
        public TimeSpan ExpirationRefreshToken { get; init; } = TimeSpan.FromDays(7);
    }
}