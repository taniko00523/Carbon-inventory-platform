using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaDevice");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_AreaId",
                table: "Devices",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Areas_AreaId",
                table: "Devices",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Areas_AreaId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_AreaId",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "AreaDevice",
                columns: table => new
                {
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaDevice", x => new { x.AreaId, x.AreasId });
                    table.ForeignKey(
                        name: "FK_AreaDevice_Areas_AreasId",
                        column: x => x.AreasId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaDevice_Devices_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaDevice_AreasId",
                table: "AreaDevice",
                column: "AreasId");
        }
    }
}
