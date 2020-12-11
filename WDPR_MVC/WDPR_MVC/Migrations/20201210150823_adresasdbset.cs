using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class adresasdbset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Adres_AdresId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "AdresId",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Adres_AdresId",
                table: "AspNetUsers",
                column: "AdresId",
                principalTable: "Adres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Adres_AdresId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "AdresId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Adres_AdresId",
                table: "AspNetUsers",
                column: "AdresId",
                principalTable: "Adres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
