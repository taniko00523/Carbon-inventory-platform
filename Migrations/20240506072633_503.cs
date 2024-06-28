using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _503 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameRemark",
                table: "Devices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Areas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Areas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CO2CEF", "Year" },
                values: new object[] { 0.555m, 94 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CO2CEF", "Year" },
                values: new object[] { 0.5625m, 95 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "Name", "Scope", "Year" },
                values: new object[] { 0m, 0.533m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", "外購電力", "類別二", 107 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "Name", "Scope", "Year" },
                values: new object[] { 0.509m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", "外購電力", "類別二", 108 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "CEF_Correction", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "Name", "Scope", "Year" },
                values: new object[] { 3, 0.502m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", "外購電力", "類別二", 109 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CEF_Correction", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "Name", "Scope", "Year" },
                values: new object[] { 3, 0.509m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", "外購電力", "類別二", 110 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "Name", "Scope", "Year" },
                values: new object[] { 0.495m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, "外購電力", "類別二", 111 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "Name", "Scope", "Year" },
                values: new object[] { 0.494m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, "外購電力", "類別二", 112 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "CH4CEF", "HFCSCEF", "Name" },
                values: new object[] { 0.0038250000m, 0m, "廢水處理" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "CO2CEF", "HFCSCEF", "Name" },
                values: new object[] { 1m, 0m, "二氧化碳" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "CEF_Correction", "CO2CEF", "EmissionPattern", "HFCSCEF", "Name" },
                values: new object[] { 1, 3.3841653850m, "製程", 0m, "乙炔" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "CEF_Correction", "CO2CEF", "EmissionPattern", "HFCSCEF", "Name" },
                values: new object[] { 1, 3.6666666666m, "製程", 0m, "焊條" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.003000m, "冰箱" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.003000m, "飲水機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.055000m, "商用冰箱" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.200000m, "中、大型冰箱" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.330000m, "低溫冷凍車" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "CEF_Correction", "CO2CEF", "EmissionPattern", "HFCSCEF", "Name" },
                values: new object[] { 3, 0m, "逸散", 0.160000m, "乾燥機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "Name", "Scope", "Year" },
                values: new object[] { 0m, 0m, 0m, 0m, 0m, "逸散", 0.160000m, "工業冷藏、冷凍", "類別一", 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "CO2CEF", "EmissionPattern", "HFCSCEF", "Name" },
                values: new object[] { 0m, "逸散", 0.160000m, "食品加工冷藏、冷凍" });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CEF_Correction", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "HFCSULL", "HFCSUUL", "N2OCEF", "N2OULL", "N2OUUL", "NF3CEF", "NF3ULL", "NF3UUL", "Name", "PFCSCEF", "PFCSULL", "PFCSUUL", "SF6CEF", "SF6ULL", "SF6UUL", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 46, 3, 0m, 0m, 0m, 0.558m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 96 },
                    { 47, 3, 0m, 0m, 0m, 0.555m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 97 },
                    { 48, 3, 0m, 0m, 0m, 0.543m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 98 },
                    { 49, 3, 0m, 0m, 0m, 0.534m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 99 },
                    { 50, 3, 0m, 0m, 0m, 0.534m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 100 },
                    { 51, 3, 0m, 0m, 0m, 0.529m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 101 },
                    { 52, 3, 0m, 0m, 0m, 0.519m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 102 },
                    { 53, 3, 0m, 0m, 0m, 0.518m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 103 },
                    { 54, 3, 0m, 0m, 0m, 0.525m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 104 },
                    { 55, 3, 0m, 0m, 0m, 0.530m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 105 },
                    { 56, 3, 0m, 0m, 0m, 0.554m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 106 },
                    { 75, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.090000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "冰水主機", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 76, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.030000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "冷氣機", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 77, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.200000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "車用空調", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 78, 1, 0m, 0m, 0m, 3.0260000000m, 0m, 0m, 0m, 0m, "製程", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "丁烷", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 79, 3, 0m, 0m, 0m, 0.025m, 0m, 0m, 0m, 0m, "製程", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "WD40", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DropColumn(
                name: "NameRemark",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Devices");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Areas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Areas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CO2CEF", "Year" },
                values: new object[] { 0.495m, 112 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CO2CEF", "Year" },
                values: new object[] { 0.494m, 113 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "CH4CEF", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "Name", "Scope", "Year" },
                values: new object[] { 0.0038250000m, 0m, 0m, 0m, 0m, 0m, "逸散", "廢水處理", "類別一", 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "Name", "Scope", "Year" },
                values: new object[] { 1m, 0m, 0m, 0m, 0m, "逸散", "二氧化碳", "類別一", 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "CEF_Correction", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "Name", "Scope", "Year" },
                values: new object[] { 1, 3.3841653850m, 0m, 0m, 0m, 0m, "製程", "乙炔", "類別一", 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CEF_Correction", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "Name", "Scope", "Year" },
                values: new object[] { 1, 3.6666666666m, 0m, 0m, 0m, 0m, "製程", "焊條", "類別一", 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "Name", "Scope", "Year" },
                values: new object[] { 0m, 0m, 0m, 0m, 0m, "逸散", 0.003000m, "冰箱", "類別一", 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "Name", "Scope", "Year" },
                values: new object[] { 0m, 0m, 0m, 0m, 0m, "逸散", 0.003000m, "飲水機", "類別一", 0 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "CH4CEF", "HFCSCEF", "Name" },
                values: new object[] { 0m, 0.055000m, "商用冰箱" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "CO2CEF", "HFCSCEF", "Name" },
                values: new object[] { 0m, 0.200000m, "中、大型冰箱" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "CEF_Correction", "CO2CEF", "EmissionPattern", "HFCSCEF", "Name" },
                values: new object[] { 3, 0m, "逸散", 0.330000m, "低溫冷凍車" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "CEF_Correction", "CO2CEF", "EmissionPattern", "HFCSCEF", "Name" },
                values: new object[] { 3, 0m, "逸散", 0.160000m, "乾燥機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.160000m, "工業冷藏、冷凍" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.160000m, "食品加工冷藏、冷凍" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.090000m, "冰水主機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.030000m, "冷氣機" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "HFCSCEF", "Name" },
                values: new object[] { 0.200000m, "車用空調" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "CEF_Correction", "CO2CEF", "EmissionPattern", "HFCSCEF", "Name" },
                values: new object[] { 1, 3.0260000000m, "製程", 0m, "丁烷" });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "Name", "Scope", "Year" },
                values: new object[] { 0.509m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, "外購電力", "類別二", 110 });

            migrationBuilder.UpdateData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "CO2CEF", "EmissionPattern", "HFCSCEF", "Name" },
                values: new object[] { 0.025m, "製程", 0m, "WD40" });
        }
    }
}
