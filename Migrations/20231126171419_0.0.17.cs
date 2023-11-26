using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _0017 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 9,
                column: "EmissionPattern",
                value: "移動");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 9,
                column: "EmissionPattern",
                value: "逸散");
        }
    }
}
