using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Dal.Entities;

namespace Project.Dal.Configurations
{
    internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(x => new { x.RoleId, x.UserId });

            builder.HasData(
              Create(Role.Admin, "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d"));
        }

        private static UserRole Create(Role role, User user)
        {
            return new UserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            };
        }

        private static UserRole Create(Role role, string userId)
        {
            return new UserRole
            {
                RoleId = role.Id,
                UserId = userId
            };
        }
    }
}
