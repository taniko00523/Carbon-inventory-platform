using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _0015 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "EmissionPattern", "HFCSCEF", "N2OCEF", "N2OULL", "N2OUUL", "Name", "Scope", "Unit", "Year" },
                values: new object[] { 72, 0.0, 0f, 0f, 3.0259999999999998, 0f, 0f, "製程", 0.0, 0.0, 0f, 0f, "丁烷", "類別1", "", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 72);
        }
    }
}
