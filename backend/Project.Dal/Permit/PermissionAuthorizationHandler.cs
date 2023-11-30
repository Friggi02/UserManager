using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Project.Dal.Jwt;

namespace Project.Dal.Permit
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            var user = context.User;
            var claims = user.Claims;
            // Log the claims
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }
            var permissionClaims = claims.Where(x => x.Type == CustomClaims.Permissions);
            var permissionClaimsValues = permissionClaims.Select(x => x.Value);
            var permissions = permissionClaimsValues.ToHashSet();

            //HashSet<string> permissions = context.User.Claims.Where(x => x.Type == CustomClaims.Permissions).Select(x => x.Value).ToHashSet();

            if (permissions.Contains(requirement.Permission)) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}