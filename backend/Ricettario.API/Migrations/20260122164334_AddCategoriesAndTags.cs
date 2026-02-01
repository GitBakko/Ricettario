using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ricettario.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesAndTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsSystemDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeTags",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeTags", x => new { x.RecipeId, x.TagId });
                    table.ForeignKey(
                        name: "FK_RecipeTags_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "Description", "Icon", "IsSystemDefault", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "#dc3545", "Pizze classiche, gourmet e regionali", "bi-pie-chart-fill", true, "Pizza", 1 },
                    { 2, "#8B4513", "Pane artigianale, filoni, pagnotte", "bi-basket-fill", true, "Pane", 2 },
                    { 3, "#f0ad4e", "Focacce liguri, pugliesi e varianti", "bi-grid-3x3-gap-fill", true, "Focaccia", 3 },
                    { 4, "#e91e8c", "Dolci da forno, crostate, torte", "bi-cake2-fill", true, "Pasticceria", 4 },
                    { 5, "#ffc107", "Panettone, pandoro, colomba", "bi-star-fill", true, "Grandi Lievitati", 5 },
                    { 6, "#fd7e14", "Brioches dolci e salate, croissant", "bi-moon-fill", true, "Brioche", 6 },
                    { 7, "#795548", "Biscotti, frollini, cantucci", "bi-cookie", true, "Biscotti", 7 },
                    { 8, "#28a745", "Quiche, torte rustiche, sfoglie", "bi-hexagon-fill", true, "Torte Salate", 8 },
                    { 9, "#17a2b8", "Pasta all'uovo, ravioli, gnocchi", "bi-layers-fill", true, "Pasta Fresca", 9 },
                    { 10, "#c7a17a", "Pain au chocolat, danish, sfoglia", "bi-suit-diamond-fill", true, "Viennoiserie", 10 },
                    { 11, "#6c757d", "Altre preparazioni da forno", "bi-three-dots", true, "Altro", 99 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CategoryId",
                table: "Recipes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTags_TagId",
                table: "RecipeTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Categories_CategoryId",
                table: "Recipes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Categories_CategoryId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "RecipeTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_CategoryId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Recipes");
        }
    }
}
