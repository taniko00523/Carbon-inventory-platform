using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class _001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
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
                name: "dataCorrections",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dataCorrections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dataLevels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dataLevels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "GWPs",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Num = table.Column<float>(type: "real", nullable: true),
                    GWP_Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GWPs", x => x.Name);
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
                    CO2CEF = table.Column<float>(type: "real", nullable: false),
                    CO2ULL = table.Column<float>(type: "real", nullable: false),
                    CO2UUL = table.Column<float>(type: "real", nullable: false),
                    CH4CEF = table.Column<float>(type: "real", nullable: false),
                    CH4ULL = table.Column<float>(type: "real", nullable: false),
                    CH4UUL = table.Column<float>(type: "real", nullable: false),
                    N2OCEF = table.Column<float>(type: "real", nullable: false),
                    N2OULL = table.Column<float>(type: "real", nullable: false),
                    N2OUUL = table.Column<float>(type: "real", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "refrigerants",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Num = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refrigerants", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PostalCode = table.Column<int>(type: "int", nullable: false),
                    UniqueCode = table.Column<int>(type: "int", nullable: false),
                    FactorCode = table.Column<int>(type: "int", nullable: false),
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
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmissionPattern = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Provess = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CO2_Emission = table.Column<bool>(type: "bit", nullable: false),
                    CH4_Emission = table.Column<bool>(type: "bit", nullable: false),
                    N2O_Emission = table.Column<bool>(type: "bit", nullable: false),
                    HFCS_Emission = table.Column<bool>(type: "bit", nullable: false),
                    PFCS_Emission = table.Column<bool>(type: "bit", nullable: false),
                    SF6_Emission = table.Column<bool>(type: "bit", nullable: false),
                    NF3_Emission = table.Column<bool>(type: "bit", nullable: false),
                    CO2 = table.Column<float>(type: "real", nullable: false),
                    CH4 = table.Column<float>(type: "real", nullable: false),
                    N2O = table.Column<float>(type: "real", nullable: false),
                    HFCS = table.Column<float>(type: "real", nullable: false),
                    PFCS = table.Column<float>(type: "real", nullable: false),
                    SF6 = table.Column<float>(type: "real", nullable: false),
                    NF3 = table.Column<float>(type: "real", nullable: false),
                    Emissions = table.Column<float>(type: "real", nullable: false),
                    UUL = table.Column<float>(type: "real", nullable: false),
                    ULL = table.Column<float>(type: "real", nullable: false),
                    Num = table.Column<float>(type: "real", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dept = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Correction = table.Column<int>(type: "int", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaterialId = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Devices_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "emissions",
                columns: table => new
                {
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scope1_CO2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CO2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CH4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    N2O = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HFCS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PFCS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SF6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NF3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage1_CO2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage1_CH4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage1_N2O = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage1_HFCS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage1_PFCS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage1_SF6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage1_NF3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage2_CO2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage2_CH4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage2_N2O = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage2_HFCS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage2_PFCS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage2_SF6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage2_NF3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    non_move = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    move = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    process = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    escape = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage_nonMove = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage_Move = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage_Process = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage_Escape = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage_Scope1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    All = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage_Scope2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cal_all = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    percentage_CalAll = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emissions", x => x.AreaId);
                    table.ForeignKey(
                        name: "FK_emissions_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[,]
                {
                    { "七氟丙烷", 2022, 3600f },
                    { "CH4", 2022, 27.9f },
                    { "CO2", 2022, 1f },
                    { "N2O", 2022, 273f },
                    { "NF3", 2022, 17400f },
                    { "R-134A", 2022, 1530f },
                    { "R-22", 2022, 1960f },
                    { "R-23", 2022, 14600f },
                    { "R-32", 2022, 771f },
                    { "R-410A", 2022, 2256f },
                    { "R-600A", 2022, 0f },
                    { "SF6", 2022, 24300f }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "EmissionPattern", "N2OCEF", "N2OULL", "N2OUUL", "Name", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 20, 0.000107f, 0.666667f, 2.333333f, 2.558763f, -0.015299f, 0.025035f, "固定", 2.1E-05f, 0.666667f, 2.333333f, "煤油", "類別1", "L", 0 },
                    { 22, 0.000106f, 0.666667f, 2.333333f, 2.606032f, -0.020243f, 0.009447f, "固定", 2.1E-05f, 0.666667f, 2.333333f, "柴油", "類別1", "L", 0 },
                    { 23, 9.8E-05f, 0.666667f, 2.333333f, 2.263133f, -0.025974f, 0.053391f, "固定", 2E-05f, 0.666667f, 2.333333f, "車用汽油", "類別1", "L", 0 },
                    { 25, 2.8E-05f, 0.7f, 2f, 1.752881f, -0.023772f, 0.03962f, "固定", 3E-06f, 0.7f, 2f, "液化石油氣", "類別1", "L", 0 },
                    { 28, 0.000121f, 0.666667f, 2.333333f, 2.946167f, -0.0191f, 0.025921f, "固定", 2.4E-05f, 0.666667f, 2.333333f, "潤滑油", "類別1", "L", 0 },
                    { 29, 0.000113f, 0.666667f, 2.333333f, 2.762032f, -0.015007f, 0.015007f, "固定", 2.3E-05f, 0.666667f, 2.333333f, "其他油品", "類別1", "L", 0 },
                    { 30, 4.6E-05f, 0.7f, 2f, 2.860187f, -0.082792f, 0.113636f, "固定", 5E-06f, 0.7f, 2f, "乙烷", "類別1", "L", 0 },
                    { 50, 0.000816f, 0.666667f, 2.44f, 2.263133f, -0.025974f, 0.053391f, "移動", 0.000261f, 0.666667f, 2.333333f, "車用汽油", "類別1", "L", 0 },
                    { 51, 0.000137f, 0.589744f, 1.435897f, 2.606032f, -0.020243f, 0.009447f, "移動", 0.000137f, 0.666667f, 2.076923f, "柴油", "類別1", "L", 0 },
                    { 52, 0.000107f, 0.666667f, 2.333333f, 2.558763f, -0.015299f, 0.025035f, "移動", 2.1E-05f, 0.666667f, 2.333333f, "煤油", "類別1", "L", 0 },
                    { 53, 0.000121f, 0.666667f, 2.333333f, 2.946167f, -0.0191f, 0.025921f, "移動", 2.4E-05f, 0.666667f, 2.333333f, "潤滑油", "類別1", "L", 0 },
                    { 54, 0.001722f, 0f, 0f, 1.752881f, -0.023772f, 0.03962f, "移動", 6E-06f, 0f, 0f, "液化石油氣", "類別1", "L", 0 },
                    { 55, 0.003467f, 0.456522f, 15.73913f, 2.113915f, -0.032086f, 0.039216f, "移動", 0.000113f, 0.666667f, 24.666668f, "液化天然氣", "類別1", "M3", 0 },
                    { 56, 0f, 0f, 0f, 0.495f, -0.07f, 0f, "其他電力", 0f, 0f, 0f, "外購電力", "類別2", "", 111 }
                });

            migrationBuilder.InsertData(
                table: "dataCorrections",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "有外部校正或多組數據佐證者" },
                    { 2, "有內部校正或經過會計簽證等證明者" },
                    { 3, "未進行儀器校正或未進行紀錄彙整者" }
                });

            migrationBuilder.InsertData(
                table: "dataLevels",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "連續監測" },
                    { 2, "定期/間歇量測" },
                    { 3, "自行/財務推估" }
                });

            migrationBuilder.InsertData(
                table: "refrigerants",
                columns: new[] { "Name", "Num" },
                values: new object[,]
                {
                    { "工業冷凍、冷藏裝備，包括食品加工及冷藏", 0.15999999642372131 },
                    { "中、大型冷凍、冷藏裝備", 0.20000000298023224 },
                    { "交通用冷凍、冷藏裝備", 0.33000001311302185 },
                    { "冰水機", 0.090000003576278687 },
                    { "住宅及商業建築冷氣機", 0.029999999329447746 },
                    { "家用冷凍、冷藏裝備", 0.0030000000260770321 },
                    { "移動式空氣清靜機", 0.20000000298023224 },
                    { "獨立商用冷凍、冷藏裝備", 0.054999999701976776 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CompanyId",
                table: "Areas",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_AreaId",
                table: "Devices",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MaterialId",
                table: "Devices",
                column: "MaterialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "dataCorrections");

            migrationBuilder.DropTable(
                name: "dataLevels");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "emissions");

            migrationBuilder.DropTable(
                name: "GWPs");

            migrationBuilder.DropTable(
                name: "refrigerants");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
