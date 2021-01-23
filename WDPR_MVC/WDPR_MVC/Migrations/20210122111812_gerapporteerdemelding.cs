using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class gerapporteerdemelding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GerapporteerdeMeldingen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MeldingId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GerapporteerdeMeldingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GerapporteerdeMeldingen_Meldingen_MeldingId",
                        column: x => x.MeldingId,
                        principalTable: "Meldingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GerapporteerdeMeldingen_MeldingId",
                table: "GerapporteerdeMeldingen",
                column: "MeldingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GerapporteerdeMeldingen");
        }
    }
}
