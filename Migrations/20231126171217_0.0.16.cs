using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _0016 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-417A");

            migrationBuilder.CreateTable(
                name: "deviceDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmissionPattern = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deviceDatas", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 56,
                column: "EmissionPattern",
                value: "外購電力");

            migrationBuilder.InsertData(
                table: "deviceDatas",
                columns: new[] { "Id", "EmissionPattern", "Material", "Name", "Scope" },
                values: new object[,]
                {
                    { 1, "逸散", "R-410A", "冷氣機", "類別一" },
                    { 2, "逸散", "R-134A", "冰水主機", "類別一" },
                    { 3, "逸散", "R-134A", "冰箱", "類別一" },
                    { 4, "逸散", "R-134A", "飲水機", "類別一" },
                    { 5, "逸散", "R-134A", "乾燥機", "類別一" },
                    { 6, "逸散", "R-134A", "車用空調", "類別一" },
                    { 7, "逸散", "R-134A", "工業冷媒", "類別一" },
                    { 8, "固定", "柴油", "緊急發電機", "類別一" },
                    { 9, "逸散", "液化石油氣", "廚房", "類別一" },
                    { 10, "移動", "車用汽油", "公務車", "類別一" },
                    { 11, "移動", "柴油", "堆高機", "類別一" },
                    { 12, "逸散", "二氧化碳", "CO2滅火器", "類別一" },
                    { 13, "逸散", "二氧化碳", "二氧化碳", "類別一" },
                    { 14, "逸散", "二氧化碳", "WD40", "類別一" },
                    { 15, "逸散", "海龍1211", "海龍滅火器", "類別一" },
                    { 16, "逸散", "FM200", "FM200", "類別一" },
                    { 17, "逸散", "廢水處理", "化糞池", "類別一" },
                    { 18, "外購電力", "外購電力", "電力", "類別二" },
                    { 19, "製程", "乙炔", "乙炔", "類別一" },
                    { 20, "製程", "焊條", "焊條", "類別一" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deviceDatas");

            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[] { "R-417A", 2022, 2127f });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 56,
                column: "EmissionPattern",
                value: "其他電力");
        }
    }
}
