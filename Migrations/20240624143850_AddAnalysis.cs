using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ARVersion",
                table: "Areas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    _21A = table.Column<int>(type: "int", nullable: false),
                    _21B = table.Column<int>(type: "int", nullable: false),
                    _21C = table.Column<int>(type: "int", nullable: false),
                    _22A = table.Column<int>(type: "int", nullable: false),
                    _22B = table.Column<int>(type: "int", nullable: false),
                    _22C = table.Column<int>(type: "int", nullable: false),
                    _31A = table.Column<int>(type: "int", nullable: false),
                    _31B = table.Column<int>(type: "int", nullable: false),
                    _31C = table.Column<int>(type: "int", nullable: false),
                    _32A = table.Column<int>(type: "int", nullable: false),
                    _32B = table.Column<int>(type: "int", nullable: false),
                    _32C = table.Column<int>(type: "int", nullable: false),
                    _33A = table.Column<int>(type: "int", nullable: false),
                    _33B = table.Column<int>(type: "int", nullable: false),
                    _33C = table.Column<int>(type: "int", nullable: false),
                    _34A = table.Column<int>(type: "int", nullable: false),
                    _34B = table.Column<int>(type: "int", nullable: false),
                    _34C = table.Column<int>(type: "int", nullable: false),
                    _35A = table.Column<int>(type: "int", nullable: false),
                    _35B = table.Column<int>(type: "int", nullable: false),
                    _35C = table.Column<int>(type: "int", nullable: false),
                    _41A = table.Column<int>(type: "int", nullable: false),
                    _41B = table.Column<int>(type: "int", nullable: false),
                    _41C = table.Column<int>(type: "int", nullable: false),
                    _42A = table.Column<int>(type: "int", nullable: false),
                    _42B = table.Column<int>(type: "int", nullable: false),
                    _42C = table.Column<int>(type: "int", nullable: false),
                    _43A = table.Column<int>(type: "int", nullable: false),
                    _43B = table.Column<int>(type: "int", nullable: false),
                    _43C = table.Column<int>(type: "int", nullable: false),
                    _44A = table.Column<int>(type: "int", nullable: false),
                    _44B = table.Column<int>(type: "int", nullable: false),
                    _44C = table.Column<int>(type: "int", nullable: false),
                    _45A = table.Column<int>(type: "int", nullable: false),
                    _45B = table.Column<int>(type: "int", nullable: false),
                    _45C = table.Column<int>(type: "int", nullable: false),
                    _46A = table.Column<int>(type: "int", nullable: false),
                    _46B = table.Column<int>(type: "int", nullable: false),
                    _46C = table.Column<int>(type: "int", nullable: false),
                    _47A = table.Column<int>(type: "int", nullable: false),
                    _47B = table.Column<int>(type: "int", nullable: false),
                    _47C = table.Column<int>(type: "int", nullable: false),
                    _48A = table.Column<int>(type: "int", nullable: false),
                    _48B = table.Column<int>(type: "int", nullable: false),
                    _48C = table.Column<int>(type: "int", nullable: false),
                    _49A = table.Column<int>(type: "int", nullable: false),
                    _49B = table.Column<int>(type: "int", nullable: false),
                    _49C = table.Column<int>(type: "int", nullable: false),
                    _410A = table.Column<int>(type: "int", nullable: false),
                    _410B = table.Column<int>(type: "int", nullable: false),
                    _410C = table.Column<int>(type: "int", nullable: false),
                    _411A = table.Column<int>(type: "int", nullable: false),
                    _411B = table.Column<int>(type: "int", nullable: false),
                    _411C = table.Column<int>(type: "int", nullable: false),
                    _51A = table.Column<int>(type: "int", nullable: false),
                    _51B = table.Column<int>(type: "int", nullable: false),
                    _51C = table.Column<int>(type: "int", nullable: false),
                    _52A = table.Column<int>(type: "int", nullable: false),
                    _52B = table.Column<int>(type: "int", nullable: false),
                    _52C = table.Column<int>(type: "int", nullable: false),
                    _53A = table.Column<int>(type: "int", nullable: false),
                    _53B = table.Column<int>(type: "int", nullable: false),
                    _53C = table.Column<int>(type: "int", nullable: false),
                    _54A = table.Column<int>(type: "int", nullable: false),
                    _54B = table.Column<int>(type: "int", nullable: false),
                    _54C = table.Column<int>(type: "int", nullable: false),
                    _55A = table.Column<int>(type: "int", nullable: false),
                    _55B = table.Column<int>(type: "int", nullable: false),
                    _55C = table.Column<int>(type: "int", nullable: false),
                    _61A = table.Column<int>(type: "int", nullable: false),
                    _61B = table.Column<int>(type: "int", nullable: false),
                    _61C = table.Column<int>(type: "int", nullable: false),
                    _21 = table.Column<int>(type: "int", nullable: false),
                    _22 = table.Column<int>(type: "int", nullable: false),
                    _31 = table.Column<int>(type: "int", nullable: false),
                    _32 = table.Column<int>(type: "int", nullable: false),
                    _33 = table.Column<int>(type: "int", nullable: false),
                    _34 = table.Column<int>(type: "int", nullable: false),
                    _35 = table.Column<int>(type: "int", nullable: false),
                    _41 = table.Column<int>(type: "int", nullable: false),
                    _42 = table.Column<int>(type: "int", nullable: false),
                    _43 = table.Column<int>(type: "int", nullable: false),
                    _44 = table.Column<int>(type: "int", nullable: false),
                    _45 = table.Column<int>(type: "int", nullable: false),
                    _46 = table.Column<int>(type: "int", nullable: false),
                    _47 = table.Column<int>(type: "int", nullable: false),
                    _48 = table.Column<int>(type: "int", nullable: false),
                    _49 = table.Column<int>(type: "int", nullable: false),
                    _410 = table.Column<int>(type: "int", nullable: false),
                    _411 = table.Column<int>(type: "int", nullable: false),
                    _51 = table.Column<int>(type: "int", nullable: false),
                    _52 = table.Column<int>(type: "int", nullable: false),
                    _53 = table.Column<int>(type: "int", nullable: false),
                    _54 = table.Column<int>(type: "int", nullable: false),
                    _55 = table.Column<int>(type: "int", nullable: false),
                    _61 = table.Column<int>(type: "int", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analyses_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_AreaId",
                table: "Analyses",
                column: "AreaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analyses");

            migrationBuilder.DropColumn(
                name: "ARVersion",
                table: "Areas");
        }
    }
}
