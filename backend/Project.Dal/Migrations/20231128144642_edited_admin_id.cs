using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Dal.Migrations
{
    /// <inheritdoc />
    public partial class edited_admin_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "E5521F4C-C677-4B6E-81E4-E0DCD8A0EA2D");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "IsDeleted", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d", 0, "79484be8-0696-41ed-abcb-c2afb8010b58", "User", "admin@gmail.com", false, false, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEBtWmWPRWhAePW7/CyuQ6NPRF+FCCe73X5PNx7jQeeDEaKnGNBYBnkik3DTP86QgQw==", null, false, null, "UQJUD4BXGAF2JYYBFGLHXSTJ23Y4L5R3", false, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "IsDeleted", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "E5521F4C-C677-4B6E-81E4-E0DCD8A0EA2D", 0, "79484be8-0696-41ed-abcb-c2afb8010b58", "User", "admin@gmail.com", false, false, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEBtWmWPRWhAePW7/CyuQ6NPRF+FCCe73X5PNx7jQeeDEaKnGNBYBnkik3DTP86QgQw==", null, false, null, "UQJUD4BXGAF2JYYBFGLHXSTJ23Y4L5R3", false, "admin" });
        }
    }
}
