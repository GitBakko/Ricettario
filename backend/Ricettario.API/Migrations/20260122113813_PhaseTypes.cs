using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ricettario.API.Migrations
{
    /// <inheritdoc />
    public partial class PhaseTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhaseType",
                table: "RecipePhases",
                newName: "RecipePhaseTypeId");

            migrationBuilder.CreateTable(
                name: "RecipePhaseTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActiveWork = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipePhaseTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "RecipePhaseTypes",
                columns: new[] { "Id", "Description", "IsActiveWork", "IsSystemDefault", "Name" },
                values: new object[,]
                {
                    { 1, null, true, true, "Impasto" },
                    { 2, null, false, true, "Lievitazione" },
                    { 3, null, true, true, "Cottura" },
                    { 4, null, true, true, "Pre-Impasto" },
                    { 5, null, true, true, "Pieghe" },
                    { 6, null, true, true, "Formatura" },
                    { 7, null, false, true, "Appretto" },
                    { 8, null, false, true, "Autoli" }
                });

            // Adjust existing data: Shift 0-based Enum values to 1-based IDs
            migrationBuilder.Sql("UPDATE RecipePhases SET RecipePhaseTypeId = RecipePhaseTypeId + 1");
            
            // Safety: Ensure all FKs are valid (default to 1 'Impasto' if out of range)
            migrationBuilder.Sql("UPDATE RecipePhases SET RecipePhaseTypeId = 1 WHERE RecipePhaseTypeId NOT IN (SELECT Id FROM RecipePhaseTypes)");

            migrationBuilder.CreateIndex(
                name: "IX_RecipePhases_RecipePhaseTypeId",
                table: "RecipePhases",
                column: "RecipePhaseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipePhases_RecipePhaseTypes_RecipePhaseTypeId",
                table: "RecipePhases",
                column: "RecipePhaseTypeId",
                principalTable: "RecipePhaseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipePhases_RecipePhaseTypes_RecipePhaseTypeId",
                table: "RecipePhases");

            migrationBuilder.DropTable(
                name: "RecipePhaseTypes");

            migrationBuilder.DropIndex(
                name: "IX_RecipePhases_RecipePhaseTypeId",
                table: "RecipePhases");

            migrationBuilder.RenameColumn(
                name: "RecipePhaseTypeId",
                table: "RecipePhases",
                newName: "PhaseType");
        }
    }
}
