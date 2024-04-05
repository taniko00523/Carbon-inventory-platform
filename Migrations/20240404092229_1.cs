using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "GWP",
                table: "GHGs",
                type: "decimal(18, 10)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Customize",
                table: "Devices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customize",
                table: "Devices");

            migrationBuilder.AlterColumn<decimal>(
                name: "GWP",
                table: "GHGs",
                type: "decimal(18, 10)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 10)");
        }
    }
}
