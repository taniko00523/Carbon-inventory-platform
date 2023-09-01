using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0015 : Migration
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

            migrationBuilder.DropTable(
                name: "ActivityDatas");

            migrationBuilder.DropTable(
                name: "CH4s");

            migrationBuilder.DropTable(
                name: "CO2s");

            migrationBuilder.DropTable(
                name: "N2Os");

            migrationBuilder.DropIndex(
                name: "IX_Materials_CH4Id",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_CO2Id",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_N2OId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CH4Id",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CO2Id",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "N2OId",
                table: "Materials");

            migrationBuilder.AddColumn<float>(
                name: "CH4CEF",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CH4ULL",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CH4UUL",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CO2CEF",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CO2ULL",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CO2UUL",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "N2OCEF",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "N2OULL",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "N2OUUL",
                table: "Materials",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Correction",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Dept",
                table: "Devices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Num",
                table: "Devices",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Devices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Devices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.5E-05f, 0.7f, 2f, 2.33286f, 0.077167f, 0.067653f, 3.7E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.8E-05f, 0.7f, 2f, 2.693285f, 0.077167f, 0.067653f, 4.3E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.5E-05f, 0.7f, 2f, 2.408113f, 0.077167f, 0.067653f, 3.8E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 3E-05f, 0.7f, 2f, 2.922093f, 0.03764f, 0.027467f, 4.5E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.8E-05f, 0.7f, 2f, 2.693285f, 0.077167f, 0.067653f, 4.3E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.5E-05f, 0.7f, 2f, 2.408113f, 0.053911f, 0.053911f, 3.8E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.1E-05f, 0.7f, 2f, 1.971522f, 0.034339f, 0.040583f, 3.1E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.3E-05f, 0.7f, 2f, 2.253168f, 0.034339f, 0.040583f, 3.5E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 1.2E-05f, 0.7f, 2f, 1.202633f, 0.1f, 0.138614f, 1.8E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 9E-06f, 0.7f, 2f, 0.95287f, 0.157009f, 0.168224f, 1.3E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 1E-05f, 0.7f, 2f, 1.035387f, 0.056604f, 0.018868f, 1.5E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 1.6E-05f, 0.7f, 2f, 1.551209f, 0.104615f, 0.117949f, 2.4E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.9E-05f, 0.7f, 2f, 3.135913f, 0.105607f, 0.11215f, 4.4E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000103f, 0.666667f, 2.333333f, 3.347347f, 0.149744f, 0.179487f, 2.1E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 9.4E-05f, 0.666667f, 2.333333f, 2.19807f, 0.035714f, 0.042857f, 1.9E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.0001f, 0.666667f, 2.333333f, 2.39485f, 0.025175f, 0.040559f, 2E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000113f, 0.666667f, 2.333333f, 2.762032f, 0.030014f, 0.030014f, 2.3E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 8.3E-05f, 0.666667f, 2.333333f, 2.119027f, 0.1f, 0.109091f, 1.7E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000133f, 0.666667f, 2.333333f, 2.839525f, 0.0919f, 0.096573f, 2.7E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000107f, 0.666667f, 2.333333f, 2.558763f, 0.015299f, 0.025035f, 2.1E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000108f, 0.666667f, 2.333333f, 2.794563f, 0.075034f, 0.080491f, 2.2E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000106f, 0.666667f, 2.333333f, 2.606032f, 0.020243f, 0.009447f, 2.1E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 9.8E-05f, 0.666667f, 2.333333f, 2.263133f, 0.025974f, 0.053391f, 2E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000121f, 0.666667f, 2.333333f, 3.11096f, 0.024548f, 0.018088f, 2.4E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 2.8E-05f, 0.7f, 2f, 1.752881f, 0.023772f, 0.03962f, 3E-06f, 0.7f, 2f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 9.8E-05f, 0.666667f, 2.333333f, 2.393761f, 0.05457f, 0.040928f, 2E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000126f, 0.666667f, 2.333333f, 3.378748f, 0.095415f, 0.114002f, 2.5E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000121f, 0.666667f, 2.333333f, 2.946167f, 0.0191f, 0.025921f, 2.4E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000113f, 0.666667f, 2.333333f, 2.762032f, 0.015007f, 0.015007f, 2.3E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 4.6E-05f, 0.7f, 2f, 2.860187f, 0.082792f, 0.113636f, 5E-06f, 0.7f, 2f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 3.3E-05f, 0.7f, 2f, 1.879036f, 0.032086f, 0.039216f, 3E-06f, 0.7f, 2f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 3.8E-05f, 0.7f, 2f, 2.170437f, 0.163194f, 0.197917f, 4E-06f, 0.7f, 2f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 1.8E-05f, 0.7f, 2f, 0.780754f, 0.15991f, 0.218468f, 2E-06f, 0.7f, 2f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 3E-06f, 0.7f, 2f, 0.845817f, 0.157692f, 0.184615f, 0f, 0.7f, 2f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000255f, 0.666667f, 2.333333f, 0.779227f, 0.200654f, 0.31952f, 3.4E-05f, 0.625f, 2.75f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 9.4E-05f, 0.666667f, 2.333333f, 2.19807f, 0.035714f, 0.042857f, 1.9E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.0001f, 0.666667f, 2.333333f, 2.39485f, 0.025175f, 0.040559f, 2E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000816f, 0.666667f, 2.44f, 2.263133f, 0.025974f, 0.053391f, 0.000261f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000137f, 0.589744f, 1.435897f, 2.606032f, 0.020243f, 0.009447f, 0.000137f, 0.666667f, 2.076923f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000107f, 0.666667f, 2.333333f, 2.558763f, 0.015299f, 0.025035f, 2.1E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.000121f, 0.666667f, 2.333333f, 2.946167f, 0.0191f, 0.025921f, 2.4E-05f, 0.666667f, 2.333333f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.001722f, 0f, 0f, 1.752881f, 0.023772f, 0.03962f, 6E-06f, 0f, 0f, 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "N2OCEF", "N2OULL", "N2OUUL", "Year" },
                values: new object[] { 0.003467f, 0.456522f, 15.73913f, 2.113915f, 0.032086f, 0.039216f, 0.000113f, 0.666667f, 24.666668f, 0 });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "EmissionPattern", "N2OCEF", "N2OULL", "N2OUUL", "Name", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 56, null, null, null, 0.559f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 57, null, null, null, 0.559f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 58, null, null, null, 0.559f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 59, null, null, null, 0.559f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 60, null, null, null, 0.559f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 61, null, null, null, 0.559f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 62, null, null, null, 0.564f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 63, null, null, null, 0.559f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 64, null, null, null, 0.557f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 65, null, null, null, 0.543f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 66, null, null, null, 0.535f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 67, null, null, null, 0.536f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 68, null, null, null, 0.532f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 69, null, null, null, 0.522f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 70, null, null, null, 0.521f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 71, null, null, null, 0.528f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 72, null, null, null, 0.529f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 73, null, null, null, 0.529f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 },
                    { 74, null, null, null, 0.533f, null, null, "其他電力", null, null, null, "外購電力", "範疇2", "", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DropColumn(
                name: "CH4CEF",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CH4ULL",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CH4UUL",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CO2CEF",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CO2ULL",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CO2UUL",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "N2OCEF",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "N2OULL",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "N2OUUL",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Correction",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Dept",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Num",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Devices");

            migrationBuilder.AddColumn<int>(
                name: "CH4Id",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CO2Id",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "N2OId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Correction = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Dept = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Num = table.Column<float>(type: "real", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityDatas_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CH4s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CEF = table.Column<float>(type: "real", nullable: false),
                    UncertaintyLowerLimit = table.Column<float>(type: "real", nullable: false),
                    UncertaintyUpperLimit = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CH4s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CO2s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CEF = table.Column<float>(type: "real", nullable: false),
                    UncertaintyLowerLimit = table.Column<float>(type: "real", nullable: false),
                    UncertaintyUpperLimit = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CO2s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "N2Os",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CEF = table.Column<float>(type: "real", nullable: false),
                    UncertaintyLowerLimit = table.Column<float>(type: "real", nullable: false),
                    UncertaintyUpperLimit = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_N2Os", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CH4s",
                columns: new[] { "Id", "CEF", "UncertaintyLowerLimit", "UncertaintyUpperLimit" },
                values: new object[,]
                {
                    { 1, 2.5E-05f, 0.7f, 2f },
                    { 2, 2.8E-05f, 0.7f, 2f },
                    { 3, 2.5E-05f, 0.7f, 2f },
                    { 4, 3E-05f, 0.7f, 2f },
                    { 5, 2.8E-05f, 0.7f, 2f },
                    { 6, 2.5E-05f, 0.7f, 2f },
                    { 7, 2.1E-05f, 0.7f, 2f },
                    { 8, 2.3E-05f, 0.7f, 2f },
                    { 9, 1.2E-05f, 0.7f, 2f },
                    { 10, 9E-06f, 0.7f, 2f },
                    { 11, 1E-05f, 0.7f, 2f },
                    { 12, 1.6E-05f, 0.7f, 2f },
                    { 13, 2.9E-05f, 0.7f, 2f },
                    { 14, 0.000103f, 0.666667f, 2.333333f },
                    { 15, 9.4E-05f, 0.666667f, 2.333333f },
                    { 16, 0.0001f, 0.666667f, 2.333333f },
                    { 17, 0.000113f, 0.666667f, 2.333333f },
                    { 18, 8.3E-05f, 0.666667f, 2.333333f },
                    { 19, 0.000133f, 0.666667f, 2.333333f },
                    { 20, 0.000107f, 0.666667f, 2.333333f },
                    { 21, 0.000108f, 0.666667f, 2.333333f },
                    { 22, 0.000106f, 0.666667f, 2.333333f },
                    { 23, 9.8E-05f, 0.666667f, 2.333333f },
                    { 24, 0.000121f, 0.666667f, 2.333333f },
                    { 25, 2.8E-05f, 0.7f, 2f },
                    { 26, 9.8E-05f, 0.666667f, 2.333333f },
                    { 27, 0.000126f, 0.666667f, 2.333333f },
                    { 28, 0.000121f, 0.666667f, 2.333333f },
                    { 29, 0.000113f, 0.666667f, 2.333333f },
                    { 30, 4.6E-05f, 0.7f, 2f },
                    { 31, 3.3E-05f, 0.7f, 2f },
                    { 32, 3.8E-05f, 0.7f, 2f },
                    { 33, 1.8E-05f, 0.7f, 2f },
                    { 34, 3E-06f, 0.7f, 2f },
                    { 35, 0.000255f, 0.666667f, 2.333333f },
                    { 36, 0f, 0f, 0f },
                    { 37, 0f, 0f, 0f },
                    { 38, 0f, 0f, 0f },
                    { 39, 0f, 0f, 0f },
                    { 40, 0f, 0f, 0f },
                    { 41, 0f, 0f, 0f },
                    { 42, 0f, 0f, 0f },
                    { 43, 0f, 0f, 0f },
                    { 44, 0f, 0f, 0f },
                    { 45, 0f, 0f, 0f },
                    { 46, 0f, 0f, 0f },
                    { 47, 0f, 0f, 0f },
                    { 48, 9.4E-05f, 0.666667f, 2.333333f },
                    { 49, 0.0001f, 0.666667f, 2.333333f },
                    { 50, 0.000816f, 0.666667f, 2.44f },
                    { 51, 0.000137f, 0.589744f, 1.435897f },
                    { 52, 0.000107f, 0.666667f, 2.333333f },
                    { 53, 0.000121f, 0.666667f, 2.333333f },
                    { 54, 0.001722f, 0f, 0f },
                    { 55, 0.003467f, 0.456522f, 15.73913f }
                });

            migrationBuilder.InsertData(
                table: "CO2s",
                columns: new[] { "Id", "CEF", "UncertaintyLowerLimit", "UncertaintyUpperLimit" },
                values: new object[,]
                {
                    { 1, 2.33286f, 0.077167f, 0.067653f },
                    { 2, 2.693285f, 0.077167f, 0.067653f },
                    { 3, 2.408113f, 0.077167f, 0.067653f },
                    { 4, 2.922093f, 0.03764f, 0.027467f },
                    { 5, 2.693285f, 0.077167f, 0.067653f },
                    { 6, 2.408113f, 0.053911f, 0.053911f },
                    { 7, 1.971522f, 0.034339f, 0.040583f },
                    { 8, 2.253168f, 0.034339f, 0.040583f },
                    { 9, 1.202633f, 0.1f, 0.138614f },
                    { 10, 0.95287f, 0.157009f, 0.168224f },
                    { 11, 1.035387f, 0.056604f, 0.018868f },
                    { 12, 1.551209f, 0.104615f, 0.117949f },
                    { 13, 3.135913f, 0.105607f, 0.11215f },
                    { 14, 3.347347f, 0.149744f, 0.179487f },
                    { 15, 2.19807f, 0.035714f, 0.042857f },
                    { 16, 2.39485f, 0.025175f, 0.040559f },
                    { 17, 2.762032f, 0.030014f, 0.030014f },
                    { 18, 2.119027f, 0.1f, 0.109091f },
                    { 19, 2.839525f, 0.0919f, 0.096573f },
                    { 20, 2.558763f, 0.015299f, 0.025035f },
                    { 21, 2.794563f, 0.075034f, 0.080491f },
                    { 22, 2.606032f, 0.020243f, 0.009447f },
                    { 23, 2.263133f, 0.025974f, 0.053391f },
                    { 24, 3.11096f, 0.024548f, 0.018088f },
                    { 25, 1.752881f, 0.023772f, 0.03962f },
                    { 26, 2.393761f, 0.05457f, 0.040928f },
                    { 27, 3.378748f, 0.095415f, 0.114002f },
                    { 28, 2.946167f, 0.0191f, 0.025921f },
                    { 29, 2.762032f, 0.015007f, 0.015007f },
                    { 30, 2.860187f, 0.082792f, 0.113636f },
                    { 31, 1.879036f, 0.032086f, 0.039216f },
                    { 32, 2.170437f, 0.163194f, 0.197917f },
                    { 33, 0.780754f, 0.15991f, 0.218468f },
                    { 34, 0.845817f, 0.157692f, 0.184615f },
                    { 35, 0.779227f, 0.200654f, 0.31952f },
                    { 36, 0f, 0f, 0f },
                    { 37, 0f, 0f, 0f },
                    { 38, 0f, 0f, 0f },
                    { 39, 0f, 0f, 0f },
                    { 40, 0f, 0f, 0f },
                    { 41, 0f, 0f, 0f },
                    { 42, 0f, 0f, 0f },
                    { 43, 0f, 0f, 0f },
                    { 44, 0f, 0f, 0f },
                    { 45, 0f, 0f, 0f },
                    { 46, 0f, 0f, 0f },
                    { 47, 0f, 0f, 0f },
                    { 48, 2.19807f, 0.035714f, 0.042857f },
                    { 49, 2.39485f, 0.025175f, 0.040559f },
                    { 50, 2.263133f, 0.025974f, 0.053391f },
                    { 51, 2.606032f, 0.020243f, 0.009447f },
                    { 52, 2.558763f, 0.015299f, 0.025035f },
                    { 53, 2.946167f, 0.0191f, 0.025921f },
                    { 54, 1.752881f, 0.023772f, 0.03962f },
                    { 55, 2.113915f, 0.032086f, 0.039216f }
                });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 1, 1, 1 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 2, 2, 2 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 3, 3, 3 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 4, 4, 4 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 5, 5, 5 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 6, 6, 6 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 7, 7, 7 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 8, 8, 8 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 9, 9, 9 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 10, 10, 10 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 11, 11, 11 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 12, 12, 12 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 13, 13, 13 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 14, 14, 14 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 15, 15, 15 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 16, 16, 16 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 17, 17, 17 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 18, 18, 18 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 19, 19, 19 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 20, 20, 20 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 21, 21, 21 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 22, 22, 22 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 23, 23, 23 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 24, 24, 24 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 25, 25, 25 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 26, 26, 26 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 27, 27, 27 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 28, 28, 28 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 29, 29, 29 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 30, 30, 30 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 31, 31, 31 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 32, 32, 32 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 33, 33, 33 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 34, 34, 34 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 35, 35, 35 });

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

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 48, 48, 48 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 49, 49, 49 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 50, 50, 50 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 51, 51, 51 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 52, 52, 52 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 53, 53, 53 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 54, 54, 54 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "CH4Id", "CO2Id", "N2OId" },
                values: new object[] { 55, 55, 55 });

            migrationBuilder.InsertData(
                table: "N2Os",
                columns: new[] { "Id", "CEF", "UncertaintyLowerLimit", "UncertaintyUpperLimit" },
                values: new object[,]
                {
                    { 1, 3.7E-05f, 0.666667f, 2.333333f },
                    { 2, 4.3E-05f, 0.666667f, 2.333333f },
                    { 3, 3.8E-05f, 0.666667f, 2.333333f },
                    { 4, 4.5E-05f, 0.666667f, 2.333333f },
                    { 5, 4.3E-05f, 0.666667f, 2.333333f },
                    { 6, 3.8E-05f, 0.666667f, 2.333333f },
                    { 7, 3.1E-05f, 0.666667f, 2.333333f },
                    { 8, 3.5E-05f, 0.666667f, 2.333333f },
                    { 9, 1.8E-05f, 0.666667f, 2.333333f },
                    { 10, 1.3E-05f, 0.666667f, 2.333333f },
                    { 11, 1.5E-05f, 0.666667f, 2.333333f },
                    { 12, 2.4E-05f, 0.666667f, 2.333333f },
                    { 13, 4.4E-05f, 0.666667f, 2.333333f },
                    { 14, 2.1E-05f, 0.666667f, 2.333333f },
                    { 15, 1.9E-05f, 0.666667f, 2.333333f },
                    { 16, 2E-05f, 0.666667f, 2.333333f },
                    { 17, 2.3E-05f, 0.666667f, 2.333333f },
                    { 18, 1.7E-05f, 0.666667f, 2.333333f },
                    { 19, 2.7E-05f, 0.666667f, 2.333333f },
                    { 20, 2.1E-05f, 0.666667f, 2.333333f },
                    { 21, 2.2E-05f, 0.666667f, 2.333333f },
                    { 22, 2.1E-05f, 0.666667f, 2.333333f },
                    { 23, 2E-05f, 0.666667f, 2.333333f },
                    { 24, 2.4E-05f, 0.666667f, 2.333333f },
                    { 25, 3E-06f, 0.7f, 2f },
                    { 26, 2E-05f, 0.666667f, 2.333333f },
                    { 27, 2.5E-05f, 0.666667f, 2.333333f },
                    { 28, 2.4E-05f, 0.666667f, 2.333333f },
                    { 29, 2.3E-05f, 0.666667f, 2.333333f },
                    { 30, 5E-06f, 0.7f, 2f },
                    { 31, 3E-06f, 0.7f, 2f },
                    { 32, 4E-06f, 0.7f, 2f },
                    { 33, 2E-06f, 0.7f, 2f },
                    { 34, 0f, 0.7f, 2f },
                    { 35, 3.4E-05f, 0.625f, 2.75f },
                    { 36, 0f, 0f, 0f },
                    { 37, 0f, 0f, 0f },
                    { 38, 0f, 0f, 0f },
                    { 39, 0f, 0f, 0f },
                    { 40, 0f, 0f, 0f },
                    { 41, 0f, 0f, 0f },
                    { 42, 0f, 0f, 0f },
                    { 43, 0f, 0f, 0f },
                    { 44, 0f, 0f, 0f },
                    { 45, 0f, 0f, 0f },
                    { 46, 0f, 0f, 0f },
                    { 47, 0f, 0f, 0f },
                    { 48, 1.9E-05f, 0.666667f, 2.333333f },
                    { 49, 2E-05f, 0.666667f, 2.333333f },
                    { 50, 0.000261f, 0.666667f, 2.333333f },
                    { 51, 0.000137f, 0.666667f, 2.076923f },
                    { 52, 2.1E-05f, 0.666667f, 2.333333f },
                    { 53, 2.4E-05f, 0.666667f, 2.333333f },
                    { 54, 6E-06f, 0f, 0f },
                    { 55, 0.000113f, 0.666667f, 24.666668f }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_CH4Id",
                table: "Materials",
                column: "CH4Id");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_CO2Id",
                table: "Materials",
                column: "CO2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_N2OId",
                table: "Materials",
                column: "N2OId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDatas_DeviceId",
                table: "ActivityDatas",
                column: "DeviceId");

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
    }
}
