using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class DbsetMeldingen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Melding_MeldingId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Melding_AspNetUsers_AuteurId",
                table: "Melding");

            migrationBuilder.DropForeignKey(
                name: "FK_Melding_Categorie_CategorieId",
                table: "Melding");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Melding_MeldingId",
                table: "Report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Melding",
                table: "Melding");

            migrationBuilder.RenameTable(
                name: "Melding",
                newName: "Meldingen");

            migrationBuilder.RenameIndex(
                name: "IX_Melding_CategorieId",
                table: "Meldingen",
                newName: "IX_Meldingen_CategorieId");

            migrationBuilder.RenameIndex(
                name: "IX_Melding_AuteurId",
                table: "Meldingen",
                newName: "IX_Meldingen_AuteurId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meldingen",
                table: "Meldingen",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Meldingen_MeldingId",
                table: "Comment",
                column: "MeldingId",
                principalTable: "Meldingen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meldingen_AspNetUsers_AuteurId",
                table: "Meldingen",
                column: "AuteurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meldingen_Categorie_CategorieId",
                table: "Meldingen",
                column: "CategorieId",
                principalTable: "Categorie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Meldingen_MeldingId",
                table: "Report",
                column: "MeldingId",
                principalTable: "Meldingen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Meldingen_MeldingId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Meldingen_AspNetUsers_AuteurId",
                table: "Meldingen");

            migrationBuilder.DropForeignKey(
                name: "FK_Meldingen_Categorie_CategorieId",
                table: "Meldingen");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Meldingen_MeldingId",
                table: "Report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meldingen",
                table: "Meldingen");

            migrationBuilder.RenameTable(
                name: "Meldingen",
                newName: "Melding");

            migrationBuilder.RenameIndex(
                name: "IX_Meldingen_CategorieId",
                table: "Melding",
                newName: "IX_Melding_CategorieId");

            migrationBuilder.RenameIndex(
                name: "IX_Meldingen_AuteurId",
                table: "Melding",
                newName: "IX_Melding_AuteurId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Melding",
                table: "Melding",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Melding_MeldingId",
                table: "Comment",
                column: "MeldingId",
                principalTable: "Melding",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Melding_AspNetUsers_AuteurId",
                table: "Melding",
                column: "AuteurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Melding_Categorie_CategorieId",
                table: "Melding",
                column: "CategorieId",
                principalTable: "Categorie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Melding_MeldingId",
                table: "Report",
                column: "MeldingId",
                principalTable: "Melding",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
