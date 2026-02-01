using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ricettario.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArchiveIngredientId",
                table: "Ingredients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IngredientArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomPropertiesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystemDefault = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientArchives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientConversions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromIngredientId = table.Column<int>(type: "int", nullable: false),
                    ToIngredientId = table.Column<int>(type: "int", nullable: false),
                    ConversionRatio = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientConversions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientConversions_IngredientArchives_FromIngredientId",
                        column: x => x.FromIngredientId,
                        principalTable: "IngredientArchives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientConversions_IngredientArchives_ToIngredientId",
                        column: x => x.ToIngredientId,
                        principalTable: "IngredientArchives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "IngredientArchives",
                columns: new[] { "Id", "Category", "Color", "CustomPropertiesJson", "DefaultUnit", "Description", "Icon", "IsSystemDefault", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, 1, "#FFC107", null, "g", null, "fa-solid fa-cubes", true, "Lievito di Birra Fresco", 1 },
                    { 2, 1, "#FF9800", null, "g", null, "fa-solid fa-cubes-stacked", true, "Lievito di Birra Secco", 2 },
                    { 3, 1, "#8D6E63", null, "g", null, "fa-solid fa-bread-slice", true, "Lievito Madre Solido", 3 },
                    { 4, 1, "#A1887F", null, "g", null, "fa-solid fa-droplet", true, "Licoli (Lievito Madre Liquido)", 4 }
                });

            migrationBuilder.InsertData(
                table: "IngredientConversions",
                columns: new[] { "Id", "ConversionRatio", "FromIngredientId", "Notes", "ToIngredientId" },
                values: new object[,]
                {
                    { 1, 0.33000000000000002, 1, "Il lievito secco è più concentrato", 2 },
                    { 2, 3.0, 1, "Aumentare tempo lievitazione", 3 },
                    { 3, 3.0, 1, "Ridurre liquidi del 15%", 4 },
                    { 4, 3.0, 2, null, 1 },
                    { 5, 1.0, 3, "Aggiungere 50% acqua per compensare idratazione", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_ArchiveIngredientId",
                table: "Ingredients",
                column: "ArchiveIngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientArchives_Category",
                table: "IngredientArchives",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientArchives_Name",
                table: "IngredientArchives",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientConversions_FromIngredientId_ToIngredientId",
                table: "IngredientConversions",
                columns: new[] { "FromIngredientId", "ToIngredientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientConversions_ToIngredientId",
                table: "IngredientConversions",
                column: "ToIngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_IngredientArchives_ArchiveIngredientId",
                table: "Ingredients",
                column: "ArchiveIngredientId",
                principalTable: "IngredientArchives",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_IngredientArchives_ArchiveIngredientId",
                table: "Ingredients");

            migrationBuilder.DropTable(
                name: "IngredientConversions");

            migrationBuilder.DropTable(
                name: "IngredientArchives");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_ArchiveIngredientId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "ArchiveIngredientId",
                table: "Ingredients");
        }
    }
}
