using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Dal.Migrations
{
    /// <inheritdoc />
    public partial class edited_admin_id_fr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRole",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, "E5521F4C-C677-4B6E-81E4-E0DCD8A0EA2D" });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 2, "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRole",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d" });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 2, "E5521F4C-C677-4B6E-81E4-E0DCD8A0EA2D" });
        }
    }
}
