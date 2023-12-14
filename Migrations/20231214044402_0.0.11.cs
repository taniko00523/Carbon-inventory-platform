using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _0011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 61,
                column: "Name",
                value: "冰箱");

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.0030000000000000001, "飲水機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.055, "商用冰箱" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.20000000000000001, "中、大型冰箱" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.33000000000000002, "低溫冷凍車" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.16, "乾燥機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.16, "工業冷藏、冷凍" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.16, "食品加工冷藏、冷凍" });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "EmissionPattern", "HFCSCEF", "N2OCEF", "N2OULL", "N2OUUL", "Name", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 69, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.089999999999999997, 0.0, 0f, 0f, "冰水主機", "", "", 0 },
                    { 70, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.029999999999999999, 0.0, 0f, 0f, "冷氣機", "", "", 0 },
                    { 71, 0.0, 0f, 0f, 0.0, 0f, 0f, "", 0.20000000000000001, 0.0, 0f, 0f, "車用空調", "", "", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 61,
                column: "Name",
                value: "家用冷凍、冷藏裝備");

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.055, "獨立商用冷凍、冷藏裝備" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.20000000000000001, "中、大型冷凍、冷藏裝備" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.33000000000000002, "交通用冷凍、冷藏裝備" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.16, "工業冷凍、冷藏裝備，包括食品加工及冷藏" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.089999999999999997, "冰水機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.029999999999999999, "住宅及商業建築冷氣機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.20000000000000001, "移動式空氣清靜機" });
        }
    }
}
