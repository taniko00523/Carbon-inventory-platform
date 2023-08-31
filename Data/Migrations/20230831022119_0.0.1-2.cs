using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_CH4s_CH4Id",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_CO2s_CO2Id",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_N2Os_N2OId",
                table: "Materials");

            migrationBuilder.AlterColumn<int>(
                name: "N2OId",
                table: "Materials",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CO2Id",
                table: "Materials",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CH4Id",
                table: "Materials",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { null, null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_CH4s_CH4Id",
                table: "Materials",
                column: "CH4Id",
                principalTable: "CH4s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_CO2s_CO2Id",
                table: "Materials",
                column: "CO2Id",
                principalTable: "CO2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_N2Os_N2OId",
                table: "Materials",
                column: "N2OId",
                principalTable: "N2Os",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_CH4s_CH4Id",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_CO2s_CO2Id",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_N2Os_N2OId",
                table: "Materials");

            migrationBuilder.AlterColumn<int>(
                name: "N2OId",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CO2Id",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CH4Id",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 36, 36, 36 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 37, 37, 37 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 38, 38, 38 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 39, 39, 39 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 40, 40, 40 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 41, 41, 41 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 42, 42, 42 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 43, 43, 43 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 44, 44, 44 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 45, 45, 45 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 46, 46, 46 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 47, 47, 47 });

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_CH4s_CH4Id",
                table: "Materials",
                column: "CH4Id",
                principalTable: "CH4s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_CO2s_CO2Id",
                table: "Materials",
                column: "CO2Id",
                principalTable: "CO2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_N2Os_N2OId",
                table: "Materials",
                column: "N2OId",
                principalTable: "N2Os",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
