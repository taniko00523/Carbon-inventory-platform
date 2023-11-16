using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _00117 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "N2OCEF",
                table: "Materials",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<float>(
                name: "CO2CEF",
                table: "Materials",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<float>(
                name: "CH4CEF",
                table: "Materials",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<float>(
                name: "Num",
                table: "GWPs",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<float>(
                name: "SF6",
                table: "Devices",
                type: "real",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<float>(
                name: "PFCS",
                table: "Devices",
                type: "real",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<float>(
                name: "NF3",
                table: "Devices",
                type: "real",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<float>(
                name: "N2O",
                table: "Devices",
                type: "real",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<float>(
                name: "HFCS",
                table: "Devices",
                type: "real",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<float>(
                name: "CO2",
                table: "Devices",
                type: "real",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<float>(
                name: "CH4",
                table: "Devices",
                type: "real",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "CH4_Emission",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CO2_Emission",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HFCS_Emission",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "N2O_Emission",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NF3_Emission",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PFCS_Emission",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SF6_Emission",
                table: "Devices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CH4_Emission",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CO2_Emission",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "HFCS_Emission",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "N2O_Emission",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "NF3_Emission",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "PFCS_Emission",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "SF6_Emission",
                table: "Devices");

            migrationBuilder.AlterColumn<float>(
                name: "N2OCEF",
                table: "Materials",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "CO2CEF",
                table: "Materials",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "CH4CEF",
                table: "Materials",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Num",
                table: "GWPs",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SF6",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<bool>(
                name: "PFCS",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<bool>(
                name: "NF3",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<bool>(
                name: "N2O",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<bool>(
                name: "HFCS",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<bool>(
                name: "CO2",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<bool>(
                name: "CH4",
                table: "Devices",
                type: "bit",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
