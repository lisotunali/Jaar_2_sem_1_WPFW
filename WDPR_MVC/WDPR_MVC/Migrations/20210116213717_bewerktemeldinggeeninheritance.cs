using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class bewerktemeldinggeeninheritance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meldingen_Meldingen_MeldingId",
                table: "Meldingen");

            migrationBuilder.DropIndex(
                name: "IX_Meldingen_MeldingId",
                table: "Meldingen");

            migrationBuilder.DropColumn(
                name: "MeldingId",
                table: "Meldingen");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Meldingen");

            migrationBuilder.CreateTable(
                name: "BewerkteMeldingen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MeldingId = table.Column<int>(nullable: false),
                    Titel = table.Column<string>(nullable: true),
                    Omschrijving = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BewerkteMeldingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BewerkteMeldingen_Meldingen_MeldingId",
                        column: x => x.MeldingId,
                        principalTable: "Meldingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BewerkteMeldingen_MeldingId",
                table: "BewerkteMeldingen",
                column: "MeldingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BewerkteMeldingen");

            migrationBuilder.AddColumn<int>(
                name: "MeldingId",
                table: "Meldingen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Meldingen",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Meldingen_MeldingId",
                table: "Meldingen",
                column: "MeldingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meldingen_Meldingen_MeldingId",
                table: "Meldingen",
                column: "MeldingId",
                principalTable: "Meldingen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
