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
                    Information = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "deviceDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmissionPattern = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Correction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deviceDatas", x => x.Id);
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
                    CO2CEF = table.Column<double>(type: "float", nullable: false),
                    CO2ULL = table.Column<float>(type: "real", nullable: false),
                    CO2UUL = table.Column<float>(type: "real", nullable: false),
                    CH4CEF = table.Column<double>(type: "float", nullable: false),
                    CH4ULL = table.Column<float>(type: "real", nullable: false),
                    CH4UUL = table.Column<float>(type: "real", nullable: false),
                    N2OCEF = table.Column<double>(type: "float", nullable: false),
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
                    count_UUL = table.Column<float>(type: "real", nullable: false),
                    count_ULL = table.Column<float>(type: "real", nullable: false),
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
                    percentage_CalAll = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    no1_Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    no2_Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    no3_Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    avg_Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    all_Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ULL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UUL = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    { "R-407C", 2022, 1908f },
                    { "R-410A", 2022, 2256f },
                    { "R-600A", 2022, 0.006f },
                    { "SF6", 2022, 24300f }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "EmissionPattern", "N2OCEF", "N2OULL", "N2OUUL", "Name", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 1, 2.4660252E-05, 0.7f, 2f, 2.3328598392000002, 0.077167f, 0.067653f, "固定", 3.6990377999999997E-05, 0.666667f, 2.333333f, "自產煤", "類別1", "Kg", 0 },
                    { 2, 2.847024E-05, 0.7f, 2f, 2.6932847039999999, 0.077167f, 0.067653f, "固定", 4.2705359999999997E-05, 0.666667f, 2.333333f, "原料煤", "類別1", "Kg", 0 },
                    { 3, 2.5455744000000001E-05, 0.7f, 2f, 2.4081133823999998, 0.077167f, 0.067653f, "固定", 3.8183616000000002E-05, 0.666667f, 2.333333f, "燃料煤", "類別1", "Kg", 0 },
                    { 4, 2.9726279999999999E-05, 0.7f, 2f, 2.922093324, 0.03764f, 0.027467f, "固定", 4.4589419999999998E-05, 0.666667f, 2.333333f, "無煙煤", "類別1", "Kg", 0 },
                    { 5, 2.847024E-05, 0.7f, 2f, 2.6932847039999999, 0.077167f, 0.067653f, "固定", 4.2705359999999997E-05, 0.666667f, 2.333333f, "焦煤", "類別1", "Kg", 0 },
                    { 6, 2.5455744000000001E-05, 0.7f, 2f, 2.4081133823999998, 0.053911f, 0.053911f, "固定", 3.8183616000000002E-05, 0.666667f, 2.333333f, "煙煤", "類別1", "Kg", 0 },
                    { 7, 2.0515320000000001E-05, 0.7f, 2f, 1.971522252, 0.034339f, 0.040583f, "固定", 3.0772979999999997E-05, 0.666667f, 2.333333f, "亞煙煤(發電)", "類別1", "Kg", 0 },
                    { 8, 2.3446080000000001E-05, 0.7f, 2f, 2.2531682879999999, 0.034339f, 0.040583f, "固定", 3.5169120000000003E-05, 0.666667f, 2.333333f, "亞煙煤(其他)", "類別1", "Kg", 0 },
                    { 9, 1.19072592E-05, 0.7f, 2f, 1.2026331792, 0.1f, 0.138614f, "固定", 1.78608888E-05, 0.666667f, 2.333333f, "褐煤", "類別1", "Kg", 0 },
                    { 10, 8.9053236000000002E-06, 0.7f, 2f, 0.95286962519999996, 0.157009f, 0.168224f, "固定", 1.33579854E-05, 0.666667f, 2.333333f, "油頁岩", "類別1", "Kg", 0 },
                    { 11, 9.7678044000000006E-06, 0.7f, 2f, 1.0353872663999999, 0.056604f, 0.018868f, "固定", 1.46517066E-05, 0.666667f, 2.333333f, "泥煤", "類別1", "Kg", 0 },
                    { 12, 1.5909840000000001E-05, 0.7f, 2f, 1.5512094000000001, 0.104615f, 0.117949f, "固定", 2.386476E-05, 0.666667f, 2.333333f, "煤球", "類別1", "Kg", 0 },
                    { 13, 2.93076E-05, 0.7f, 2f, 3.1359132000000001, 0.105607f, 0.11215f, "固定", 4.3961400000000002E-05, 0.666667f, 2.333333f, "焦炭", "類別1", "Kg", 0 },
                    { 14, 0.00010299528000000001, 0.666667f, 2.333333f, 3.3473465999999998, 0.149744f, 0.179487f, "固定", 2.0599056E-05, 0.666667f, 2.333333f, "石油焦", "類別1", "Kg", 0 },
                    { 15, 9.4203000000000006E-05, 0.666667f, 2.333333f, 2.19807, 0.035714f, 0.042857f, "固定", 1.8840600000000001E-05, 0.666667f, 2.333333f, "航空汽油", "類別1", "L", 0 },
                    { 16, 0.00010048319999999999, 0.666667f, 2.333333f, 2.3948496000000001, 0.025175f, 0.040559f, "固定", 2.0096639999999999E-05, 0.666667f, 2.333333f, "航空燃油", "類別1", "L", 0 },
                    { 17, 0.0001130436, 0.666667f, 2.333333f, 2.7620319599999998, 0.030014f, 0.030014f, "固定", 2.2608720000000001E-05, 0.666667f, 2.333333f, "原油", "類別1", "L", 0 },
                    { 18, 8.2559509200000002E-05, 0.666667f, 2.333333f, 2.1190274028, 0.1f, 0.109091f, "固定", 1.6511901839999998E-05, 0.666667f, 2.333333f, "奧里油", "類別1", "Kg", 0 },
                    { 19, 0.00013268806559999999, 0.666667f, 2.333333f, 2.8395246038400002, 0.0919f, 0.096573f, "固定", 2.6537613120000002E-05, 0.666667f, 2.333333f, "天然氣凝結油", "類別1", "M3", 0 },
                    { 20, 0.00010676339999999999, 0.666667f, 2.333333f, 2.5587628200000001, 0.015299f, 0.025035f, "固定", 2.1352679999999998E-05, 0.666667f, 2.333333f, "煤油", "類別1", "L", 0 },
                    { 21, 0.00010799431920000001, 0.666667f, 2.333333f, 2.79456255864, 0.075034f, 0.080491f, "固定", 2.1598863839999998E-05, 0.666667f, 2.333333f, "頁岩油", "類別1", "Kg", 0 },
                    { 22, 0.00010550736, 0.666667f, 2.333333f, 2.606031792, 0.020243f, 0.009447f, "固定", 2.1101472000000001E-05, 0.666667f, 2.333333f, "柴油", "類別1", "L", 0 },
                    { 23, 9.7971119999999996E-05, 0.666667f, 2.333333f, 2.2631328719999999, 0.025974f, 0.053391f, "固定", 1.9594223999999998E-05, 0.666667f, 2.333333f, "車用汽油", "類別1", "L", 0 },
                    { 24, 0.00012057984, 0.666667f, 2.333333f, 3.110959872, 0.024548f, 0.018088f, "固定", 2.4115968E-05, 0.666667f, 2.333333f, "蒸餘油 (燃料油)", "類別1", "L", 0 },
                    { 25, 2.7779418E-05, 0.7f, 2f, 1.7528812758000001, 0.023772f, 0.03962f, "固定", 2.7779418000000001E-06, 0.7f, 2f, "液化石油氣", "類別1", "L", 0 },
                    { 26, 9.7971119999999996E-05, 0.666667f, 2.333333f, 2.393761032, 0.05457f, 0.040928f, "固定", 1.9594223999999998E-05, 0.666667f, 2.333333f, "石油腦", "類別1", "L", 0 },
                    { 27, 0.000125604, 0.666667f, 2.333333f, 3.3787476000000001, 0.095415f, 0.114002f, "固定", 2.5120799999999998E-05, 0.666667f, 2.333333f, "柏油", "類別1", "L", 0 },
                    { 28, 0.00012057984, 0.666667f, 2.333333f, 2.946167424, 0.0191f, 0.025921f, "固定", 2.4115968E-05, 0.666667f, 2.333333f, "潤滑油", "類別1", "L", 0 },
                    { 29, 0.0001130436, 0.666667f, 2.333333f, 2.7620319599999998, 0.015007f, 0.015007f, "固定", 2.2608720000000001E-05, 0.666667f, 2.333333f, "其他油品", "類別1", "L", 0 },
                    { 30, 4.6431611999999997E-05, 0.7f, 2f, 2.8601872992000001, 0.082792f, 0.113636f, "固定", 4.6431611999999997E-06, 0.7f, 2f, "乙烷", "類別1", "L", 0 },
                    { 31, 3.3494400000000002E-05, 0.7f, 2f, 1.87903584, 0.032086f, 0.039216f, "固定", 3.3494399999999999E-06, 0.7f, 2f, "天然氣", "類別1", "M3", 0 },
                    { 32, 3.7681200000000001E-05, 0.7f, 2f, 2.1704371199999999, 0.163194f, 0.197917f, "固定", 3.7681200000000001E-06, 0.7f, 2f, "煉油氣", "類別1", "M3", 0 },
                    { 33, 1.7584560000000002E-05, 0.7f, 2f, 0.78075446400000004, 0.15991f, 0.218468f, "固定", 1.758456E-06, 0.7f, 2f, "焦爐氣", "類別1", "M3", 0 },
                    { 34, 3.2531436E-06, 0.7f, 2f, 0.84581733599999998, 0.157692f, 0.184615f, "固定", 3.2531435999999998E-07, 0.7f, 2f, "高爐氣", "類別1", "M3", 0 },
                    { 35, 0.00025492713443999999, 0.666667f, 2.333333f, 0.77922727427159999, 0.200654f, 0.31952f, "固定", 3.3990284592000002E-05, 0.625f, 2.75f, "一般廢棄物", "類別1", "Kg", 0 },
                    { 36, 9.4203000000000006E-05, 0.666667f, 2.333333f, 2.19807, 0.035714f, 0.042857f, "移動", 1.8840600000000001E-05, 0.666667f, 2.333333f, "航空汽油", "類別1", "L", 0 },
                    { 37, 0.00010048319999999999, 0.666667f, 2.333333f, 2.3948496000000001, 0.025175f, 0.040559f, "移動", 2.0096639999999999E-05, 0.666667f, 2.333333f, "航空燃油", "類別1", "L", 0 },
                    { 38, 0.00081642599999999998, 0.666667f, 2.44f, 2.2631328719999999, 0.025974f, 0.053391f, "移動", 0.00026125631999999999, 0.666667f, 2.333333f, "車用汽油", "類別1", "L", 0 },
                    { 39, 0.00013715956800000001, 0.589744f, 1.435897f, 2.606031792, 0.020243f, 0.009447f, "移動", 0.00013715956800000001, 0.666667f, 2.076923f, "柴油", "類別1", "L", 0 },
                    { 40, 0.00010676339999999999, 0.666667f, 2.333333f, 2.5587628200000001, 0.015299f, 0.025035f, "移動", 2.1352679999999998E-05, 0.666667f, 2.333333f, "煤油", "類別1", "L", 0 },
                    { 41, 0.00012057984, 0.666667f, 2.333333f, 2.946167424, 0.0191f, 0.025921f, "移動", 2.4115968E-05, 0.666667f, 2.333333f, "潤滑油", "類別1", "L", 0 },
                    { 42, 0.001722323916, 0f, 0f, 1.7528812758000001, 0.023772f, 0.03962f, "移動", 5.5558836000000003E-06, 0f, 0f, "液化石油氣", "類別1", "L", 0 },
                    { 43, 0.0034666704, 0.456522f, 15.73913f, 2.1139153199999998, 0.032086f, 0.039216f, "移動", 0.0001130436, 0.666667f, 24.666668f, "液化天然氣", "類別1", "M3", 0 },
                    { 56, 0.0, 0f, 0f, 0.495, -0.07f, 0.07f, "外購電力", 0.0, 0f, 0f, "外購電力", "類別2", "", 111 },
                    { 57, 0.002546062, 0f, 0f, 0.0, 0f, 0f, "逸散", 0.0, 0f, 0f, "廢水處理", "類別1", "", 0 },
                    { 58, 0.0, 0f, 0f, 1.0, 0f, 0f, "逸散", 0.0, 0f, 0f, "二氧化碳", "類別1", "", 0 },
                    { 59, 0.0, 0f, 0f, 3.3841653850000002, 0f, 0f, "製程", 0.0, 0f, 0f, "乙炔", "類別1", "", 0 },
                    { 60, 0.0, 0f, 0f, 3.6666666665999998, 0f, 0f, "製程", 0.0, 0f, 0f, "焊條", "類別1", "", 0 }
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
                table: "deviceDatas",
                columns: new[] { "Id", "Correction", "EmissionPattern", "Level", "Material", "Name", "Scope" },
                values: new object[,]
                {
                    { 1, 3, "逸散", 3, "R-410A", "冷氣機", "類別一" },
                    { 2, 3, "逸散", 3, "R-134A", "冰水主機", "類別一" },
                    { 3, 3, "逸散", 3, "R-134A", "冰箱", "類別一" },
                    { 4, 3, "逸散", 3, "R-134A", "飲水機", "類別一" },
                    { 5, 3, "逸散", 3, "R-134A", "乾燥機", "類別一" },
                    { 6, 3, "逸散", 3, "R-134A", "車用空調", "類別一" },
                    { 7, 3, "逸散", 3, "R-134A", "工業冷媒", "類別一" },
                    { 8, 2, "固定", 2, "柴油", "緊急發電機", "類別一" },
                    { 9, 2, "固定", 2, "液化石油氣", "廚房", "類別一" },
                    { 10, 2, "移動", 2, "車用汽油", "公務車", "類別一" },
                    { 11, 2, "移動", 2, "柴油", "堆高機", "類別一" },
                    { 12, 3, "逸散", 3, "二氧化碳", "CO2滅火器", "類別一" },
                    { 13, 3, "逸散", 3, "二氧化碳", "二氧化碳", "類別一" },
                    { 14, 3, "逸散", 3, "二氧化碳", "WD40", "類別一" },
                    { 15, 3, "逸散", 3, "海龍1211", "海龍滅火器", "類別一" },
                    { 16, 3, "逸散", 3, "FM200", "FM200", "類別一" },
                    { 17, 3, "逸散", 3, "廢水處理", "化糞池", "類別一" },
                    { 18, 1, "外購電力", 1, "外購電力", "電力", "類別二" },
                    { 19, 3, "製程", 3, "乙炔", "乙炔", "類別一" },
                    { 20, 3, "製程", 3, "焊條", "焊條", "類別一" }
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
                name: "deviceDatas");

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
