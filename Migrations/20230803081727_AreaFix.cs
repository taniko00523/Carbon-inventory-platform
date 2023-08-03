using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 碳盤查平台.Migrations
{
    /// <inheritdoc />
    public partial class AreaFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyNO",
                table: "Areas",
                newName: "CompanyNo");

            migrationBuilder.AddColumn<byte>(
                name: "CH4",
                table: "Devices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "CO2",
                table: "Devices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "CreateTime",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeleteTime",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "HFCS",
                table: "Devices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedTime",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "N2O",
                table: "Devices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "NF3",
                table: "Devices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PFCS",
                table: "Devices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SF6",
                table: "Devices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "isDeleted",
                table: "Devices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CompanyNo",
                table: "Areas",
                column: "CompanyNo");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Company_CompanyNo",
                table: "Areas",
                column: "CompanyNo",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Company_CompanyNo",
                table: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_Areas_CompanyNo",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "CH4",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CO2",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "HFCS",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "N2O",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "NF3",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "PFCS",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "SF6",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "CompanyNo",
                table: "Areas",
                newName: "CompanyNO");
        }
    }
}
