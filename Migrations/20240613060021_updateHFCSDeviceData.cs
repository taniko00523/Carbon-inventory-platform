using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class updateHFCSDeviceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "海龍-1211");

            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[] { "二氟一氯一溴甲烷", 2022, 1930m });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CEF_Correction", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "HFCSULL", "HFCSUUL", "N2OCEF", "N2OULL", "N2OUUL", "NF3CEF", "NF3ULL", "NF3UUL", "Name", "PFCSCEF", "PFCSULL", "PFCSUUL", "SF6CEF", "SF6ULL", "SF6UUL", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 80, 1, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 1m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "HFC-236fa", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 81, 1, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 1m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "二氟一氯一溴甲烷", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 }
                });

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 15,
                column: "Material",
                value: "二氟一氯一溴甲烷");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "二氟一氯一溴甲烷");

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[] { "海龍-1211", 2022, 1930m });

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 15,
                column: "Material",
                value: "海龍-1211");
        }
    }
}
