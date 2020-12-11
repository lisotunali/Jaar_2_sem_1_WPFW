using Microsoft.EntityFrameworkCore.Migrations;

namespace WDPR_MVC.Migrations
{
    public partial class DbsetCategorieen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meldingen_Categorie_CategorieId",
                table: "Meldingen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorie",
                table: "Categorie");

            migrationBuilder.RenameTable(
                name: "Categorie",
                newName: "Categorieen");

            migrationBuilder.RenameIndex(
                name: "IX_Categorie_Naam",
                table: "Categorieen",
                newName: "IX_Categorieen_Naam");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorieen",
                table: "Categorieen",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meldingen_Categorieen_CategorieId",
                table: "Meldingen",
                column: "CategorieId",
                principalTable: "Categorieen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meldingen_Categorieen_CategorieId",
                table: "Meldingen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorieen",
                table: "Categorieen");

            migrationBuilder.RenameTable(
                name: "Categorieen",
                newName: "Categorie");

            migrationBuilder.RenameIndex(
                name: "IX_Categorieen_Naam",
                table: "Categorie",
                newName: "IX_Categorie_Naam");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorie",
                table: "Categorie",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meldingen_Categorie_CategorieId",
                table: "Meldingen",
                column: "CategorieId",
                principalTable: "Categorie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
