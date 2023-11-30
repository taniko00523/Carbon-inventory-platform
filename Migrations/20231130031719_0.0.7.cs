using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "unit",
                table: "deviceDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 1,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 2,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 3,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 4,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 5,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 6,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 7,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 8,
                column: "unit",
                value: "公升");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 9,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 10,
                column: "unit",
                value: "公升");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 11,
                column: "unit",
                value: "公升");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 12,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 13,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 14,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 15,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 16,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 17,
                column: "unit",
                value: "人");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 18,
                column: "unit",
                value: "度");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 19,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 20,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 21,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 22,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 23,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 24,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 25,
                column: "unit",
                value: "公斤");

            migrationBuilder.UpdateData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 26,
                column: "unit",
                value: "公斤");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "unit",
                table: "deviceDatas");
        }
    }
}
