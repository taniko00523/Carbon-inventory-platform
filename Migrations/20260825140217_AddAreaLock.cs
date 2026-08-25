using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaLock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Areas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedAt",
                table: "Areas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LockedByUserId",
                table: "Areas",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LockedByUserName",
                table: "Areas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LockedSnapshotAll",
                table: "Areas",
                type: "decimal(18,4)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "LockedAt",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "LockedByUserId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "LockedByUserName",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "LockedSnapshotAll",
                table: "Areas");
        }
    }
}
