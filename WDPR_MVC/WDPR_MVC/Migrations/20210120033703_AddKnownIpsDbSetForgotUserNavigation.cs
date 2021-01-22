using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class AddKnownIpsDbSetForgotUserNavigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnownIps_AspNetUsers_ApplicationUserId",
                table: "KnownIps");

            migrationBuilder.DropIndex(
                name: "IX_KnownIps_ApplicationUserId",
                table: "KnownIps");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "KnownIps");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "KnownIps",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_KnownIps_UserId",
                table: "KnownIps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_KnownIps_AspNetUsers_UserId",
                table: "KnownIps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnownIps_AspNetUsers_UserId",
                table: "KnownIps");

            migrationBuilder.DropIndex(
                name: "IX_KnownIps_UserId",
                table: "KnownIps");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "KnownIps");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "KnownIps",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnownIps_ApplicationUserId",
                table: "KnownIps",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_KnownIps_AspNetUsers_ApplicationUserId",
                table: "KnownIps",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
