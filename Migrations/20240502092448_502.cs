using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _502 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 44,
                column: "Year",
                value: 112);

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CEF_Correction", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "HFCSULL", "HFCSUUL", "N2OCEF", "N2OULL", "N2OUUL", "NF3CEF", "NF3ULL", "NF3UUL", "Name", "PFCSCEF", "PFCSULL", "PFCSUUL", "SF6CEF", "SF6ULL", "SF6UUL", "Scope", "Unit", "Year" },
                values: new object[] { 45, 3, 0m, 0m, 0m, 0.494m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 113 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 44,
                column: "Year",
                value: 0);
        }
    }
}
