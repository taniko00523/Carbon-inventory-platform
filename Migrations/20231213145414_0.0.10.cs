using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _0010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Materials_MaterialId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_MaterialId",
                table: "Devices");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaterialId",
                table: "Devices",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "MaterialId1",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MaterialId1",
                table: "Devices",
                column: "MaterialId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Materials_MaterialId1",
                table: "Devices",
                column: "MaterialId1",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Materials_MaterialId1",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_MaterialId1",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "MaterialId1",
                table: "Devices");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialId",
                table: "Devices",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MaterialId",
                table: "Devices",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Materials_MaterialId",
                table: "Devices",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
