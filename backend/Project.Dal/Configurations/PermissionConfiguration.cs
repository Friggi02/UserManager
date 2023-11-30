using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Dal.Entities;
using Project.Dal.Permit;

namespace Project.Dal.Configurations
{
    internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");
            builder.HasKey(x => x.Id);
            IEnumerable<Permission> permissions = Enum
                .GetValues<Permissions>()
                .Select(p => new Permission
                {
                    Id = (int)p,
                    Name = p.ToString()
                });
            builder.HasData(permissions);
        }
    }
}