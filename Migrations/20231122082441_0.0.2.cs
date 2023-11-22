using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "all_Grade",
                table: "emissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "avg_Grade",
                table: "emissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "no1_Grade",
                table: "emissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "no2_Grade",
                table: "emissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "no3_Grade",
                table: "emissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "all_Grade",
                table: "emissions");

            migrationBuilder.DropColumn(
                name: "avg_Grade",
                table: "emissions");

            migrationBuilder.DropColumn(
                name: "no1_Grade",
                table: "emissions");

            migrationBuilder.DropColumn(
                name: "no2_Grade",
                table: "emissions");

            migrationBuilder.DropColumn(
                name: "no3_Grade",
                table: "emissions");
        }
    }
}
