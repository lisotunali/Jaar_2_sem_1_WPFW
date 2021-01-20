using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class AddKnownIpsDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnownIp_AspNetUsers_ApplicationUserId",
                table: "KnownIp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KnownIp",
                table: "KnownIp");

            migrationBuilder.RenameTable(
                name: "KnownIp",
                newName: "KnownIps");

            migrationBuilder.RenameIndex(
                name: "IX_KnownIp_ApplicationUserId",
                table: "KnownIps",
                newName: "IX_KnownIps_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KnownIps",
                table: "KnownIps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnownIps_AspNetUsers_ApplicationUserId",
                table: "KnownIps",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnownIps_AspNetUsers_ApplicationUserId",
                table: "KnownIps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KnownIps",
                table: "KnownIps");

            migrationBuilder.RenameTable(
                name: "KnownIps",
                newName: "KnownIp");

            migrationBuilder.RenameIndex(
                name: "IX_KnownIps_ApplicationUserId",
                table: "KnownIp",
                newName: "IX_KnownIp_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KnownIp",
                table: "KnownIp",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnownIp_AspNetUsers_ApplicationUserId",
                table: "KnownIp",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
