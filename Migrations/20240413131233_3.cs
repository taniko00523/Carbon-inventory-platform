using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CEF_Correction", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "HFCSULL", "HFCSUUL", "N2OCEF", "N2OULL", "N2OUUL", "NF3CEF", "NF3ULL", "NF3UUL", "Name", "PFCSCEF", "PFCSULL", "PFCSUUL", "SF6CEF", "SF6ULL", "SF6UUL", "Scope", "Unit", "Year" },
                values: new object[] { 74, 3, 0m, 0m, 0m, 0.025m, 0m, 0m, 0m, 0m, "逸散", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "WD40", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 });

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 14,
                column: "Material",
                value: "WD40");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 14,
                column: "Material",
                value: "二氧化碳");
        }
    }
}
