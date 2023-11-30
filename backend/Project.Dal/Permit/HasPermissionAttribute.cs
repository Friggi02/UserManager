using Microsoft.AspNetCore.Authorization;

namespace Project.Dal.Permit
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permissions permission) : base(policy: permission.ToString())
        {

        }
    }
}