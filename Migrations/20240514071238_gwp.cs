using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class gwp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-410A",
                column: "Num",
                value: 2255.5m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-410A",
                column: "Num",
                value: 2256m);
        }
    }
}
