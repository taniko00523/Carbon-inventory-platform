using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Materials_MaterialId",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "Material",
                table: "Devices",
                newName: "Material_Name");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialId",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Materials_MaterialId",
                table: "Devices",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Materials_MaterialId",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "Material_Name",
                table: "Devices",
                newName: "Material");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialId",
                table: "Devices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Materials_MaterialId",
                table: "Devices",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");
        }
    }
}
