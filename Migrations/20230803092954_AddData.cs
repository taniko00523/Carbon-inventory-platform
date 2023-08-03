using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 碳盤查平台.Migrations
{
    /// <inheritdoc />
    public partial class AddData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Company_CompanyId",
                table: "Areas");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "AreaNo",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "CompanyNo",
                table: "Company");

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "Companies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Companies_CompanyId",
                table: "Areas",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Companies_CompanyId",
                table: "Areas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company");

            migrationBuilder.AddColumn<int>(
                name: "AreaNo",
                table: "Areas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyNo",
                table: "Company",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaNo = table.Column<int>(type: "int", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CH4 = table.Column<byte>(type: "tinyint", nullable: false),
                    CO2 = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<int>(type: "int", nullable: false),
                    DeleteTime = table.Column<int>(type: "int", nullable: false),
                    DeviceNo = table.Column<int>(type: "int", nullable: false),
                    HFCS = table.Column<byte>(type: "tinyint", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaterialNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifiedTime = table.Column<int>(type: "int", nullable: false),
                    N2O = table.Column<byte>(type: "tinyint", nullable: false),
                    NF3 = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PFCS = table.Column<byte>(type: "tinyint", nullable: false),
                    Provess = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SF6 = table.Column<byte>(type: "tinyint", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Company_CompanyId",
                table: "Areas",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
