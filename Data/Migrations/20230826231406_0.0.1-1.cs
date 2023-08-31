using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Num = table.Column<float>(type: "real", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dept = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Correction = table.Column<int>(type: "int", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityDatas", x => x.Id);
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
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PostalCode = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    District = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmissionPattern = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CH4Id = table.Column<int>(type: "int", nullable: false),
                    CO2Id = table.Column<int>(type: "int", nullable: false),
                    N2OId = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_CH4s_CH4Id",
                        column: x => x.CH4Id,
                        principalTable: "CH4s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Materials_CO2s_CO2Id",
                        column: x => x.CO2Id,
                        principalTable: "CO2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Materials_N2Os_N2OId",
                        column: x => x.N2OId,
                        principalTable: "N2Os",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Provess = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActivityDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    CO2 = table.Column<byte>(type: "tinyint", nullable: false),
                    CH4 = table.Column<byte>(type: "tinyint", nullable: false),
                    N2O = table.Column<byte>(type: "tinyint", nullable: false),
                    HFCS = table.Column<byte>(type: "tinyint", nullable: false),
                    PFCS = table.Column<byte>(type: "tinyint", nullable: false),
                    SF6 = table.Column<byte>(type: "tinyint", nullable: false),
                    NF3 = table.Column<byte>(type: "tinyint", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_ActivityDatas_ActivityDataId",
                        column: x => x.ActivityDataId,
                        principalTable: "ActivityDatas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Devices_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaDevice",
                columns: table => new
                {
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaDevice", x => new { x.AreaId, x.AreasId });
                    table.ForeignKey(
                        name: "FK_AreaDevice_Areas_AreasId",
                        column: x => x.AreasId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaDevice_Devices_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4Id", "CO2Id", "EmissionPattern", "N2OId", "Name", "Scope", "Unit" },
                values: new object[,]
                {
                    { 1, 1, 1, "固定", 1, "自產煤", "範疇1", "Kg" },
                    { 2, 2, 2, "固定", 2, "原料煤", "範疇1", "Kg" },
                    { 3, 3, 3, "固定", 3, "燃料煤", "範疇1", "Kg" },
                    { 4, 4, 4, "固定", 4, "無煙煤", "範疇1", "Kg" },
                    { 5, 5, 5, "固定", 5, "焦煤", "範疇1", "Kg" },
                    { 6, 6, 6, "固定", 6, "煙煤", "範疇1", "Kg" },
                    { 7, 7, 7, "固定", 7, "亞煙煤(發電)", "範疇1", "Kg" },
                    { 8, 8, 8, "固定", 8, "亞煙煤(其他)", "範疇1", "Kg" },
                    { 9, 9, 9, "固定", 9, "褐煤", "範疇1", "Kg" },
                    { 10, 10, 10, "固定", 10, "油頁岩", "範疇1", "Kg" },
                    { 11, 11, 11, "固定", 11, "泥煤", "範疇1", "Kg" },
                    { 12, 12, 12, "固定", 12, "煤球", "範疇1", "Kg" },
                    { 13, 13, 13, "固定", 13, "焦炭", "範疇1", "Kg" },
                    { 14, 14, 14, "固定", 14, "石油焦", "範疇1", "Kg" },
                    { 15, 15, 15, "固定", 15, "航空汽油", "範疇1", "L" },
                    { 16, 16, 16, "固定", 16, "航空燃油", "範疇1", "L" },
                    { 17, 17, 17, "固定", 17, "原油", "範疇1", "L" },
                    { 18, 18, 18, "固定", 18, "奧里油", "範疇1", "Kg" },
                    { 19, 19, 19, "固定", 19, "天然氣凝結油", "範疇1", "M3" },
                    { 20, 20, 20, "固定", 20, "煤油", "範疇1", "L" },
                    { 21, 21, 21, "固定", 21, "頁岩油", "範疇1", "Kg" },
                    { 22, 22, 22, "固定", 22, "柴油", "範疇1", "L" },
                    { 23, 23, 23, "固定", 23, "車用汽油", "範疇1", "L" },
                    { 24, 24, 24, "固定", 24, "蒸餘油 (燃料油)", "範疇1", "L" },
                    { 25, 25, 25, "固定", 25, "液化石油氣", "範疇1", "L" },
                    { 26, 26, 26, "固定", 26, "石油腦", "範疇1", "L" },
                    { 27, 27, 27, "固定", 27, "柏油", "範疇1", "L" },
                    { 28, 28, 28, "固定", 28, "潤滑油", "範疇1", "L" },
                    { 29, 29, 29, "固定", 29, "其他油品", "範疇1", "L" },
                    { 30, 30, 30, "固定", 30, "乙烷", "範疇1", "L" },
                    { 31, 31, 31, "固定", 31, "天然氣", "範疇1", "M3" },
                    { 32, 32, 32, "固定", 32, "煉油氣", "範疇1", "M3" },
                    { 33, 33, 33, "固定", 33, "焦爐氣", "範疇1", "M3" },
                    { 34, 34, 34, "固定", 34, "高爐氣", "範疇1", "M3" },
                    { 35, 35, 35, "固定", 35, "一般廢棄物", "範疇1", "Kg" },
                    { 36, 36, 36, "固定", 36, "事業廢棄物", "範疇1", "" },
                    { 37, 37, 37, "固定", 37, "其他非化石燃料", "範疇1", "" },
                    { 38, 38, 38, "固定", 38, "木頭－固態", "範疇1", "" },
                    { 39, 39, 39, "固定", 39, "黑液", "範疇1", "" },
                    { 40, 40, 40, "固定", 40, "木炭", "範疇1", "" },
                    { 41, 41, 41, "固定", 41, "其他固體生質燃料", "範疇1", "" },
                    { 42, 42, 42, "固定", 42, "生質汽油", "範疇1", "" },
                    { 43, 43, 43, "固定", 43, "生質柴油", "範疇1", "" },
                    { 44, 44, 44, "固定", 44, "其他液態生質燃料", "範疇1", "" },
                    { 45, 45, 45, "固定", 45, "掩埋場沼氣", "範疇1", "" },
                    { 46, 46, 46, "固定", 46, "污泥沼氣", "範疇1", "" },
                    { 47, 47, 47, "固定", 47, "其他氣態生質燃料", "範疇1", "" },
                    { 48, 48, 48, "移動", 48, "航空汽油", "範疇1", "L" },
                    { 49, 49, 49, "移動", 49, "航空燃油", "範疇1", "L" },
                    { 50, 50, 50, "移動", 50, "車用汽油", "範疇1", "L" },
                    { 51, 51, 51, "移動", 51, "柴油", "範疇1", "L" },
                    { 52, 52, 52, "移動", 52, "煤油", "範疇1", "L" },
                    { 53, 53, 53, "移動", 53, "潤滑油", "範疇1", "L" },
                    { 54, 54, 54, "移動", 54, "液化石油氣", "範疇1", "L" },
                    { 55, 55, 55, "移動", 55, "液化天然氣", "範疇1", "M3" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaDevice_AreasId",
                table: "AreaDevice",
                column: "AreasId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CompanyId",
                table: "Areas",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ActivityDataId",
                table: "Devices",
                column: "ActivityDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MaterialId",
                table: "Devices",
                column: "MaterialId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaDevice");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "ActivityDatas");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "CH4s");

            migrationBuilder.DropTable(
                name: "CO2s");

            migrationBuilder.DropTable(
                name: "N2Os");
        }
    }
}
