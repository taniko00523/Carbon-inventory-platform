using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class editGWP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GWPs",
                table: "GWPs");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "二氟一氯一溴甲烷");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "CH4");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "CO2");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "FM200");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "HFC-236fa");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "N2O");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "NF3");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-12");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-1234yf");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-125");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-134A");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-22");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-23");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-32");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-404A");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-407C");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-407F");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-410A");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-417A");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-452A");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-507A");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "R-600A");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Name",
                keyValue: "SF6");

            migrationBuilder.RenameColumn(
                name: "GWP_Year",
                table: "GWPs",
                newName: "ARCount");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GWPs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GWPs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GWPs",
                table: "GWPs",
                column: "Id");

            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Id", "ARCount", "Name", "Num" },
                values: new object[,]
                {
                    { 1, 6, "CO2", 1m },
                    { 2, 6, "CH4", 27.9m },
                    { 3, 6, "N2O", 273m },
                    { 4, 6, "R-12", 12500m },
                    { 5, 6, "R-125", 3740m },
                    { 6, 6, "R-1234yf", 0.501m },
                    { 7, 6, "R-23", 14600m },
                    { 8, 6, "R-32", 771m },
                    { 9, 6, "R-134A", 1530m },
                    { 10, 6, "FM200", 3600m },
                    { 11, 6, "R-22", 1960m },
                    { 12, 6, "R-410A", 2255.5m },
                    { 13, 6, "R-600A", 0.006m },
                    { 14, 6, "R-417A", 2127m },
                    { 15, 6, "R-404A", 4728m },
                    { 16, 6, "R-407C", 1908m },
                    { 17, 6, "R-407F", 1965.3m },
                    { 18, 6, "R-452A", 2291.5603m },
                    { 19, 6, "R-507A", 4475m },
                    { 20, 6, "NF3", 17400m },
                    { 21, 6, "SF6", 24300m },
                    { 22, 6, "二氟一氯一溴甲烷", 1930m },
                    { 23, 6, "HFC-236fa", 8690m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GWPs",
                table: "GWPs");

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "GWPs",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 23);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GWPs");

            migrationBuilder.RenameColumn(
                name: "ARCount",
                table: "GWPs",
                newName: "GWP_Year");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GWPs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GWPs",
                table: "GWPs",
                column: "Name");

            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[,]
                {
                    { "二氟一氯一溴甲烷", 2022, 1930m },
                    { "CH4", 2022, 27.9m },
                    { "CO2", 2022, 1m },
                    { "FM200", 2022, 3600m },
                    { "HFC-236fa", 2022, 8690m },
                    { "N2O", 2022, 273m },
                    { "NF3", 2022, 17400m },
                    { "R-12", 2022, 12500m },
                    { "R-1234yf", 2022, 0.501m },
                    { "R-125", 2022, 3740m },
                    { "R-134A", 2022, 1530m },
                    { "R-22", 2022, 1960m },
                    { "R-23", 2022, 14600m },
                    { "R-32", 2022, 771m },
                    { "R-404A", 2022, 4728m },
                    { "R-407C", 2022, 1908m },
                    { "R-407F", 2022, 1965.3m },
                    { "R-410A", 2022, 2255.5m },
                    { "R-417A", 2022, 2127m },
                    { "R-452A", 2022, 2291.5603m },
                    { "R-507A", 2022, 4475m },
                    { "R-600A", 2022, 0.006m },
                    { "SF6", 2022, 24300m }
                });
        }
    }
}
