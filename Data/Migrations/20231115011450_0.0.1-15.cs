using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _00115 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GWP",
                table: "GWP");

            migrationBuilder.RenameTable(
                name: "GWP",
                newName: "GWPs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GWPs",
                table: "GWPs",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GWPs",
                table: "GWPs");

            migrationBuilder.RenameTable(
                name: "GWPs",
                newName: "GWP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GWP",
                table: "GWP",
                column: "Name");
        }
    }
}
