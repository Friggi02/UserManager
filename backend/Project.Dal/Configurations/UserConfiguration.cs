using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.OData.ModelBuilder;
using Project.Dal.Entities;

namespace Project.Dal.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public User BASE_ADMIN = new()
        {
            Id = "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d",
            UserName = "fritz",
            Email = "fritz@gmail.com",
            Name = "Andrea",
            Surname = "Frigerio",
            ProfilePic = "https://avatars.githubusercontent.com/u/71127905?v=4",
            NormalizedEmail = "FRITZ@GMAIL.COM",
            NormalizedUserName = "FRITZ",
            EmailConfirmed = false,
            PasswordHash = "AQAAAAIAAYagAAAAEBtWmWPRWhAePW7/CyuQ6NPRF+FCCe73X5PNx7jQeeDEaKnGNBYBnkik3DTP86QgQw==",
            SecurityStamp = "UQJUD4BXGAF2JYYBFGLHXSTJ23Y4L5R3",
            ConcurrencyStamp = "79484be8-0696-41ed-abcb-c2afb8010b58",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            IsDeleted = false,
            AccessFailedCount = 0,
            LockoutEnd = null,
            PhoneNumber = null
        };
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(x => x.Roles).WithMany().UsingEntity<UserRole>();
            builder.HasData(BASE_ADMIN);
        }
    }
}