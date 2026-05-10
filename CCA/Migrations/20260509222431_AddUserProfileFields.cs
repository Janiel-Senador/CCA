using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCA.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailConfirmedAt",
                table: "AspNetUsers",
                newName: "VerifiedAt");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaHandle",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SocialMediaHandle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "VerifiedAt",
                table: "AspNetUsers",
                newName: "EmailConfirmedAt");
        }
    }
}
