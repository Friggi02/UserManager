namespace Project.Dal.Entities
{
    public sealed class Role : Enumeration<Role>
    {
        public static readonly Role Registered = new(1, "Registered");
        public static readonly Role Admin = new(2, "Admin");
        public Role(int id, string name)
            : base(id, name)
        {
        }

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}