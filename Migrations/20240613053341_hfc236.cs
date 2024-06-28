using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class hfc236 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[] { "HFC-236fa", 2022, 8690m });

            migrationBuilder.InsertData(
                table: "deviceDatas",
                columns: new[] { "Id", "Data_Correction", "Device_Correction", "EmissionPattern", "Material", "Name", "Scope", "unit" },
                values: new object[] { 27, 3, 3, "逸散", "HFC-236fa", "六氟丙烷滅火器", "類別一", "公斤" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "HFC-236fa");

            migrationBuilder.DeleteData(
                table: "deviceDatas",
                keyColumn: "Id",
                keyValue: 27);
        }
    }
}
