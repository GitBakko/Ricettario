using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ricettario.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhaseTypeIcons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#0d6efd", "fa-solid fa-compact-disc" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#ffc107", "fa-solid fa-hourglass-half" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#dc3545", "fa-solid fa-fire" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Icon",
                value: "fa-solid fa-basket-shopping");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#198754", "fa-solid fa-layer-group" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#0dcaf0", "fa-solid fa-box" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#fd7e14", "fa-solid fa-rotate-right" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#20c997", "fa-solid fa-droplet" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "fa-solid fa-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "fa-solid fa-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "fa-solid fa-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Icon",
                value: "fa-solid fa-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "fa-solid fa-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "fa-solid fa-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "fa-solid fa-circle" });

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Color", "Icon" },
                values: new object[] { "#6c757d", "fa-solid fa-circle" });
        }
    }
}
