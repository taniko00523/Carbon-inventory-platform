using System;
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
            migrationBuilder.DropColumn(
                name: "Num",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "ActivityDatas",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Num = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    remark = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityDatas", x => x.id);
                    table.ForeignKey(
                        name: "FK_ActivityDatas_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDatas_DeviceId",
                table: "ActivityDatas",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityDatas");

            migrationBuilder.AddColumn<decimal>(
                name: "Num",
                table: "Devices",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
