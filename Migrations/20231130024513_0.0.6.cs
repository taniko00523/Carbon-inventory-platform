using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "工業冷藏、冷凍");

            migrationBuilder.InsertData(
                table: "deviceDatas",
                columns: new[] { "Id", "Correction", "EmissionPattern", "Level", "Material", "Name", "Scope" },
                values: new object[,]
                {
                    { 21, 3, "逸散", 3, "R-134A", "工業冷藏、冷凍", "類別一" },
                    { 22, 3, "逸散", 3, "R-134A", "商用冰箱", "類別一" },
                    { 23, 3, "逸散", 3, "R-134A", "中、大型冰箱", "類別一" },
                    { 24, 3, "逸散", 3, "R-134A", "低溫冷凍車", "類別一" },
                    { 25, 3, "逸散", 3, "R-134A", "食品加工冷藏、冷凍", "類別一" },
                    { 26, 3, "製程", 3, "丁烷", "瓦斯罐", "類別一" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "工業冷媒");
        }
    }
}
