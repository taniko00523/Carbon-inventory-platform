using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Carbon_inventory_platform.Migrations
{
    /// <inheritdoc />
    public partial class InfinityData : Migration
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EasyName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EnglishName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EasyEnglishName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                name: "defaultDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmissionPattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_defaultDevices", x => x.Id);
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
                    Data_Correction = table.Column<int>(type: "int", nullable: false),
                    Device_Correction = table.Column<int>(type: "int", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Num = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
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
                    CO2CEF = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    CO2ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    CO2UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    CH4CEF = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    CH4ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    CH4UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    N2OCEF = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    N2OULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    N2OUUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    HFCSCEF = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    HFCSULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    HFCSUUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    PFCSCEF = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    PFCSULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    PFCSUUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    SF6CEF = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    SF6ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    NF3UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    NF3CEF = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    NF3ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    SF6UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    CEF_Correction = table.Column<int>(type: "int", nullable: false),
                    DataUUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    DataULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
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
                    FullAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
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
                name: "Years",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Num = table.Column<int>(type: "int", nullable: false),
                    Scope1_CO2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope1_CH4 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope1_N2O = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope1_HFCS = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope1_PFCS = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope1_SF6 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope1_NF3 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope2_CO2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope2_CH4 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope2_N2O = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope2_HFCS = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope2_PFCS = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope2_SF6 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope2_NF3 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CO2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CH4 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    N2O = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HFCS = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PFCS = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SF6 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NF3 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    percentage1_CO2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage1_CH4 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage1_N2O = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage1_HFCS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage1_PFCS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage1_SF6 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage1_NF3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage2_CO2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage2_CH4 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage2_N2O = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage2_HFCS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage2_PFCS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage2_SF6 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage2_NF3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    non_move = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    move = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    process = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    escape = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    percentage_nonMove = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage_Move = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage_Process = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage_Escape = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage_Scope1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    percentage_Scope2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Scope1 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Scope2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    All = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    cal_all = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    percentage_CalAll = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    no1_Grade = table.Column<int>(type: "int", nullable: false),
                    no2_Grade = table.Column<int>(type: "int", nullable: false),
                    no3_Grade = table.Column<int>(type: "int", nullable: false),
                    avg_Grade = table.Column<float>(type: "real", nullable: false),
                    all_Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ULL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UUL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Years", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Years_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmissionPattern = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Provess = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Dept = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Num = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Data_Correction = table.Column<int>(type: "int", nullable: false),
                    Device_Correction = table.Column<int>(type: "int", nullable: false),
                    CEF_Correction = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    Emissions = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    data_UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    data_ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    all_UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    all_ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    count_UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    count_ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GHGs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GWP = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    CEF = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    all_UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    all_ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    CEF_UUL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    CEF_ULL = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    Emission = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    isDeleted = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GHGs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GHGs_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GWPs",
                columns: new[] { "Name", "GWP_Year", "Num" },
                values: new object[,]
                {
                    { "海龍-1211", 2022, 1930m },
                    { "CH4", 2022, 27.9m },
                    { "CO2", 2022, 1m },
                    { "FM200", 2022, 3600m },
                    { "N2O", 2022, 273m },
                    { "NF3", 2022, 17400m },
                    { "R-12", 2022, 12500m },
                    { "R-134A", 2022, 1530m },
                    { "R-22", 2022, 1960m },
                    { "R-23", 2022, 14600m },
                    { "R-32", 2022, 771m },
                    { "R-404A", 2022, 4728m },
                    { "R-407C", 2022, 1908m },
                    { "R-410A", 2022, 2256m },
                    { "R-417A", 2022, 2127m },
                    { "R-507A", 2022, 4475m },
                    { "R-600A", 2022, 0.006m },
                    { "SF6", 2022, 24300m }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CEF_Correction", "CH4CEF", "CH4ULL", "CH4UUL", "CO2CEF", "CO2ULL", "CO2UUL", "DataULL", "DataUUL", "EmissionPattern", "HFCSCEF", "HFCSULL", "HFCSUUL", "N2OCEF", "N2OULL", "N2OUUL", "NF3CEF", "NF3ULL", "NF3UUL", "Name", "PFCSCEF", "PFCSULL", "PFCSUUL", "SF6CEF", "SF6ULL", "SF6UUL", "Scope", "Unit", "Year" },
                values: new object[,]
                {
                    { 1, 3, 0.0000246603m, 0.7m, 2m, 2.3328598392m, 0.077167019m, 0.067653277m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000369904m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "自產煤", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 2, 3, 0.0000284702m, 0.7m, 2m, 2.693284704m, 0.077167019m, 0.067653277m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000427054m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "原料煤", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 3, 3, 0.0000254557m, 0.7m, 2m, 2.4081133824m, 0.077167019m, 0.067653277m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000381836m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "燃料煤", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 4, 3, 0.0000297263m, 0.7m, 2m, 2.922093324m, 0.0376398779m, 0.0274669379m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000445894m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "無煙煤", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 5, 3, 0.0000284702m, 0.7m, 2m, 2.693284704m, 0.077167019m, 0.067653277m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000427054m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "焦煤", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 6, 3, 0.0000254557m, 0.7m, 2m, 2.4081133824m, 0.0539112051m, 0.0539112051m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000381836m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "煙煤", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 7, 3, 0.0000205153m, 0.7m, 2m, 1.971522252m, 0.03433923m, 0.0405827263m, 0m, 0m, "固定", 0m, 0m, 0m, 0.000030773m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "亞煙煤(發電)", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 8, 3, 0.0000234461m, 0.7m, 2m, 2.253168288m, 0.03433923m, 0.0405827263m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000351691m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "亞煙煤(其他)", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 9, 3, 0.0000119073m, 0.7m, 2m, 1.2026331792m, 0.1m, 0.1386138614m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000178609m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "褐煤", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 10, 3, 0.0000089053m, 0.7m, 2m, 0.9528696252m, 0.1570093458m, 0.1682242991m, 0m, 0m, "固定", 0m, 0m, 0m, 0.000013358m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "油頁岩", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 11, 3, 0.0000097678m, 0.7m, 2m, 1.0353872664m, 0.0566037736m, 0.0188679245m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000146517m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "泥煤", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 12, 3, 0.0000159098m, 0.7m, 2m, 1.5512094m, 0.1046153846m, 0.1179487179m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000238648m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "煤球", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 13, 3, 0.0000293076m, 0.7m, 2m, 3.1359132m, 0.1056074766m, 0.1121495327m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000439614m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "焦炭", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 14, 3, 0.0001029953m, 0.6666666667m, 2.3333333333m, 3.3473466m, 0.1497435897m, 0.1794871795m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000205991m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "石油焦", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 15, 3, 0.000094203m, 0.6666666667m, 2.3333333333m, 2.19807m, 0.0357142857m, 0.0428571429m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000188406m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "航空汽油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 16, 3, 0.0001004832m, 0.6666666667m, 2.3333333333m, 2.3948496m, 0.0251748252m, 0.0405594406m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000200966m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "航空燃油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 17, 3, 0.0001130436m, 0.6666666667m, 2.3333333333m, 2.76203196m, 0.0300136426m, 0.0300136426m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000226087m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "原油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 18, 3, 0.0000825595m, 0.6666666667m, 2.3333333333m, 2.1190274028m, 0.1m, 0.1090909091m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000165119m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "奧里油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 19, 3, 0.0001326881m, 0.6666666667m, 2.3333333333m, 2.8395246038m, 0.0919003115m, 0.0965732087m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000265376m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "天然氣凝結油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "M3", 0 },
                    { 20, 3, 0.0001067634m, 0.6666666667m, 2.3333333333m, 2.55876282m, 0.0152990264m, 0.0250347705m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000213527m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "煤油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 21, 3, 0.0001079943m, 0.6666666667m, 2.3333333333m, 2.7945625586m, 0.0750341064m, 0.0804911323m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000215989m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "頁岩油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 22, 3, 0.0001055074m, 0.6666666667m, 2.3333333333m, 2.606031792m, 0.020242915m, 0.0094466937m, -0.01m, -0.01m, "固定", 0m, 0m, 0m, 0.0000211015m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "柴油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 23, 3, 0.0000979711m, 0.6666666667m, 2.3333333333m, 2.263132872m, 0.025974026m, 0.0533910534m, -0.01m, -0.01m, "固定", 0m, 0m, 0m, 0.0000195942m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "車用汽油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 24, 3, 0.0001205798m, 0.6666666667m, 2.3333333333m, 3.110959872m, 0.0245478036m, 0.0180878553m, 0m, 0m, "固定", 0m, 0m, 0m, 0.000024116m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "蒸餘油 (燃料油)", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 25, 3, 0.0000277794m, 0.7m, 2m, 1.7528812758m, 0.0237717908m, 0.0396196513m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000027779m, 0.7m, 2m, 0m, 0m, 0m, "液化石油氣", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 26, 3, 0.0000979711m, 0.6666666667m, 2.3333333333m, 2.393761032m, 0.0545702592m, 0.0409276944m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000195942m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "石油腦", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 27, 3, 0.000125604m, 0.6666666667m, 2.3333333333m, 3.3787476m, 0.0954151177m, 0.1140024783m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000251208m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "柏油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 28, 3, 0.0001205798m, 0.6666666667m, 2.3333333333m, 2.946167424m, 0.0190995907m, 0.0259208731m, 0m, 0m, "固定", 0m, 0m, 0m, 0.000024116m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "潤滑油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 29, 3, 0.0001130436m, 0.6666666667m, 2.3333333333m, 2.76203196m, 0.0150068213m, 0.0150068213m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000226087m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "其他油品", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 30, 3, 0.0000464316m, 0.7m, 2m, 2.8601872992m, 0.0827922078m, 0.1136363636m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000046432m, 0.7m, 2m, 0m, 0m, 0m, "乙烷", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 31, 3, 0.0000334944m, 0.7m, 2m, 1.87903584m, 0.0320855615m, 0.0392156863m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000033494m, 0.7m, 2m, 0m, 0m, 0m, "天然氣", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "M3", 0 },
                    { 32, 3, 0.0000376812m, 0.7m, 2m, 2.17043712m, 0.1631944444m, 0.1979166667m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000037681m, 0.7m, 2m, 0m, 0m, 0m, "煉油氣", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "M3", 0 },
                    { 33, 3, 0.0000175846m, 0.7m, 2m, 0.780754464m, 0.1599099099m, 0.2184684685m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000017585m, 0.7m, 2m, 0m, 0m, 0m, "焦爐氣", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "M3", 0 },
                    { 34, 3, 0.0000032531m, 0.7m, 2m, 0.845817336m, 0.1576923077m, 0.1846153846m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000003253m, 0.7m, 2m, 0m, 0m, 0m, "高爐氣", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "M3", 0 },
                    { 35, 3, 0.0002549271m, 0.6666666667m, 2.3333333333m, 0.7792272743m, 0.2006543075m, 0.3195201745m, 0m, 0m, "固定", 0m, 0m, 0m, 0.0000339903m, 0.625m, 2.75m, 0m, 0m, 0m, "一般廢棄物", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "Kg", 0 },
                    { 36, 3, 0.000094203m, 0.6666666667m, 2.3333333333m, 2.19807m, 0.0357142857m, 0.0428571429m, 0m, 0m, "移動", 0m, 0m, 0m, 0.0000188406m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "航空汽油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 37, 3, 0.0001004832m, 0.6666666667m, 2.3333333333m, 2.3948496m, 0.0251748252m, 0.0405594406m, 0m, 0m, "移動", 0m, 0m, 0m, 0.0000200966m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "航空燃油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 38, 3, 0.000816426m, 0.6666666667m, 2.44m, 2.263132872m, 0.025974026m, 0.0533910534m, -0.01m, -0.01m, "移動", 0m, 0m, 0m, 0.0002612563m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "車用汽油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 39, 3, 0.0001371596m, 0.5897435897m, 1.4358974359m, 2.606031792m, 0.020242915m, 0.0094466937m, -0.01m, -0.01m, "移動", 0m, 0m, 0m, 0.0001371596m, 0.6666666667m, 2.0769230769m, 0m, 0m, 0m, "柴油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 40, 3, 0.0001067634m, 0.6666666667m, 2.3333333333m, 2.55876282m, 0.0152990264m, 0.0250347705m, 0m, 0m, "移動", 0m, 0m, 0m, 0.0000213527m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "煤油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 41, 3, 0.0001205798m, 0.6666666667m, 2.3333333333m, 2.946167424m, 0.0190995907m, 0.0259208731m, 0m, 0m, "移動", 0m, 0m, 0m, 0.000024116m, 0.6666666667m, 2.3333333333m, 0m, 0m, 0m, "潤滑油", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 42, 3, 0.0017223239m, 0m, 0m, 1.7528812758m, 0.0237717908m, 0.0396196513m, 0m, 0m, "移動", 0m, 0m, 0m, 0.0000055559m, 0m, 0m, 0m, 0m, 0m, "液化石油氣", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "L", 0 },
                    { 43, 3, 0.0034666704m, 0.4565217391m, 15.7391304348m, 2.11391532m, 0.0320855615m, 0.0392156863m, 0m, 0m, "移動", 0m, 0m, 0m, 0.0001130436m, 0.6666666667m, 24.6666666667m, 0m, 0m, 0m, "液化天然氣", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "M3", 0 },
                    { 44, 3, 0m, 0m, 0m, 0.495m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 0 },
                    { 57, 3, 0.0031875000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "廢水處理", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 58, 3, 0m, 0m, 0m, 1m, 0m, 0m, 0m, 0m, "逸散", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "二氧化碳", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 59, 1, 0m, 0m, 0m, 3.3841653850m, 0m, 0m, 0m, 0m, "製程", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "乙炔", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 60, 1, 0m, 0m, 0m, 3.6666666666m, 0m, 0m, 0m, 0m, "製程", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "焊條", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 61, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.003000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "冰箱", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 62, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.003000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "飲水機", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 63, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.055000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "商用冰箱", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 64, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.200000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "中、大型冰箱", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 65, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.330000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "低溫冷凍車", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 66, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.160000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "乾燥機", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 67, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.160000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "工業冷藏、冷凍", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 68, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.160000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "食品加工冷藏、冷凍", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 69, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.090000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "冰水主機", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 70, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.030000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "冷氣機", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 71, 3, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "逸散", 0.200000m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "車用空調", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 72, 1, 0m, 0m, 0m, 3.0260000000m, 0m, 0m, 0m, 0m, "製程", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "丁烷", 0m, 0m, 0m, 0m, 0m, 0m, "類別一", "", 0 },
                    { 73, 3, 0m, 0m, 0m, 0.509m, -0.07m, 0.07m, -0.01m, -0.01m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, "外購電力", 0m, 0m, 0m, 0m, 0m, 0m, "類別二", "", 110 }
                });

            migrationBuilder.InsertData(
                table: "defaultDevices",
                columns: new[] { "Id", "EmissionPattern", "Material", "Name", "Scope", "Type" },
                values: new object[,]
                {
                    { 1, "固定", "柴油", "緊急發電機", "類別一", "" },
                    { 2, "移動", "柴油", "公務車", "類別一", "" },
                    { 3, "移動", "車用汽油", "公務車", "類別一", "" },
                    { 4, "逸散", "R-410A", "冷氣機", "類別一", "" },
                    { 5, "逸散", "R-134A", "飲水機", "類別一", "" },
                    { 6, "逸散", "R-134A", "乾燥機", "類別一", "" },
                    { 7, "逸散", "R-134A", "冰水主機", "類別一", "" },
                    { 8, "逸散", "R-134A", "車用空調", "類別一", "" },
                    { 9, "逸散", "廢水處理", "化糞池", "類別一", "" },
                    { 10, "外購電力", "外購電力", "電力", "類別二", "" }
                });

            migrationBuilder.InsertData(
                table: "deviceDatas",
                columns: new[] { "Id", "Data_Correction", "Device_Correction", "EmissionPattern", "Material", "Name", "Scope", "unit" },
                values: new object[,]
                {
                    { 1, 3, 3, "逸散", "R-410A", "冷氣機", "類別一", "公斤" },
                    { 2, 3, 3, "逸散", "R-134A", "冰水主機", "類別一", "公斤" },
                    { 3, 3, 3, "逸散", "R-134A", "冰箱", "類別一", "公斤" },
                    { 4, 3, 3, "逸散", "R-134A", "飲水機", "類別一", "公斤" },
                    { 5, 3, 3, "逸散", "R-134A", "乾燥機", "類別一", "公斤" },
                    { 6, 3, 3, "逸散", "R-134A", "車用空調", "類別一", "公斤" },
                    { 7, 3, 3, "逸散", "R-134A", "工業冷藏、冷凍", "類別一", "公斤" },
                    { 8, 3, 3, "固定", "柴油", "緊急發電機", "類別一", "公升" },
                    { 9, 2, 2, "固定", "液化石油氣", "廚房", "類別一", "公斤" },
                    { 10, 2, 2, "移動", "車用汽油", "公務車", "類別一", "公升" },
                    { 11, 2, 2, "移動", "柴油", "堆高機", "類別一", "公升" },
                    { 12, 3, 3, "逸散", "二氧化碳", "CO2滅火器", "類別一", "公斤" },
                    { 13, 3, 3, "逸散", "二氧化碳", "二氧化碳", "類別一", "公斤" },
                    { 14, 3, 3, "逸散", "二氧化碳", "WD40", "類別一", "公斤" },
                    { 15, 3, 3, "逸散", "海龍-1211", "海龍1211", "類別一", "公斤" },
                    { 16, 3, 3, "逸散", "FM200", "FM200", "類別一", "公斤" },
                    { 17, 3, 3, "逸散", "廢水處理", "化糞池", "類別一", "人" },
                    { 18, 1, 1, "外購電力", "外購電力", "電力", "類別二", "度" },
                    { 19, 3, 3, "製程", "乙炔", "乙炔", "類別一", "公斤" },
                    { 20, 3, 3, "製程", "焊條", "焊條", "類別一", "公斤" },
                    { 21, 3, 3, "逸散", "R-134A", "工業冷藏、冷凍", "類別一", "公斤" },
                    { 22, 3, 3, "逸散", "R-134A", "商用冰箱", "類別一", "公斤" },
                    { 23, 3, 3, "逸散", "R-134A", "中、大型冰箱", "類別一", "公斤" },
                    { 24, 3, 3, "逸散", "R-134A", "低溫冷凍車", "類別一", "公斤" },
                    { 25, 3, 3, "逸散", "R-134A", "食品加工冷藏、冷凍", "類別一", "公斤" },
                    { 26, 3, 3, "製程", "丁烷", "瓦斯罐", "類別一", "公斤" }
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
                name: "IX_Devices_YearId",
                table: "Devices",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_GHGs_DeviceId",
                table: "GHGs",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Years_AreaId",
                table: "Years",
                column: "AreaId");
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
                name: "defaultDevices");

            migrationBuilder.DropTable(
                name: "deviceDatas");

            migrationBuilder.DropTable(
                name: "GHGs");

            migrationBuilder.DropTable(
                name: "GWPs");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Years");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
