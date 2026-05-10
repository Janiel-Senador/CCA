using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCA.Migrations
{
    /// <inheritdoc />
    public partial class AddCommissionRequestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Artists_ArtistId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Review");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ArtistId",
                table: "Review",
                newName: "IX_Review_ArtistId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CommissionRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CommissionRequests",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ArtistResponse",
                table: "CommissionRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "CommissionRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RespondedAt",
                table: "CommissionRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserRole",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Artists",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionRequests_ArtistId",
                table: "CommissionRequests",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionRequests_CustomerId",
                table: "CommissionRequests",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionRequests_Artists_ArtistId",
                table: "CommissionRequests",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionRequests_AspNetUsers_CustomerId",
                table: "CommissionRequests",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Artists_ArtistId",
                table: "Review",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionRequests_Artists_ArtistId",
                table: "CommissionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionRequests_AspNetUsers_CustomerId",
                table: "CommissionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Artists_ArtistId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_CommissionRequests_ArtistId",
                table: "CommissionRequests");

            migrationBuilder.DropIndex(
                name: "IX_CommissionRequests_CustomerId",
                table: "CommissionRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ArtistResponse",
                table: "CommissionRequests");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CommissionRequests");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                table: "CommissionRequests");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "Reviews");

            migrationBuilder.RenameIndex(
                name: "IX_Review_ArtistId",
                table: "Reviews",
                newName: "IX_Reviews_ArtistId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CommissionRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CommissionRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "UserRole",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Artists",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Artists_ArtistId",
                table: "Reviews",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
