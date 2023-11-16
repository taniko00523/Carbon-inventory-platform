using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _00118 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dataCorrections",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dataCorrections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dataLevels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dataLevels", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "dataCorrections",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "有外部校正或多組數據佐證者" },
                    { 2, "有內部校正或經過會計簽證等證明者" },
                    { 3, "未進行儀器校正或未進行紀錄彙整者" }
                });

            migrationBuilder.InsertData(
                table: "dataLevels",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "連續監測" },
                    { 2, "定期/間歇量測" },
                    { 3, "自行/財務推估" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dataCorrections");

            migrationBuilder.DropTable(
                name: "dataLevels");
        }
    }
}
