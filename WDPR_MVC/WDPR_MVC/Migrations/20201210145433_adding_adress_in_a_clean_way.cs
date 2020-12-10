using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class adding_adress_in_a_clean_way : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdresId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Adres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Straatnaam = table.Column<string>(nullable: false),
                    Huisnummer = table.Column<int>(nullable: false),
                    Toevoeging = table.Column<string>(nullable: true),
                    Postcode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adres", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AdresId",
                table: "AspNetUsers",
                column: "AdresId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Adres_AdresId",
                table: "AspNetUsers",
                column: "AdresId",
                principalTable: "Adres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Adres_AdresId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Adres");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AdresId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdresId",
                table: "AspNetUsers");
        }
    }
}
