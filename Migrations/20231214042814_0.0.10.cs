using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _0010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "EmissionPattern", "HFCSCEF", "N2OCEF", "N2OULL", "N2OUUL", "Name", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 61, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.0030000000000000001, 0.0, 0f, 0f, "家用冷凍、冷藏裝備", "", "", 0 },
                    { 62, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.055, 0.0, 0f, 0f, "獨立商用冷凍、冷藏裝備", "", "", 0 },
                    { 63, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.20000000000000001, 0.0, 0f, 0f, "中、大型冷凍、冷藏裝備", "", "", 0 },
                    { 64, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.33000000000000002, 0.0, 0f, 0f, "交通用冷凍、冷藏裝備", "", "", 0 },
                    { 65, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.16, 0.0, 0f, 0f, "工業冷凍、冷藏裝備，包括食品加工及冷藏", "", "", 0 },
                    { 66, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.089999999999999997, 0.0, 0f, 0f, "冰水機", "", "", 0 },
                    { 67, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.029999999999999999, 0.0, 0f, 0f, "住宅及商業建築冷氣機", "", "", 0 },
                    { 68, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.20000000000000001, 0.0, 0f, 0f, "移動式空氣清靜機", "", "", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 68);
        }
    }
}
