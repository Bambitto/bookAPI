using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRUDApi.Migrations
{
    public partial class seedAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_UserModelId",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "UserModelId",
                table: "Books",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_UserModelId",
                table: "Books",
                newName: "IX_Books_UserId");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password", "Role", "Surname", "Username" },
                values: new object[] { new Guid("8fb64705-b2e0-4e53-965e-46bfd9976372"), "samcollins@email.com", "Sam", "qwertyui123", "admin", "Collins", "samcol" });

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_UserId",
                table: "Books",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_UserId",
                table: "Books");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8fb64705-b2e0-4e53-965e-46bfd9976372"));

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Books",
                newName: "UserModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_UserId",
                table: "Books",
                newName: "IX_Books_UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_UserModelId",
                table: "Books",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
