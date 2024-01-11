using Microsoft.AspNetCore.Identity;

namespace Project.Dal.Entities
{
    public class User : IdentityUser
    {
        public bool IsDeleted { get; set; }
        public string? RefreshToken { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ProfilePic { get; set; }
        public ICollection<Role> Roles { get; set; } = new List<Role>();

        // add all the lists of the entities linked to the user like this:
        // public List<Entity> Entities { get; set; } = new List<Entity>();
        public bool IsLockedout() => LockoutEnd != null && LockoutEnd > DateTime.UtcNow;
    }
}