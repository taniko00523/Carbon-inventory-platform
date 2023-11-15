using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _00116 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refrigerants",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Num = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refrigerants", x => x.Name);
                });

            migrationBuilder.InsertData(
                table: "refrigerants",
                columns: new[] { "Name", "Num" },
                values: new object[,]
                {
                    { "工業冷凍、冷藏裝備，包括食品加工及冷藏", 0.15999999642372131 },
                    { "中、大型冷凍、冷藏裝備", 0.20000000298023224 },
                    { "交通用冷凍、冷藏裝備", 0.33000001311302185 },
                    { "冰水機", 0.090000003576278687 },
                    { "住宅及商業建築冷氣機", 0.029999999329447746 },
                    { "家用冷凍、冷藏裝備", 0.0030000000260770321 },
                    { "移動式空氣清靜機", 0.20000000298023224 },
                    { "獨立商用冷凍、冷藏裝備", 0.054999999701976776 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refrigerants");
        }
    }
}
