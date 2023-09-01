using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0014 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_ActivityDatas_ActivityDataId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_ActivityDataId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ActivityDataId",
                table: "Devices");

            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "ActivityDatas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDatas_DeviceId",
                table: "ActivityDatas",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityDatas_Devices_DeviceId",
                table: "ActivityDatas",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityDatas_Devices_DeviceId",
                table: "ActivityDatas");

            migrationBuilder.DropIndex(
                name: "IX_ActivityDatas_DeviceId",
                table: "ActivityDatas");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "ActivityDatas");

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityDataId",
                table: "Devices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ActivityDataId",
                table: "Devices",
                column: "ActivityDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_ActivityDatas_ActivityDataId",
                table: "Devices",
                column: "ActivityDataId",
                principalTable: "ActivityDatas",
                principalColumn: "Id");
        }
    }
}
