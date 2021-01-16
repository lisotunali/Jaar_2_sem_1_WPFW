using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class lalala : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Omschrijving",
                table: "BewerkteMeldingen");

            migrationBuilder.AddColumn<string>(
                name: "Beschrijving",
                table: "BewerkteMeldingen",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Beschrijving",
                table: "BewerkteMeldingen");

            migrationBuilder.AddColumn<string>(
                name: "Omschrijving",
                table: "BewerkteMeldingen",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
