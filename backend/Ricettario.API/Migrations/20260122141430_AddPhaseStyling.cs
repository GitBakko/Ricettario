using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ricettario.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPhaseStyling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "RecipePhaseTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "RecipePhaseTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "bi-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "bi-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "bi-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "bi-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "bi-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "bi-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "bi-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "bi-circle" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "RecipePhaseTypes");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "RecipePhaseTypes");
        }
    }
}
