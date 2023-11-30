using Microsoft.AspNetCore.Authorization;

namespace Project.Dal.Permit
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
        public string Permission { get; }
    }
}