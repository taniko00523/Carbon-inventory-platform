using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[,]
                {
                    { "R-417A", 2022, 2127f },
                    { "R-507A", 2022, 4475f }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-417A");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-507A");
        }
    }
}
