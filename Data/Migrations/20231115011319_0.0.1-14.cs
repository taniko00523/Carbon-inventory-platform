using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _00114 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GWP",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Num = table.Column<float>(type: "real", nullable: false),
                    GWP_Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GWP", x => x.Name);
                });

            migrationBuilder.InsertData(
                table: "GWP",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[,]
                {
                    { "七氟丙烷", 2022, 3600f },
                    { "CH4", 2022, 27.9f },
                    { "CO2", 2022, 1f },
                    { "N2O", 2022, 273f },
                    { "NF3", 2022, 17400f },
                    { "R-134A", 2022, 1530f },
                    { "R-22", 2022, 1960f },
                    { "R-23", 2022, 14600f },
                    { "R-32", 2022, 771f },
                    { "R-410A", 2022, 2256f },
                    { "R-600A", 2022, 0f },
                    { "SF6", 2022, 24300f }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GWP");
        }
    }
}
