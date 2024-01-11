using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Dal.Migrations
{
    /// <inheritdoc />
    public partial class added_user_infos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePic",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d",
                columns: new[] { "Email", "Name", "NormalizedEmail", "NormalizedUserName", "ProfilePic", "Surname", "UserName" },
                values: new object[] { "fritz@gmail.com", "Andrea", "FRITZ@GMAIL.COM", "FRITZ", "https://avatars.githubusercontent.com/u/71127905?v=4", "Frigerio", "fritz" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePic",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5521f4c-c677-4b6e-81e4-e0dcd8a0ea2d",
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "admin@gmail.com", "ADMIN@GMAIL.COM", "ADMIN", "admin" });
        }
    }
}
