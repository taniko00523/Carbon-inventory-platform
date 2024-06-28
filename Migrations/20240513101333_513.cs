using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _513 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "二氧化碳滅火器");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 17,
                column: "unit",
                value: "人-年");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "CO2滅火器");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 17,
                column: "unit",
                value: "人");
        }
    }
}
