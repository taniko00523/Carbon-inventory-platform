using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 碳盤查平台.Migrations
{
    /// <inheritdoc />
    public partial class AddData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Datas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Num = table.Column<float>(type: "real", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dept = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Correction = table.Column<int>(type: "int", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<int>(type: "int", nullable: false),
                    ModifiedTime = table.Column<int>(type: "int", nullable: false),
                    DeleteTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CEF = table.Column<float>(type: "real", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Provess = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    CO2 = table.Column<byte>(type: "tinyint", nullable: false),
                    CH4 = table.Column<byte>(type: "tinyint", nullable: false),
                    N2O = table.Column<byte>(type: "tinyint", nullable: false),
                    HFCS = table.Column<byte>(type: "tinyint", nullable: false),
                    PFCS = table.Column<byte>(type: "tinyint", nullable: false),
                    SF6 = table.Column<byte>(type: "tinyint", nullable: false),
                    NF3 = table.Column<byte>(type: "tinyint", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<int>(type: "int", nullable: false),
                    ModifiedTime = table.Column<int>(type: "int", nullable: false),
                    DeleteTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_Datas_DataId",
                        column: x => x.DataId,
                        principalTable: "Datas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_AreaId",
                table: "Devices",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DataId",
                table: "Devices",
                column: "DataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MaterialId",
                table: "Devices",
                column: "MaterialId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Datas");

            migrationBuilder.DropTable(
                name: "Materials");
        }
    }
}
