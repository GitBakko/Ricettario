using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ricettario.API.Migrations
{
    /// <inheritdoc />
    public partial class ConvertIconsToFontAwesome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Icon",
                value: "fa-solid fa-pizza-slice");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: "fa-solid fa-bread-slice");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Icon",
                value: "fa-solid fa-border-all");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Icon",
                value: "fa-solid fa-cake-candles");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Icon",
                value: "fa-solid fa-star");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "Icon",
                value: "fa-solid fa-moon");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "Icon",
                value: "fa-solid fa-cookie");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "Icon",
                value: "fa-solid fa-hexagon");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "Icon",
                value: "fa-solid fa-layer-group");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "Icon",
                value: "fa-solid fa-diamond");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "Icon",
                value: "fa-solid fa-ellipsis");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Icon",
                value: "fa-solid fa-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: "fa-solid fa-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Icon",
                value: "fa-solid fa-circle");

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
                column: "Icon",
                value: "fa-solid fa-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "Icon",
                value: "fa-solid fa-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "Icon",
                value: "fa-solid fa-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "Icon",
                value: "fa-solid fa-circle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Icon",
                value: "bi-pie-chart-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: "bi-basket-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Icon",
                value: "bi-grid-3x3-gap-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Icon",
                value: "bi-cake2-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Icon",
                value: "bi-star-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "Icon",
                value: "bi-moon-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                column: "Icon",
                value: "bi-cookie");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                column: "Icon",
                value: "bi-hexagon-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                column: "Icon",
                value: "bi-layers-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "Icon",
                value: "bi-suit-diamond-fill");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "Icon",
                value: "bi-three-dots");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Icon",
                value: "bi-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: "bi-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Icon",
                value: "bi-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Icon",
                value: "bi-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Icon",
                value: "bi-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "Icon",
                value: "bi-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "Icon",
                value: "bi-circle");

            migrationBuilder.UpdateData(
                table: "RecipePhaseTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "Icon",
                value: "bi-circle");
        }
    }
}
