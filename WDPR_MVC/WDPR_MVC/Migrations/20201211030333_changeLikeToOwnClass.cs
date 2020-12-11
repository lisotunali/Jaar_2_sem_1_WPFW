using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class changeLikeToOwnClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AantalLikes",
                table: "Meldingen");

            migrationBuilder.CreateTable(
                name: "MeldingLike",
                columns: table => new
                {
                    MeldingId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeldingLike", x => new { x.MeldingId, x.UserId });
                    table.ForeignKey(
                        name: "FK_MeldingLike_Meldingen_MeldingId",
                        column: x => x.MeldingId,
                        principalTable: "Meldingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeldingLike_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeldingLike_UserId",
                table: "MeldingLike",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeldingLike");

            migrationBuilder.AddColumn<int>(
                name: "AantalLikes",
                table: "Meldingen",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
